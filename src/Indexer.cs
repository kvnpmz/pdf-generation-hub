using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

public class Indexer : IStep
{
    private static readonly string ConnectionString =
        $"Data Source={Path.Combine(Paths.RootPath, "projects.db")}";

    private static readonly string[] ListingColumns = { "title", "tags", "description" };

    private static readonly Regex ListingSectionPattern = new(
        @"^(TITLE|TAGS|DESCRIPTION)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public async Task ExecuteAsync(Context context)
    {
        using var conn = new SqliteConnection(ConnectionString);
        await conn.OpenAsync();

        await CreateTable(conn);

        var existingIds = await IndexFolders(conn, Path.Combine(Paths.RootPath, "documents"));
        await DeleteMissingProjects(conn, existingIds);
    }

    private async Task CreateTable(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Projects (
                    id TEXT PRIMARY KEY,
                    folder_path TEXT,
                    completed_date TEXT,
                    overrides TEXT,
                    output_name TEXT,
                    is_editable INTEGER,
                    template TEXT
                    );";
        await cmd.ExecuteNonQueryAsync();

        await EnsureListingColumns(conn);
    }

    // Existing databases predate the listing columns, so add them in place.
    private static async Task EnsureListingColumns(SqliteConnection conn)
    {
        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var pragma = conn.CreateCommand())
        {
            pragma.CommandText = "PRAGMA table_info(Projects);";
            using var reader = await pragma.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                existing.Add(reader.GetString(1));
        }

        foreach (var column in ListingColumns)
        {
            if (existing.Contains(column))
                continue;

            using var alter = conn.CreateCommand();
            alter.CommandText = $"ALTER TABLE Projects ADD COLUMN {column} TEXT;";
            await alter.ExecuteNonQueryAsync();
        }
    }

    // Reads TITLE / TAGS / DESCRIPTION sections from listing.txt when present.
    private static (string? Title, string? Tags, string? Description) ReadListing(string directory)
    {
        var path = Path.Combine(directory, "listing.txt");
        if (!File.Exists(path))
            return (null, null, null);

        var sections = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        string? current = null;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
                continue;

            var header = ListingSectionPattern.Match(line);
            if (header.Success)
            {
                current = header.Groups[1].Value.ToUpperInvariant();
                continue;
            }

            if (current == null)
                continue;

            if (!sections.TryGetValue(current, out var lines))
            {
                lines = new List<string>();
                sections[current] = lines;
            }

            lines.Add(line);
        }

        string? Value(string key) =>
            sections.TryGetValue(key, out var lines) && lines.Count > 0
                ? string.Join(" ", lines)
                : null;

        return (Value("TITLE"), Value("TAGS"), Value("DESCRIPTION"));
    }

    private async Task<HashSet<string>> IndexFolders(SqliteConnection conn, string rootFolder)
    {
        var existingIds = new HashSet<string>();

        foreach (var directory in Directory.GetDirectories(rootFolder))
        {
            var id = Path.GetFileName(directory);
            existingIds.Add(id);

            var configPath = Path.Combine(directory, "config.tl");
            if (!File.Exists(configPath))
                continue;

            var lua = File.ReadAllText(configPath);

            string template = Extract(lua, "template");
            int editable = Extract(lua, "is_editable") == "true" ? 1 : 0;
            var completed = Extract(lua, "completed_date");
            var (title, tags, description) = ReadListing(directory);

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"
                INSERT OR REPLACE INTO Projects 
                (id, folder_path, completed_date, overrides, output_name, is_editable, template, title, tags, description)
                VALUES 
                ($id, $path, $completed, $overrides, $output, $edit, $template, $title, $tags, $description);
            ";
            cmd.Parameters.AddWithValue("$id", Path.GetFileName(directory));
            cmd.Parameters.AddWithValue("$path", directory);
            cmd.Parameters.AddWithValue("$edit", editable);
            cmd.Parameters.AddWithValue("$template", template);
            cmd.Parameters.AddWithValue(
                "$completed",
                string.IsNullOrWhiteSpace(completed) ? DBNull.Value : completed
            );
            cmd.Parameters.AddWithValue("$overrides", Extract(lua, "overrides"));
            cmd.Parameters.AddWithValue("$output", Extract(lua, "output_name"));
            cmd.Parameters.AddWithValue("$title", (object?)title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$tags", (object?)tags ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$description", (object?)description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        return existingIds;
    }

    private async Task DeleteMissingProjects(SqliteConnection conn, HashSet<string> validIds)
    {
        if (validIds.Count == 0)
            return;

        using var cmd = conn.CreateCommand();
        var placeholderList = new List<string>();
        int index = 0;

        foreach (var id in validIds)
        {
            placeholderList.Add($"$id{index}");
            index++;
        }

        var placeholders = string.Join(",", placeholderList);

        cmd.CommandText =
            $@"
            DELETE FROM Projects 
            WHERE id NOT IN ({placeholders});
        ";

        int i = 0;
        foreach (var id in validIds)
        {
            cmd.Parameters.AddWithValue($"$id{i}", id);
            i++;
        }

        await cmd.ExecuteNonQueryAsync();
    }

    private string Extract(string lua, string key)
    {
        var match = Regex.Match(lua, $@"{key}\s*=\s*['""]?(.*?)['""]?[\r\n,]");

        return match.Success ? match.Groups[1].Value : "";
    }
}
