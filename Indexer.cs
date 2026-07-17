using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

public class Indexer : IStep
{
    private static readonly string ConnectionString =
        $"Data Source={Path.Combine(Paths.RootPath, "projects.db")}";

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

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"
                INSERT OR REPLACE INTO Projects 
                (id, folder_path, completed_date, overrides, output_name, is_editable, template)
                VALUES 
                ($id, $path, $completed, $overrides, $output, $edit, $template);
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
