using NLua;

public class RenderStrategy : IPipelineStep  
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        using var lua = new Lua();
        lua.DoString("require('init')");

        var setupFunction = (LuaFunction)lua["SetupDocument"];
        var results = (object[])setupFunction.Call(context.DocId);

        var config = (LuaTable)results[0];
        var renderTemplate = (LuaFunction)results[1];

        string outputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output";
        string template = config["template"]?.ToString() ?? "default";

        lua["data"] = new { config = config, renderTemplate = renderTemplate };
        var generateDoc = (LuaFunction)lua["GenerateDocument"];

        string html = (string)generateDoc.Call(lua["data"])[0];
        var applyStyling = (LuaFunction)lua["ApplyStyling"];

        html = (string)applyStyling.Call(html, config["template"])[0];
        Console.WriteLine(html);

        context.Html = html;
        context.OutputName = outputName;
    }
}

