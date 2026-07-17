using System.Collections.Generic;
using NLua;

namespace CleaningPlanner;

public class TemplateRender : IRenderer
{
    private static readonly string[] MonthLetters = { "J", "F", "M", "A", "M", "J", "J", "A", "S", "O", "N", "D" };
    public RenderResult Render(LuaTable config)
    {
        var html = new List<string>();
        html.Add(Dom.Header("", Dom.H1("", config["header"]?.ToString() ?? "")));

        var sections = (LuaTable)config["sections"];

        foreach (var key in sections.Keys)
        {
            var areaName = key.ToString()!;
            var section = (LuaTable)sections[key];

            if (areaName.Contains("calendar"))
            {
                html.Add(Dom.Section(areaName, GenerateCalendar(areaName.Contains("month") ? 12 : 7)));
            }
            else if (areaName == "annually")
            {
                var items = (LuaTable)section["items"];
                var allItems = new List<string>();

                int index = 0;
                foreach (var item in items.Values)
                {
                    allItems.Add(Dom.Li("",
                                Dom.Div("month-label", MonthLetters[index]),
                                Dom.Div("item-text", item?.ToString() ?? "")
                                )
                            );
                    index++;
                }

                var col1 = new List<string>();
                var col2 = new List<string>();

                int midpoint = (allItems.Count + 1) / 2;
                for (int i = 0; i < allItems.Count; i++)
                {
                    if (i < midpoint)
                        col1.Add(allItems[i]);
                    else
                        col2.Add(allItems[i]);
                }

                html.Add(Dom.Section(areaName,
                            Dom.H2("", section["title"]?.ToString() ?? ""),
                            Dom.Div("annually-container",
                                Dom.Ul("annually-list", col1.ToArray()) +
                                Dom.Ul("annually-list", col2.ToArray())
                                )
                            ));
            }
            else
            {
                var itemsHtml = new List<string>();
                if (section["items"] is LuaTable items)
                {
                    foreach (var item in items.Values)
                    {
                        itemsHtml.Add(
                                Dom.Li("", item?.ToString() ?? "")
                                );
                    }
                }

                html.Add(Dom.Section(areaName,
                            Dom.H2("", section["title"]?.ToString() ?? ""),
                            Dom.Ul("", itemsHtml.ToArray())
                            ));
            }
        }

        return new RenderResult
        {
            Html = string.Concat(html),
            OutputName = config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "output"
        };
    }

    private string GenerateCalendar(int columns = 7)
    {
        var labels = "";
        if (columns == 12)
        {
            foreach (var month in MonthLetters)
            {
                labels += Dom.Div("day-label", month);
            }
        }
        else
        {
            var days = new[] { "S", "M", "T", "W", "T", "F", "S" };
            foreach (var day in days)
            {
                labels += Dom.Div("day-label", day);
            }
        }

        var cells = "";
        for (int i = 1; i <= (columns * 7); i++)
        {
            cells += Dom.Div("day-cell", "&nbsp");
        }

        return Dom.Div("calendar-container", labels + cells);
    }
}
