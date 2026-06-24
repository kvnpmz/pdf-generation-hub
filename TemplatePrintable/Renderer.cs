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

        html = (string)applyStyling.Call(html, config)[0];

        if (Convert.ToBoolean(1))
        {
            string htmlDecoded = WebUtility.HtmlDecode(html);

            string cssDecoded = Regex.Replace(
                htmlDecoded,
                @"\\([0-9a-fA-F]{1,6})\s?",
                match =>
                {
                    string hex = match.Groups[1].Value;
                    int codePoint = Convert.ToInt32(hex, 16);
                    return char.ConvertFromUtf32(codePoint);
                });

            File.WriteAllText("render-preview.html", cssDecoded, Encoding.UTF8);
        }

        context.Html = html;
        context.OutputName = outputName;
    }
}

