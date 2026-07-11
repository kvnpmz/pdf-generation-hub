using NLua;

public static class StyleApplier
{
    private static string? ReadFile(string path)
    {
        if (!File.Exists(path))
            return null;

        return File.ReadAllText(path);
    }

    public static string Apply(string html, LuaTable config)
    {
        string baseCss = ReadFile(
            Path.Combine(Paths.RootPath, "base.css")
        ) ?? string.Empty;

        string style = config["style"]?.ToString() ?? "style";
        string template = config["template"]?.ToString() ?? "default";

        string themeFilename = $"{style}.css";

        string themePath = Path.Combine(
            Paths.RootPath,
            "templates",
            template,
            themeFilename
        );

        string theme = string.Empty;

        string? themeContent = ReadFile(themePath);
        if (themeContent != null)
        {
            theme = themeContent;
        }

        string? overrides = config["overrides"]?.ToString();

        if (!string.IsNullOrEmpty(overrides))
        {
            string overridesPath = Path.Combine(
                Paths.RootPath,
                "templates",
                template,
                $"{overrides}.css"
            );

            string? overridesContent = ReadFile(overridesPath);

            if (overridesContent != null)
            {
                theme += overridesContent;
            }
        }

        string styleBlock = $"<style>{baseCss}</style><style>{theme}</style></head>";

        const string closingHead = "</head>";
        int index = html.IndexOf(closingHead, StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            return html[..index] +
                   styleBlock +
                   html[(index + closingHead.Length)..];
        }

        return html;
    }
}
