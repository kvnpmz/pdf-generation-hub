using System.Collections.Generic;
using NLua;

namespace InvestmentGoals;

public class TemplateRender : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>();
        var main = new List<string>();

        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "")));
        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "Investments Purchased")));
        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "Price")));
        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "Quantity")));
        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "Holding Short Term or Long Term")));
        main.Add(Dom.Div("cell label", Dom.Tag("H4", "", "Expected ROI")));

        var months = new[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        };

        foreach (var month in months)
        {
            main.Add(Dom.Div("cell label", Dom.Tag("h5", "", month)));

            for (int i = 0; i < 5; i++)
            {
                main.Add(CreateSplitCell());
            }
        }

        string header = config["header"]?.ToString() ?? "";
        string[] words = header.Split(
            new[] { ' ', '\t', '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        html.Add(Dom.H1("", $"{words[0]} {words[1]}"));
        html.Add(Dom.H2("", $"{words[2]} {words[3]}"));

        html.Add(
            Dom.Header(
                "",
                Dom.Div("", Dom.H3("", "Name"), Dom.Div("line", "")),
                Dom.Div("", Dom.H3("", "Year"), Dom.Div("line", ""))
            )
        );

        html.Add(Dom.MainTag("", string.Concat(main)));

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output",
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
