using System;
using System.Collections.Generic;
using NLua;

public class KidneyLabMinimal : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag("", BuildLog(config)),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "kidney_lab_minimal",
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
            "min-head",
            Dom.H1("", Read(config, "title")),
            Dom.Div("record-row", fields.ToArray())
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

        int rowCount = Convert.ToInt32(config["row_count"] ?? 14);

        var headCells = new List<string>();
        foreach (var (label, cssClass) in columns)
        {
            headCells.Add(Dom.Th(cssClass, label));
        }

        var bodyRows = new List<string>();

        for (int row = 0; row < rowCount; row++)
        {
            var cells = new List<string>();

            foreach (var (_, cssClass) in columns)
            {
                cells.Add(Dom.Td(cssClass, ""));
            }

            bodyRows.Add(Dom.Tr("", cells.ToArray()));
        }

        return Dom.Table(
            "min-table",
            Dom.Thead("", Dom.Tr("", headCells.ToArray())),
            Dom.Tbody("", bodyRows.ToArray())
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
