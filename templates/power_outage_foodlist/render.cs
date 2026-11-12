using System;
using System.Collections.Generic;
using NLua;

public class PowerOutageFoodList : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>
        {
            BuildHeader(config),
            Dom.MainTag(
                "",
                BuildEatFirst(config),
                BuildFoodList(config),
                BuildMenu(config),
                BuildStockUp(config)
            ),
        };

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName =
                config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "power_outage_foodlist",
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
                    Dom.Tag("span", "field-label", Escape(label?.ToString() ?? "")),
                    Dom.Tag("span", "field-line", "")
                )
            );
        }

        return Dom.Tag(
            "header",
            "masthead",
            Dom.Div(
                "masthead-main",
                Dom.H1("", Escape(Read(config, "header_line1"))),
                Dom.H1("line-two", Escape(Read(config, "header_line2"))),
                Dom.Tag("p", "subtitle", Escape(Read(config, "subtitle")))
            ),
            Dom.Tag(
                "div",
                "plan-card",
                Dom.Tag("span", "plan-card-title", Escape(Read(config, "plan_card_title"))),
                string.Concat(fields)
            )
        );
    }

    private static string BuildEatFirst(LuaTable config)
    {
        var cards = new List<string>();

        foreach (object entry in Values(config, "eat_first"))
        {
            if (entry is not LuaTable card)
                continue;

            var items = new List<string>();
            foreach (object item in Values(card, "items"))
            {
                items.Add(Dom.Li("", Escape(item?.ToString() ?? "")));
            }

            cards.Add(
                Dom.Div(
                    "eat-card",
                    Dom.Tag("span", "eat-window", Escape(card["window"]?.ToString() ?? "")),
                    Dom.Tag("span", "eat-label", Escape(card["label"]?.ToString() ?? "")),
                    Dom.Ul("eat-list", items.ToArray()),
                    Dom.Tag("span", "eat-note", Escape(card["note"]?.ToString() ?? ""))
                )
            );
        }

        return Dom.Section(
            "eat-section",
            SectionHead(Read(config, "eat_first_title"), Read(config, "eat_first_aside")),
            Dom.Div("eat-grid", cards.ToArray())
        );
    }

    private static string BuildFoodList(LuaTable config)
    {
        var columns = new List<string>();

        foreach (object entry in Values(config, "food_groups"))
        {
            if (entry is not LuaTable group)
                continue;

            var items = new List<string>();
            foreach (object item in Values(group, "items"))
            {
                items.Add(Dom.Li("", Escape(item?.ToString() ?? "")));
            }

            columns.Add(
                Dom.Div(
                    "food-col",
                    Dom.H3("", Escape(group["title"]?.ToString() ?? "")),
                    Dom.Ul("check-list", items.ToArray())
                )
            );
        }

        return Dom.Section(
            "list-section",
            SectionHead(Read(config, "list_title"), Read(config, "list_aside")),
            Dom.Div("food-grid", columns.ToArray())
        );
    }

    private static string BuildMenu(LuaTable config)
    {
        var headCells = new List<string> { Dom.Th("meal-col", "Meal") };

        foreach (object day in Values(config, "menu_days"))
        {
            headCells.Add(Dom.Th("", Escape(day?.ToString() ?? "")));
        }

        var bodyRows = new List<string>();

        foreach (object entry in Values(config, "menu_rows"))
        {
            if (entry is not LuaTable row)
                continue;

            var cells = new List<string> { Dom.Th("meal-col", Escape(row["meal"]?.ToString() ?? "")) };

            foreach (object cell in Values(row, "cells"))
            {
                cells.Add(Dom.Td("", Escape(cell?.ToString() ?? "")));
            }

            bodyRows.Add(Dom.Tr("", cells.ToArray()));
        }

        return Dom.Section(
            "menu-section",
            SectionHead(Read(config, "menu_title"), Read(config, "menu_aside")),
            Dom.Table(
                "menu-grid",
                Dom.Thead("", Dom.Tr("", headCells.ToArray())),
                Dom.Tbody("", bodyRows.ToArray())
            )
        );
    }

    private static string BuildStockUp(LuaTable config)
    {
        var targets = new List<string>
        {
            Dom.Tag("span", "target-title", Escape(Read(config, "targets_title"))),
        };

        foreach (object entry in Values(config, "targets"))
        {
            if (entry is not LuaTable target)
                continue;

            targets.Add(
                Dom.Div(
                    "target-row",
                    Dom.Tag("span", "target-label", Escape(target["label"]?.ToString() ?? "")),
                    Dom.Tag("span", "target-value", Escape(target["value"]?.ToString() ?? ""))
                )
            );
        }

        var columns = new List<string>();

        foreach (object entry in Values(config, "shopping"))
        {
            if (entry is not LuaTable group)
                continue;

            var items = new List<string>();
            foreach (object item in Values(group, "items"))
            {
                items.Add(Dom.Li("", Escape(item?.ToString() ?? "")));
            }

            columns.Add(
                Dom.Div(
                    "shop-col",
                    Dom.H3("", Escape(group["title"]?.ToString() ?? "")),
                    Dom.Ul("check-list", items.ToArray())
                )
            );
        }

        return Dom.Section(
            "stock-section",
            SectionHead(Read(config, "stock_title"), Read(config, "stock_aside")),
            Dom.Div(
                "stock-body",
                Dom.Div("target-card", targets.ToArray()),
                Dom.Div("shop-grid", columns.ToArray())
            )
        );
    }

    private static string SectionHead(string title, string aside)
    {
        return Dom.Div(
            "section-head",
            Dom.H2("", Escape(title)),
            string.IsNullOrEmpty(aside)
                ? ""
                : Dom.Tag("span", "section-aside", Escape(aside))
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

    private static string Escape(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }
}
