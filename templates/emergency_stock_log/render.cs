using System;
using System.Collections.Generic;
using System.Globalization;
using NLua;

public class EmergencyStockLog : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildStock(config),
                BuildRotation(config),
                BuildRoutine(config)
            ),
            Dom.Tag(
                "footer",
                "page-footer",
                Dom.Tag("span", "footer-text", Read(config, "footer_note")),
                Dom.Tag("span", "footer-mark", "Pantry")
            ),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "emergency_stock_log",
        };
    }

    private static string BuildHeader(LuaTable config)
    {
        var fields = new List<string>();

        foreach (object label in Values(config, "plan_card_fields"))
        {
            fields.Add(
                Dom.Div(
                    "plan-field",
                    Dom.Tag("span", "field-label", label?.ToString() ?? ""),
                    Dom.Tag("span", "field-line", "")
                )
            );
        }

        var card = new List<string>
        {
            Dom.Tag("span", "plan-card-title", Read(config, "plan_card_title")),
        };
        card.AddRange(fields);

        return Dom.Tag(
            "header",
            "masthead",
            Dom.Div(
                "masthead-main",
                Dom.Tag("span", "kicker", Read(config, "kicker")),
                Dom.H1("", Read(config, "header_line1")),
                Dom.H1("line-two", Read(config, "header_line2")),
                Dom.Tag("p", "subtitle", Read(config, "subtitle"))
            ),
            Dom.Tag("div", "plan-card", card.ToArray())
        );
    }

    private static string BuildStock(LuaTable config)
    {
        var households = new List<string>();

        foreach (object entry in Values(config, "households"))
        {
            if (entry is LuaTable household)
                households.Add(household["label"]?.ToString() ?? "");
        }

        var writeColumns = new List<string>();
        foreach (object column in Values(config, "write_columns"))
        {
            writeColumns.Add(column?.ToString() ?? "");
        }

        // colspan/rowspan are not exposed by Dom, so the grouped head row is raw.
        var groupRow = new List<string>
        {
            "<tr>",
            "<th class=\"label-col\" rowspan=\"2\">Food group</th>",
            "<th class=\"life-col\" rowspan=\"2\">Shelf life</th>",
            $"<th class=\"group-col\" colspan=\"{households.Count}\">People</th>",
        };

        foreach (string column in writeColumns)
        {
            string cellClass = column == "Buy" ? "buy-col" : "blank-col";
            groupRow.Add($"<th class=\"{cellClass}\" rowspan=\"2\">{column}</th>");
        }

        groupRow.Add("</tr>");

        var subRow = new List<string> { "<tr>" };
        foreach (string label in households)
        {
            subRow.Add($"<th class=\"qty-col\">{label}</th>");
        }

        subRow.Add("</tr>");

        var bodyRows = new List<string>();

        foreach (object entry in Values(config, "groups"))
        {
            if (entry is not LuaTable group)
                continue;

            string unit = group["unit"]?.ToString() ?? "";
            double perPerson = Convert.ToDouble(group["per_person"] ?? 0);

            var cells = new List<string>
            {
                Dom.Td("label-col", group["name"]?.ToString() ?? ""),
                Dom.Td("life-col", group["life"]?.ToString() ?? ""),
            };

            foreach (object size in Values(config, "households"))
            {
                int people = size is LuaTable household
                    ? Convert.ToInt32(household["people"] ?? 1)
                    : 1;

                cells.Add(Dom.Td("qty-col", FormatQty(perPerson * people, unit)));
            }

            foreach (string column in writeColumns)
            {
                cells.Add(column == "Buy" ? Dom.Td("buy-col", "") : Dom.Td("blank-col", ""));
            }

            bodyRows.Add(Dom.Tr("", cells.ToArray()));
        }

        var notes = new List<string>();
        foreach (object note in Values(config, "stock_notes"))
        {
            notes.Add(Dom.Li("", note?.ToString() ?? ""));
        }

        return Dom.Section(
            "stock-section",
            SectionHead("01", Read(config, "stock_title"), Read(config, "stock_aside")),
            Dom.Table(
                "stock-table",
                Dom.Thead(
                    "",
                    string.Concat(groupRow),
                    string.Concat(subRow)
                ),
                Dom.Tbody("", bodyRows.ToArray())
            ),
            Dom.Ul("stock-notes", notes.ToArray())
        );
    }

    private static string BuildRotation(LuaTable config)
    {
        var columns = new List<string>();
        foreach (object column in Values(config, "rotation_columns"))
        {
            columns.Add(column?.ToString() ?? "");
        }

        var headCells = new List<string>
        {
            Dom.Th("label-col", "Staple"),
            Dom.Th("life-col", "Shelf life"),
        };
        foreach (string column in columns)
        {
            headCells.Add(Dom.Th(column == "Replaced" ? "buy-col" : "blank-col", column));
        }

        var bodyRows = new List<string>();

        foreach (object entry in Values(config, "rotation_rows"))
        {
            if (entry is not LuaTable row)
                continue;

            var cells = new List<string>
            {
                Dom.Td("label-col", row["name"]?.ToString() ?? ""),
                Dom.Td("life-col", row["life"]?.ToString() ?? ""),
            };

            foreach (string column in columns)
            {
                cells.Add(column == "Replaced" ? Dom.Td("buy-col", "") : Dom.Td("blank-col", ""));
            }

            bodyRows.Add(Dom.Tr("", cells.ToArray()));
        }

        return Dom.Section(
            "rotation-section",
            SectionHead("02", Read(config, "rotation_title"), Read(config, "rotation_aside")),
            Dom.Table(
                "rotation-table",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", bodyRows.ToArray())
            )
        );
    }

    private static string BuildRoutine(LuaTable config)
    {
        var steps = new List<string>();
        int index = 1;

        foreach (object entry in Values(config, "routine_steps"))
        {
            if (entry is not LuaTable step)
                continue;

            steps.Add(
                Dom.Div(
                    "routine-step",
                    Dom.Tag("span", "routine-index", index.ToString("00")),
                    Dom.Tag("span", "routine-label", step["label"]?.ToString() ?? ""),
                    Dom.Tag("span", "routine-text", step["text"]?.ToString() ?? "")
                )
            );

            index++;
        }

        return Dom.Section(
            "routine-section",
            SectionHead("03", Read(config, "routine_title"), Read(config, "routine_aside")),
            Dom.Div("routine-grid", steps.ToArray())
        );
    }

    private static string SectionHead(string number, string title, string aside)
    {
        return Dom.Div(
            "section-head",
            Dom.Tag("span", "section-num", number),
            Dom.H2("", title),
            string.IsNullOrEmpty(aside) ? "" : Dom.Tag("span", "section-aside", aside)
        );
    }

    private static string FormatQty(double value, string unit)
    {
        string number =
            Math.Abs(value - Math.Round(value)) < 0.001
                ? Math.Round(value).ToString("0", CultureInfo.InvariantCulture)
                : value.ToString("0.#", CultureInfo.InvariantCulture);

        return string.IsNullOrEmpty(unit) ? number : $"{number} {unit}";
    }

    private static string Read(LuaTable config, string key)
    {
        return config[key]?.ToString() ?? "";
    }

    private static System.Collections.ICollection Values(LuaTable config, string key)
    {
        return config[key] is LuaTable table ? table.Values : Array.Empty<object>();
    }
}
