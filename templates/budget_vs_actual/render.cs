using System;
using System.Collections.Generic;
using NLua;

namespace BudgetVsActual;

public class TemplateRender : IRenderer
{
    private static readonly string[] DefaultColumns =
    {
        "Category",
        "Budget",
        "Actual",
        "Difference",
        "% Used",
        "Roll over",
    };

    private static readonly string[] ColumnClasses =
    {
        "col-category",
        "col-money",
        "col-money",
        "col-money",
        "col-pct",
        "col-roll",
    };

    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag("", BuildReview(config)),
            BuildTakeaways(config),
        };

        string outputName = Read(config, "output_name");

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = string.IsNullOrEmpty(outputName) ? "budget_vs_actual" : outputName,
        };
    }

    private static string BuildHeader(LuaTable config)
    {
        var fields = new List<string>();

        foreach (string label in Strings(config, "header_fields"))
        {
            fields.Add(
                Dom.Div(
                    "header-item",
                    Dom.Div("", Escape(label)),
                    Dom.Div("line", "")
                )
            );
        }

        return Dom.Header(
            "",
            Dom.H1("", Read(config, "header")),
            Dom.Tag("p", "subtitle", Escape(Read(config, "subtitle"))),
            Dom.Div("header-fields", fields.ToArray())
        );
    }

    private static string BuildReview(LuaTable config)
    {
        var columns = new List<string>(Strings(config, "columns"));

        if (columns.Count == 0)
            columns.AddRange(DefaultColumns);

        var headCells = new List<string>();

        for (int i = 0; i < columns.Count; i++)
        {
            string cellClass = i < ColumnClasses.Length ? ColumnClasses[i] : "col-money";
            headCells.Add(Dom.Th(cellClass, Escape(columns[i])));
        }

        var rows = new List<string>();

        foreach (object groupEntry in Values(config, "groups"))
        {
            if (groupEntry is not LuaTable group)
                continue;

            string groupName = Escape(group["name"]?.ToString() ?? "");
            string groupNote = Escape(group["note"]?.ToString() ?? "");

            rows.Add(
                "<tr class=\"group-row\"><td colspan=\""
                    + columns.Count
                    + "\">"
                    + "<span class=\"group-name\">"
                    + groupName
                    + "</span>"
                    + (
                        string.IsNullOrEmpty(groupNote)
                            ? ""
                            : "<span class=\"group-note\">" + groupNote + "</span>"
                    )
                    + "</td></tr>"
            );

            bool shaded = false;

            foreach (object itemEntry in Values(group, "items"))
            {
                string item = itemEntry?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(item))
                    continue;

                rows.Add(BuildItemRow(item, columns.Count, shaded));
                shaded = !shaded;
            }

            rows.Add(
                BuildSubtotalRow(
                    group["name"]?.ToString() + " " + Read(config, "subtotal_label"),
                    columns.Count
                )
            );
        }

        rows.Add(BuildTotalRow(Read(config, "total_label"), columns.Count));

        return Dom.Section(
            "review",
            Dom.Div("section-head", Dom.H2("", Read(config, "table_title"))),
            Dom.Table(
                "",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", rows.ToArray())
            ),
            Dom.Tag("p", "table-note", Escape(Read(config, "formula_note")))
        );
    }

    private static string BuildItemRow(string item, int columnCount, bool shaded)
    {
        var cells = new List<string>
        {
            Dom.Td("col-category bullet-base bullet-category", Escape(item)),
        };

        for (int i = 1; i < columnCount; i++)
        {
            string cellClass = i < ColumnClasses.Length ? ColumnClasses[i] : "col-money";

            switch (cellClass)
            {
                case "col-pct":
                    cells.Add(Dom.Td(cellClass, Dom.Tag("span", "unit", "%")));
                    break;
                case "col-roll":
                    cells.Add(Dom.Td(cellClass, Dom.Tag("span", "box", "")));
                    break;
                default:
                    cells.Add(Dom.Td(cellClass, ""));
                    break;
            }
        }

        return Dom.Tr(shaded ? "item-row shade" : "item-row", cells.ToArray());
    }

    private static string BuildSubtotalRow(string label, int columnCount)
    {
        var cells = new List<string>
        {
            Dom.Td("col-category", Escape(label)),
        };

        for (int i = 1; i < columnCount; i++)
        {
            string cellClass = i < ColumnClasses.Length ? ColumnClasses[i] : "col-money";
            cells.Add(Dom.Td(cellClass, ""));
        }

        return Dom.Tr("subtotal-row", cells.ToArray());
    }

    private static string BuildTotalRow(string label, int columnCount)
    {
        var cells = new List<string>
        {
            Dom.Td("col-category", Escape(string.IsNullOrEmpty(label) ? "Month total" : label)),
        };

        for (int i = 1; i < columnCount; i++)
        {
            string cellClass = i < ColumnClasses.Length ? ColumnClasses[i] : "col-money";
            cells.Add(Dom.Td(cellClass, ""));
        }

        return Dom.Tr("total-row", cells.ToArray());
    }

    private static string BuildTakeaways(LuaTable config)
    {
        var rows = new List<string>();
        int number = 0;

        foreach (string label in Strings(config, "takeaways"))
        {
            number++;

            rows.Add(
                Dom.Div(
                    "takeaway-row",
                    Dom.Tag("span", "takeaway-num", number.ToString()),
                    Dom.Tag("span", "takeaway-label", Escape(label)),
                    Dom.Tag("span", "field-caption", "category"),
                    Dom.Tag("span", "field-line", ""),
                    Dom.Div(
                        "field-amount",
                        Dom.Tag("span", "dollar", "$"),
                        Dom.Tag("span", "field-line", "")
                    )
                )
            );
        }

        return Dom.Section(
            "takeaways",
            Dom.Div(
                "takeaways-head",
                Dom.H2("", Read(config, "takeaways_title")),
                Dom.Tag("span", "takeaways-aside", Escape(Read(config, "takeaways_note")))
            ),
            Dom.Div("takeaway-rows", rows.ToArray())
        );
    }

    private static string Read(LuaTable config, string key)
    {
        return config[key]?.ToString() ?? "";
    }

    private static IEnumerable<string> Strings(LuaTable config, string key)
    {
        foreach (object value in Values(config, key))
        {
            string? text = value?.ToString();
            if (!string.IsNullOrEmpty(text))
                yield return text;
        }
    }

    private static System.Collections.ICollection Values(LuaTable config, string key)
    {
        return config[key] is LuaTable table ? table.Values : Array.Empty<object>();
    }

    private static string Escape(string value)
    {
        return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
