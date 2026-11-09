using System.Collections.Generic;
using NLua;

public class MonthlyBudgetSage : IRenderer
{
    private static readonly string[] Columns =
    {
        "Expenses Category",
        "Budgeted",
        "Actual",
        "Balance",
        "Notes",
        "Paid",
    };

    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>();

        string header = config["header"]?.ToString() ?? "Monthly Budget Template";

        html.Add(Dom.H1("", header));
        html.Add(
            Dom.Header(
                "",
                Dom.Div("header-item", Dom.Div("", "Month:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Total Income:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Income – Expenses:"), Dom.Div("line", "")),
                Dom.Div("header-item", Dom.Div("", "Balance:"), Dom.Div("line", ""))
            )
        );

        var headCells = new List<string>();
        foreach (string column in Columns)
        {
            string cellClass = column switch
            {
                "Expenses Category" => "col-category",
                "Notes" => "col-notes",
                "Paid" => "col-paid",
                _ => "col-data",
            };

            headCells.Add(Dom.Th(cellClass, column));
        }

        var rows = new List<string>();

        if (config["categories"] is LuaTable categories)
        {
            foreach (object value in categories.Values)
            {
                string category = value?.ToString() ?? "";

                rows.Add(
                    Dom.Tr(
                        "",
                        Dom.Td("bullet-base bullet-category", category),
                        Dom.Td("", ""),
                        Dom.Td("", ""),
                        Dom.Td("", ""),
                        Dom.Td("", ""),
                        Dom.Td("bullet-base bullet-paid", "")
                    )
                );
            }
        }

        rows.Add(
            Dom.Tr(
                "total-row",
                Dom.Td("", "Total Expenses"),
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
                )
            )
        );

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "monthly_budget_template",
        };
    }
}
