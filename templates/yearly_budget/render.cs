using System.Collections.Generic;
using NLua;

public class YearlyBudgetPlanner : IRenderer
{
    private static readonly string[] DefaultMonths =
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

    private static readonly string[] Columns =
    {
        "Month",
        "Income",
        "Expenses",
        "Difference",
        "Savings",
        "Notes",
    };

    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>();

        string header = config["header"]?.ToString() ?? "Yearly Budget Planner";
        string totalLabel = config["total_label"]?.ToString() ?? "Annual Total";

        html.Add(Dom.H1("", header));
        html.Add(
            Dom.Header(
                "",
                Dom.Div("header-item", Dom.Div("", "Year:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Total Income:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Total Expenses:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Total Savings:"), Dom.Div("line", ""))
            )
        );

        var headCells = new List<string>();
        foreach (string column in Columns)
        {
            string cellClass = column switch
            {
                "Month" => "col-month",
                "Notes" => "col-notes",
                _ => "col-data",
            };

            headCells.Add(Dom.Th(cellClass, column));
        }

        var rows = new List<string>();

        foreach (string month in ReadMonths(config))
        {
            rows.Add(
                Dom.Tr(
                    "",
                    Dom.Td("bullet-base bullet-month", month),
                    Dom.Td("", ""),
                    Dom.Td("", ""),
                    Dom.Td("", ""),
                    Dom.Td("", ""),
                    Dom.Td("", "")
                )
            );
        }

        rows.Add(
            Dom.Tr(
                "total-row",
                Dom.Td("", totalLabel),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", "")
            )
        );

        html.Add(
            Dom.MainTag(
                "",
                Dom.Table(
                    "",
                    Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                    Dom.Tbody("", rows.ToArray())
                ),
                Dom.Div(
                    "footer-note",
                    Dom.Div("footer-label", config["footer"]?.ToString() ?? "Notes:")
                )
            )
        );

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "yearly_budget_planner",
        };
    }

    private static IEnumerable<string> ReadMonths(LuaTable config)
    {
        var fromConfig = new List<string>();

        if (config["months"] is LuaTable months)
        {
            foreach (object value in months.Values)
            {
                string month = value?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(month))
                    fromConfig.Add(month);
            }
        }

        return fromConfig.Count > 0 ? fromConfig : DefaultMonths;
    }
}
