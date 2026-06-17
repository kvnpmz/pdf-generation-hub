using NLua;

public class RenderStrategy : IPipelineStep  
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        using var lua = new Lua();
        lua.DoString("require('init')");

        string docPath = $"documents/{context.DocId}";
        string configContent = await File.ReadAllTextAsync($"{docPath}/config.tl"); 

        var luaResults = lua.DoString(configContent);
        if (luaResults.Length == 0)
            throw new Exception("config.tl returned no value");

        var config = (LuaTable)luaResults[0];
        string outputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output";
        string template = config["template"]?.ToString() ?? "default";

        string renderScript = await File.ReadAllTextAsync($"templates/{template}/render.tl");
        var renderTemplate = lua.DoString(renderScript)[0];

        var data = new {
            config = config,
            renderTemplate = renderTemplate
        };

        lua["data"] = data;
        var generateDoc = (LuaFunction)lua["GenerateDocument"];

        var result = generateDoc.Call(lua["data"]);
        string html = (string)result[0];

        string baseCss = await File.ReadAllTextAsync("templates/base/style.css");
        string cssPath = $"templates/{template}/style.css";
        string customCss = File.Exists(cssPath)
            ? await File.ReadAllTextAsync(cssPath)
            : "";

        string styleBlock = $"<style>{baseCss}</style><style>{customCss}</style></head>";
        html = html.Replace("</head>", styleBlock);

//        Console.WriteLine(html);
//        return (html, outputName);
        context.Html = html;
        context.OutputName = outputName;
    }
}

