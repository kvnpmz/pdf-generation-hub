using System.Collections.Generic;
using TemplatePrintable.Core;
using NLua;

namespace TemplateRender;

public class TemplateRender : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>();
        var grid = new List<string>();

        grid.Add(Dom.Div("cell", Dom.H3("", "")));
        grid.Add(Dom.Div("cell", Dom.H3("", "Investments Purchased")));
        grid.Add(Dom.Div("cell", Dom.H3("", "Price")));
        grid.Add(Dom.Div("cell", Dom.H3("", "Quantity")));
        grid.Add(Dom.Div("cell", Dom.H3("", "Holding Short Term or Long Term")));
        grid.Add(Dom.Div("cell", Dom.H3("", "Expected ROI")));

        var months = new[]
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };

        foreach (var month in months)
        {
            grid.Add(Dom.Div("cell", Dom.Tag("h4", "", month)));

            for (int i = 0; i < 5; i++)
            {
                grid.Add(CreateSplitCell());
            }
        }

        html.Add(Dom.H1("", config["header"]?.ToString() ?? ""));

        html.Add(Dom.Div("grid", string.Concat(grid)));
        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output"
        };
    }

    private static string CreateSplitCell()
    {
        return Dom.Div(
                "cell split",
                Dom.Div("split-container", Dom.Tag("textarea", "", "")),
                Dom.Div("split-container", Dom.Tag("textarea", "", "")),
                Dom.Div("split-container", Dom.Tag("textarea", "", ""))
                );
    }
}
