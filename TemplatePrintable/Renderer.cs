using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NLua;

public class Renderer : IPipelineStep  
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        using var lua = new Lua();
        lua.State.Encoding = System.Text.Encoding.UTF8; 

        lua.DoString("require('init')");
        var setupFunction = (LuaFunction)lua["SetupDocument"];

        var results = (object[])setupFunction.Call(context.DocumentId);
        var config = (LuaTable)results[0];
        var renderTemplate = (LuaFunction)results[1];

        string outputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output";
        string template = config["template"]?.ToString() ?? "default";

        lua["data"] = new { config = config, renderTemplate = renderTemplate };
        var generateDocument = (LuaFunction)lua["GenerateBase"];

        string html = (string)generateDocument.Call(lua["data"])[0];
        var applyStyling = (LuaFunction)lua["ApplyStyling"];

        html = (string)applyStyling.Call(html, config)[0];
        File.WriteAllText("preview.html", html);

        context.Html = html;
        context.OutputName = outputName;
    }
}

