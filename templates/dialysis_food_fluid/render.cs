using System;
using System.Collections.Generic;
using NLua;

public class DialysisFoodFluid : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildTargets(config),
                BuildFoodColumns(config),
                BuildRules(config),
                BuildFluidLog(config)
            ),
            BuildFooter(config),
        };

        string outputName = Read(config, "output_name");

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = string.IsNullOrEmpty(outputName)
                ? "dialysis_food_fluid_guide"
                : outputName,
        };
    }

    private static string BuildHeader(LuaTable config)
    {
        return Dom.Header(
            "guide-head",
            Dom.H1("", Read(config, "header")),
            Dom.Tag("p", "subtitle", Read(config, "subtitle"))
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

    private static string BuildFoodColumns(LuaTable config)
    {
        var columns = new List<string>();

        foreach (object entry in Values(config, "columns"))
        {
            if (entry is not LuaTable column)
                continue;

            var items = new List<string>();
            foreach (object item in Values(column, "items"))
            {
                items.Add(Dom.Li("", item?.ToString() ?? ""));
            }

            columns.Add(
                Dom.Div(
                    "column " + (column["tone"]?.ToString() ?? ""),
                    Dom.H2("", column["title"]?.ToString() ?? ""),
                    Dom.Ul("", items.ToArray())
                )
            );
        }

        return Dom.Div("food-columns", columns.ToArray());
    }

    private static string BuildRules(LuaTable config)
    {
        var rules = new List<string>();

        foreach (object entry in Values(config, "rules"))
        {
            if (entry is not LuaTable rule)
                continue;

            rules.Add(
                Dom.Div(
                    "rule",
                    Dom.Tag("span", "rule-label", rule["label"]?.ToString() ?? ""),
                    Dom.Tag("span", "rule-text", rule["text"]?.ToString() ?? "")
                )
            );
        }

        return Dom.Section(
            "rules",
            Dom.Div(
                "section-head",
                Dom.H2("", Read(config, "rules_title"))
            ),
            Dom.Div("rule-grid", rules.ToArray())
        );
    }

    private static string BuildFluidLog(LuaTable config)
    {
        int cupsPerDay = Convert.ToInt32(config["cups_per_day"] ?? 6);
        var days = new List<string>();

        foreach (object entry in Values(config, "days"))
        {
            string? day = entry?.ToString();
            if (!string.IsNullOrEmpty(day))
                days.Add(day);
        }

        var dayNodes = new List<string>();

        foreach (string day in days)
        {
            var cups = new List<string>();
            for (int i = 0; i < cupsPerDay; i++)
            {
                cups.Add(Dom.Tag("span", "cup", ""));
            }

            dayNodes.Add(
                Dom.Div(
                    "day",
                    Dom.Tag("span", "day-name", day),
                    Dom.Div("cup-grid", cups.ToArray()),
                    Dom.Div(
                        "day-total",
                        Dom.Tag("span", "total-label", "Total"),
                        Dom.Tag("span", "total-line", "")
                    )
                )
            );
        }

        return Dom.Section(
            "fluid",
            Dom.Div(
                "section-head",
                Dom.H2("", Read(config, "fluid_title")),
                Dom.Tag("span", "section-aside", Read(config, "fluid_note"))
            ),
            Dom.Div("fluid-grid", dayNodes.ToArray())
        );
    }

    private static string BuildFooter(LuaTable config)
    {
        return Dom.Tag(
            "footer",
            "page-footer",
            Dom.Tag("span", "footer-note", Read(config, "footer_note")),
            Dom.Tag("span", "footer-mark", "Dialysis")
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
