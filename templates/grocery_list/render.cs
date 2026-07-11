using System.Collections.Generic;
using TemplatePrintable.Core;
using NLua;

namespace GroceryListTemplate;

public class GroceryListRender : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var headerHtml = Dom.H1("", config["header"]?.ToString() ?? "");

        var columnsHtml = new List<string>();

        var columns = (LuaTable)config["columns"];

        foreach (LuaTable columnItems in columns.Values)
        {
            var columnContent = new List<string>();

            foreach (LuaTable section in columnItems.Values)
            {
                var itemsHtml = new List<string>();

                foreach (var item in ((LuaTable)section["items"]).Values)
                {
                    itemsHtml.Add(Dom.Li("", item?.ToString() ?? ""));
                }

                itemsHtml.Add(Dom.Li("blank-line", "yh1ello"));
                itemsHtml.Add(Dom.Li("blank-line", "&nbsp;"));

                columnContent.Add(
                        Dom.Section("",
                            Dom.H2("", section["title"]?.ToString() ?? ""),
                            Dom.Ul("", itemsHtml.ToArray())
                            )
                        );
            }

            columnsHtml.Add(
                    Dom.Div("column", columnContent.ToArray())
                    );
        }

        columnsHtml.Insert(0, headerHtml);

        return new RenderResult
        {
            Html = string.Concat(columnsHtml),
            OutputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output"
        };
    }
}
