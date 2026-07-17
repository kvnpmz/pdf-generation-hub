using System.Collections.Generic;
using NLua;

public class FolicAcidFoodList : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var htmlParts = new List<string>();

        string header = config["header"]?.ToString() ?? "";
        string[] words = header.Split(
            new[] { ' ', '\t', '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        htmlParts.Add(
            Dom.Header(
                "",
                Dom.H1("", $"{words[0]} {words[1]} {words[2]}"),
                Dom.H1("gray", $"{words[3]} {words[4]}")
            )
        );

        var columnsHtml = new List<string>();

        var columns = (LuaTable)config["columns"];

        foreach (LuaTable column in columns.Values)
        {
            var columnContent = new List<string>();

            foreach (LuaTable category in column.Values)
            {
                var categoryContent = new List<string>();

                var title = category["title"]?.ToString() ?? "";

                categoryContent.Add(Dom.H2("", title));

                var sections = (LuaTable)category["sections"];

                foreach (LuaTable section in sections.Values)
                {
                    var subtitle = section["subtitle"]?.ToString() ?? "";

                    var itemHtml = new List<string>();

                    var items = (LuaTable)section["items"];

                    foreach (var item in items.Values)
                    {
                        itemHtml.Add(Dom.Li("", item?.ToString() ?? ""));
                    }

                    categoryContent.Add(
                        Dom.Section("", Dom.H3("", subtitle), Dom.Ul("", itemHtml.ToArray()))
                    );
                }

                columnContent.Add(Dom.Div("category", categoryContent.ToArray()));
            }

            columnsHtml.Add(Dom.Div("column", columnContent.ToArray()));
        }

        htmlParts.Add(string.Concat(columnsHtml));

        return new RenderResult
        {
            Html = string.Concat(htmlParts),
            OutputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output",
        };
    }
}
