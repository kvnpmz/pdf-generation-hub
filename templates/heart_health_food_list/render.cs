using System.Collections.Generic;
using NLua;

public class HeartHealthFoodList : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var htmlParts = new List<string>();

        string header = config["header"]?.ToString() ?? "";
        string[] words = header.Split(
            new[] { ' ', '\t', '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        string group1 = words.Length > 0 ? words[0] : "";
        if (words.Length > 1) group1 += " " + words[1];

        string group2 = words.Length > 2 ? words[2] : "";
        if (words.Length > 3) group2 += " " + words[3];

        htmlParts.Add(
            Dom.Header(
                "",
                Dom.H1("", group1),
                Dom.H1("", group2)
            )
        );

        var categorySections = new List<string>();
        var categories = (LuaTable)config["categories"];

        foreach (LuaTable category in categories.Values)
        {
            string categoryName = category["name"]?.ToString() ?? "";
            var foodCards = new List<string>();

            var items = (LuaTable)category["items"];
            foreach (LuaTable item in items.Values)
            {
                string itemName = item["name"]?.ToString() ?? "";
                string itemDetail = item["detail"]?.ToString() ?? "";

                var cardElements = new List<string> { Dom.H3("", itemName) };
                
                if (!string.IsNullOrEmpty(itemDetail))
                {
                    cardElements.Add(Dom.Div("food-detail", itemDetail));
                }

                string card = Dom.Div("food-card", cardElements.ToArray());
                foodCards.Add(card);
            }

            string grid = Dom.Div("food-grid", foodCards.ToArray());
            string section = Dom.Section(
                "category-section",
                Dom.H2("category-title", categoryName),
                grid
            );

            categorySections.Add(section);
        }

        string mainContent = Dom.MainTag("", categorySections.ToArray());
        string footerContent = config["footer"]?.ToString() ?? "";
        string footer = Dom.Div("footer", footerContent);

        htmlParts.Add(mainContent);
        htmlParts.Add(footer);

        return new RenderResult
        {
            Html = string.Concat(htmlParts),
            OutputName = config["output_name"]?.ToString() ?? config["id"]?.ToString() ?? "output",
        };
    }
}
