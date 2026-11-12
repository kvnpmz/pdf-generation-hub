using System;
using System.Collections.Generic;
using NLua;

public class KidneyLabEssential : IRenderer
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
                BuildSymptoms(config)
            ),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "kidney_lab_essential",
        };
    }

    private static string BuildHeader(LuaTable config)
    {
        var fields = new List<string>();

        foreach (object label in Values(config, "record_fields"))
        {
            fields.Add(
                Dom.Div(
                    "record-field",
                    Dom.Tag("span", "record-label", label?.ToString() ?? ""),
                    Dom.Tag("span", "record-line", "")
                )
            );
        }

        return Dom.Tag(
            "header",
            "ess-head",
            Dom.Div(
                "head-main",
                Dom.H1("", Read(config, "title")),
                Dom.Tag("p", "subtitle", Read(config, "subtitle"))
            ),
            Dom.Div(
                "head-record",
                Dom.Div("record-fields", fields.ToArray()),
                Dom.Tag("span", "head-note", Read(config, "header_note"))
            )
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

        return Dom.Div("target-grid", cells.ToArray());
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

        int rowCount = Convert.ToInt32(config["row_count"] ?? 13);

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
                "ess-table",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", bodyRows.ToArray())
            ),
            Dom.Tag("p", "log-note", Read(config, "log_note"))
        );
    }

    private static string BuildSymptoms(LuaTable config)
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

        return Dom.Section(
            "symptom-section",
            SectionHead(Read(config, "symptom_title"), Read(config, "symptom_aside")),
            Dom.Div("symptom-grid", symptoms.ToArray())
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
