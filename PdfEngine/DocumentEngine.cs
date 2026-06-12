using NLua;

public class DocumentEngine
{
    public async Task<string> GenerateHtml(string docName)
    {
        string docPath = $"documents/{docName}"; 
        string layoutTable = await File.ReadAllTextAsync($"{docPath}/data.tl"); 

        using var lua = new Lua();
        lua.DoString("require('main')");

        var results = lua.DoString(layoutTable);
        if (results.Length == 0)
            throw new Exception("data.tl returned no value");

        var layout = (LuaTable)results[0];
        string template = layout["template"]?.ToString() ?? "default";

        string renderScript =
            await File.ReadAllTextAsync($"templates/{template}.tl");
        var renderBody = lua.DoString(renderScript)[0];

        var data = new {
            layout = layout,
            renderBody = renderBody
        };

        lua["data"] = data;
        var generateDoc = (LuaFunction)lua["GenerateDocument"];

        var result = generateDoc.Call(lua["data"]);
        string html = (string)result[0];

        string baseCss = await File.ReadAllTextAsync("templates/base_styles.css");
        string customCss = await File.ReadAllTextAsync($"{docPath}/style.css");

        string styleBlock = $"<style>{baseCss}</style><style>{customCss}</style></head>";
        html = html.Replace("</head>", styleBlock);

        Console.WriteLine(html);
        return html;
    }
}

