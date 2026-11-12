using System;
using System.Collections.Generic;
using NLua;

public class KidneyLabTracker : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildTargets(config),
                BuildLog(config),
                BuildBetweenVisits(config)
            ),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "kidney_lab_tracker",
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
                Dom.H1("", Read(config, "header_line1")),
                Dom.H1("line-two", Read(config, "header_line2")),
                Dom.Tag("p", "subtitle", Read(config, "subtitle")),
                Dom.Tag("p", "header-note", Read(config, "header_note"))
            ),
            Dom.Div("plan-card", card.ToArray())
        );
    }

    private static string BuildTargets(LuaTable config)
    {
        var cells = new List<string>();

        foreach (object entry in Values(config, "targets"))
        {
            if (entry is not LuaTable target)
                continue;

            cells.Add(
                Dom.Div(
                    "target",
                    Dom.Tag("span", "target-label", target["label"]?.ToString() ?? ""),
                    Dom.Tag("span", "target-value", target["value"]?.ToString() ?? "")
                )
            );
        }

        return Dom.Section(
            "targets",
            Dom.Div("target-grid", cells.ToArray()),
            Dom.Tag("p", "target-note", Read(config, "target_note"))
        );
    }

    private static string BuildLog(LuaTable config)
    {
        var columns = new List<(string Label, string Class)>();
        foreach (object entry in Values(config, "columns"))
        {
            if (entry is not LuaTable column)
                continue;

            columns.Add(
                (
                    column["label"]?.ToString() ?? "",
                    column["class"]?.ToString() ?? "col-blank"
                )
            );
        }

        int rowCount = Convert.ToInt32(config["row_count"] ?? 12);

        var headCells = new List<string> { Dom.Th("col-num", "#") };
        foreach (var (label, cssClass) in columns)
        {
            headCells.Add(Dom.Th(cssClass, label));
        }

        var bodyRows = new List<string>();

        for (int row = 1; row <= rowCount; row++)
        {
            var cells = new List<string> { Dom.Td("col-num", row.ToString()) };

            foreach (var (_, cssClass) in columns)
            {
                cells.Add(Dom.Td(cssClass, ""));
            }

            bodyRows.Add(Dom.Tr("", cells.ToArray()));
        }

        return Dom.Section(
            "log-section",
            SectionHead(Read(config, "log_title"), Read(config, "log_aside")),
            Dom.Table(
                "log-table",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", bodyRows.ToArray())
            ),
            Dom.Tag("p", "log-note", Read(config, "log_note"))
        );
    }

    private static string BuildBetweenVisits(LuaTable config)
    {
        var symptoms = new List<string>();

        foreach (object entry in Values(config, "symptoms"))
        {
            string? symptom = entry?.ToString();
            if (string.IsNullOrEmpty(symptom))
                continue;

            symptoms.Add(
                Dom.Div(
                    "symptom",
                    Dom.Tag("span", "check", ""),
                    Dom.Tag("span", "symptom-text", symptom)
                )
            );
        }

        var asks = new List<string>();

        foreach (object entry in Values(config, "questions"))
        {
            string? question = entry?.ToString();
            if (string.IsNullOrEmpty(question))
                continue;

            asks.Add(
                Dom.Div(
                    "ask-row",
                    Dom.Tag("span", "ask-text", question),
                    Dom.Tag("span", "ask-line", "")
                )
            );
        }

        var symptomPanel = Dom.Div(
            "panel",
            Dom.Div(
                "panel-head",
                Dom.H3("", Read(config, "symptom_title")),
                Dom.Tag("span", "panel-aside", Read(config, "symptom_aside"))
            ),
            Dom.Div("symptom-grid", symptoms.ToArray())
        );

        var askPanel = Dom.Div(
            "panel",
            Dom.Div(
                "panel-head",
                Dom.H3("", Read(config, "ask_title")),
                Dom.Tag("span", "panel-aside", Read(config, "ask_aside"))
            ),
            Dom.Div("ask-list", asks.ToArray())
        );

        return Dom.Section(
            "between-section",
            SectionHead(Read(config, "between_title"), Read(config, "between_aside")),
            Dom.Div("panel-grid", symptomPanel, askPanel)
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
