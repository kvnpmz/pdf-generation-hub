using System;
using System.Collections.Generic;
using NLua;

namespace MoveInBudget;

public class TemplateRender : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildStartup(config),
                BuildSetupTable(config)
            ),
            BuildFooter(config),
        };

        string outputName = Read(config, "output_name");

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = string.IsNullOrEmpty(outputName) ? "movein_budget" : outputName,
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
            Dom.Tag("p", "subtitle", Read(config, "subtitle")),
            Dom.Div("header-fields", fields.ToArray())
        );
    }

    private static string BuildStartup(LuaTable config)
    {
        var cells = new List<string>();

        foreach (string label in Strings(config, "startup_fields"))
        {
            cells.Add(
                Dom.Div(
                    "startup-field",
                    Dom.Tag("span", "field-label", Escape(label)),
                    Dom.Tag("span", "field-line", "")
                )
            );
        }

        string totalLabel = Read(config, "startup_total");
        if (!string.IsNullOrEmpty(totalLabel))
        {
            cells.Add(
                Dom.Div(
                    "startup-field total",
                    Dom.Tag("span", "field-label", Escape(totalLabel)),
                    Dom.Tag("span", "field-line", "")
                )
            );
        }

        return Dom.Section(
            "startup",
            Dom.Div(
                "section-head",
                Dom.H2("", Read(config, "startup_title")),
                Dom.Tag("span", "section-aside", Read(config, "startup_note"))
            ),
            Dom.Div("startup-grid", cells.ToArray())
        );
    }

    private static string BuildSetupTable(LuaTable config)
    {
        string[] columnClasses =
        {
            "col-item",
            "col-priority",
            "col-money",
            "col-money",
            "col-bought",
            "col-notes",
        };

        var headCells = new List<string>();
        int index = 0;

        foreach (string column in Strings(config, "columns"))
        {
            string cellClass = index < columnClasses.Length ? columnClasses[index] : "";
            headCells.Add(Dom.Th(cellClass, Escape(column)));
            index++;
        }

        var rows = new List<string>();

        foreach (object entry in Values(config, "rooms"))
        {
            if (entry is not LuaTable room)
                continue;

            string roomName = Escape(room["name"]?.ToString() ?? "");
            rows.Add($"<tr class=\"room-row\"><td colspan=\"6\">{roomName}</td></tr>");

            foreach (object itemEntry in Values(room, "items"))
            {
                if (itemEntry is not LuaTable item)
                    continue;

                string priority = item["priority"]?.ToString() ?? "wait";
                bool buyNow = string.Equals(priority, "now", StringComparison.OrdinalIgnoreCase);

                rows.Add(
                    Dom.Tr(
                        "item-row",
                        Dom.Td("col-item", Escape(item["name"]?.ToString() ?? "")),
                        Dom.Td(
                            "col-priority " + (buyNow ? "now" : "wait"),
                            buyNow ? "Buy now" : "Can wait"
                        ),
                        Dom.Td("col-money", Escape(item["est"]?.ToString() ?? "")),
                        Dom.Td("col-money", ""),
                        Dom.Td("col-bought", Dom.Tag("span", "box", "")),
                        Dom.Td("col-notes", "")
                    )
                );
            }
        }

        rows.Add(
            Dom.Tr(
                "total-row",
                Dom.Td("", Escape(Read(config, "total_label"))),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", ""),
                Dom.Td("", "")
            )
        );

        return Dom.Section(
            "setup",
            Dom.Div(
                "section-head",
                Dom.H2("", Read(config, "table_title")),
                Dom.Tag("span", "section-aside", Read(config, "table_note"))
            ),
            Dom.Table(
                "",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", rows.ToArray())
            )
        );
    }

    private static string BuildFooter(LuaTable config)
    {
        var items = new List<string>();

        foreach (string item in Strings(config, "footer_items"))
        {
            items.Add(Dom.Li("", Escape(item)));
        }

        return Dom.Tag(
            "footer",
            "page-footer",
            Dom.Div(
                "first-days",
                Dom.Tag("span", "first-days-title", Read(config, "footer_title")),
                Dom.Ul("", items.ToArray())
            ),
            Dom.Tag("p", "footer-note", Read(config, "footer_note"))
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
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }
}
