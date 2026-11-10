using System;
using System.Collections.Generic;
using NLua;

public class DormRoomLayout : IRenderer
{
    private const int GridColumns = 12;
    private const int GridRows = 14;

    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildMeasure(config),
                BuildPlan(config),
                BuildBottom(config),
                BuildRecheck(config)
            ),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "dorm_room_layout",
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
                "head-main",
                Dom.Tag("span", "kicker", Read(config, "kicker")),
                Dom.H1("", Read(config, "header")),
                Dom.Tag("p", "subtitle", Read(config, "subtitle"))
            ),
            Dom.Div("plan-card", card.ToArray())
        );
    }

    private static string BuildMeasure(LuaTable config)
    {
        var fields = new List<string>();

        foreach (object entry in Values(config, "measure_fields"))
        {
            if (entry is not LuaTable field)
                continue;

            string unit = field["unit"]?.ToString() ?? "";

            fields.Add(
                Dom.Div(
                    "measure-field",
                    Dom.Tag(
                        "span",
                        "measure-label",
                        field["label"]?.ToString() ?? "",
                        string.IsNullOrEmpty(unit) ? "" : Dom.Tag("em", "unit", unit)
                    ),
                    Dom.Tag("span", "measure-line", "")
                )
            );
        }

        return Dom.Section(
            "measure-section",
            SectionHead(Read(config, "measure_title"), Read(config, "measure_aside")),
            Dom.Div("measure-band", fields.ToArray())
        );
    }

    private static string BuildPlan(LuaTable config)
    {
        return Dom.Section(
            "plan-section",
            SectionHead(Read(config, "plan_title"), Read(config, "plan_aside")),
            Dom.Div(
                "plan-body",
                BuildGrid(),
                BuildSide(config)
            )
        );
    }

    private static string BuildGrid()
    {
        var xLabels = new List<string>();
        for (int i = 0; i < GridColumns; i++)
        {
            xLabels.Add(Dom.Tag("span", "", i.ToString()));
        }

        var yLabels = new List<string>();
        for (int i = 0; i < GridRows; i++)
        {
            yLabels.Add(Dom.Tag("span", "", i.ToString()));
        }

        var cells = new List<string>();

        for (int row = 0; row < GridRows; row++)
        {
            for (int column = 0; column < GridColumns; column++)
            {
                string cellClass = "cell";

                if (column == 3 || column == 7)
                    cellClass += " major-x";

                if (row == 4 || row == 9)
                    cellClass += " major-y";

                cells.Add(Dom.Tag("span", cellClass, ""));
            }
        }

        return Dom.Div(
            "grid-shell",
            Dom.Tag("span", "ruler-corner", "ft"),
            Dom.Div("ruler-x", xLabels.ToArray()),
            Dom.Div("ruler-y", yLabels.ToArray()),
            Dom.Div("room-grid", cells.ToArray())
        );
    }

    private static string BuildSide(LuaTable config)
    {
        var parts = new List<string>
        {
            Dom.H3("side-title", Read(config, "furniture_title")),
            BuildFootprints(config),
            Dom.Tag("p", "side-note", Read(config, "furniture_note")),
            Dom.H3("side-title", Read(config, "features_title")),
            BuildChecklist(config, "features"),
            Dom.H3("side-title", Read(config, "clearance_title")),
            BuildClearances(config),
            Dom.H3("side-title", Read(config, "buy_title")),
            BuildChecklist(config, "buy_items"),
            Dom.H3("side-title", Read(config, "notes_title")),
            Dom.Div(
                "wall-notes",
                Dom.Tag("span", "rule-line", ""),
                Dom.Tag("span", "rule-line", ""),
                Dom.Tag("span", "rule-line", "")
            ),
        };

        return Dom.Tag("aside", "plan-side", parts.ToArray());
    }

    private static string BuildFootprints(LuaTable config)
    {
        var rows = new List<string>();

        foreach (object entry in Values(config, "furniture"))
        {
            if (entry is not LuaTable piece)
                continue;

            rows.Add(
                Dom.Tr(
                    "",
                    Dom.Td("piece", piece["name"]?.ToString() ?? ""),
                    Dom.Td("dim", piece["size"]?.ToString() ?? ""),
                    Dom.Td("sq", piece["squares"]?.ToString() ?? "")
                )
            );
        }

        return Dom.Table(
            "footprint-table",
            Dom.Thead(
                "",
                Dom.Tr(
                    "",
                    Dom.Th("piece", "Piece"),
                    Dom.Th("dim", "Inches"),
                    Dom.Th("sq", "Squares")
                )
            ),
            Dom.Tbody("", rows.ToArray())
        );
    }

    private static string BuildChecklist(LuaTable config, string key)
    {
        var items = new List<string>();

        foreach (object item in Values(config, key))
        {
            items.Add(Dom.Tag("span", "check-item", item?.ToString() ?? ""));
        }

        return Dom.Div("feature-list", items.ToArray());
    }

    private static string BuildClearances(LuaTable config)
    {
        var rows = new List<string>();

        foreach (object entry in Values(config, "clearances"))
        {
            if (entry is not LuaTable rule)
                continue;

            rows.Add(
                Dom.Div(
                    "clearance-row",
                    Dom.Tag("span", "clearance-badge", rule["size"]?.ToString() ?? ""),
                    Dom.Tag("span", "clearance-text", rule["text"]?.ToString() ?? "")
                )
            );
        }

        return Dom.Div("clearance-list", rows.ToArray());
    }

    private static string BuildBottom(LuaTable config)
    {
        var fitItems = new List<string>();

        foreach (object item in Values(config, "fit_checks"))
        {
            fitItems.Add(Dom.Tag("span", "check-item fit-item", item?.ToString() ?? ""));
        }

        var fit = Dom.Section(
            "fit-section",
            SectionHead(Read(config, "fit_title"), Read(config, "fit_aside")),
            Dom.Div("fit-grid", fitItems.ToArray()),
            Dom.Tag("p", "fit-note", Read(config, "fit_note"))
        );

        return Dom.Div(
            "bottom-row",
            fit,
            Dom.Section(
                "split-section",
                SectionHead(Read(config, "split_title"), Read(config, "split_aside")),
                BuildSplit(config)
            )
        );
    }

    private static string BuildRecheck(LuaTable config)
    {
        var items = new List<string>();

        foreach (object item in Values(config, "recheck_items"))
        {
            items.Add(Dom.Tag("span", "check-item", item?.ToString() ?? ""));
        }

        return Dom.Section(
            "recheck-section",
            SectionHead(Read(config, "recheck_title"), Read(config, "recheck_aside")),
            Dom.Div(
                "recheck-band",
                Dom.Div("recheck-list", items.ToArray()),
                Dom.Tag("span", "recheck-note", Read(config, "recheck_note"))
            )
        );
    }

    private static string BuildSplit(LuaTable config)
    {
        var headCells = new List<string>
        {
            Dom.Th("item-col", Read(config, "split_first_column")),
        };

        foreach (object column in Values(config, "split_columns"))
        {
            headCells.Add(Dom.Th("mark", column?.ToString() ?? ""));
        }

        var rows = new List<string>();

        foreach (object entry in Values(config, "split_rows"))
        {
            var cells = new List<string>
            {
                Dom.Td("item-col", entry?.ToString() ?? ""),
            };

            foreach (object column in Values(config, "split_columns"))
            {
                cells.Add(Dom.Td("mark", ""));
            }

            rows.Add(Dom.Tr("", cells.ToArray()));
        }

        return Dom.Table(
            "split-table",
            Dom.Thead("", Dom.Tr("", headCells.ToArray())),
            Dom.Tbody("", rows.ToArray())
        );
    }

    private static string SectionHead(string title, string aside)
    {
        return Dom.Div(
            "section-head",
            Dom.H2("", title),
            string.IsNullOrEmpty(aside) ? "" : Dom.Tag("span", "section-aside", aside)
        );
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
