using System.Text;
using NLua;

public class Renderer : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        lua.DoString("local tl = require('tl'); tl.loader();");

        var exports = (LuaTable)lua.DoString("return require('init')")[0];
        var render = (LuaFunction)exports["Render"];

        var result = (LuaTable)render.Call(context.DocumentId)[0];
        context.Html = result["html"]?.ToString() ?? string.Empty;

        context.OutputName = result["outputName"]?.ToString() ?? "output";
        string formattedHtml = HtmlFormatter.Beautify(context.Html);
        
        File.WriteAllText("preview.html", formattedHtml);
    }
}
