public static class Dom
{
    public static string Tag(string name, string className, params string[] children)
    {
        var attributes = string.IsNullOrEmpty(className)
            ? ""
            : $" class=\"{className}\"";

        var content = string.Concat(children);

        return string.IsNullOrEmpty(attributes)
            ? $"<{name}>{content}</{name}>"
            : $"\n<{name}{attributes}>{content}</{name}>";
    }

    public static string Header(string c, params string[] x) => Tag("header", c, x);
    public static string MainTag(string c, params string[] x) => Tag("main", c, x);
    public static string Section(string c, params string[] x) => Tag("section", c, x);
    public static string H1(string c, params string[] x) => Tag("h1", c, x);
    public static string H2(string c, params string[] x) => Tag("h2", c, x);
    public static string H3(string c, params string[] x) => Tag("h3", c, x);
    public static string Ul(string c, params string[] x) => Tag("ul", c, x);
    public static string Li(string c, params string[] x) => Tag("li", c, x);
    public static string Table(string c, params string[] x) => Tag("table", c, x);
    public static string Thead(string c, params string[] x) => Tag("thead", c, x);
    public static string Tbody(string c, params string[] x) => Tag("tbody", c, x);
    public static string Th(string c, params string[] x) => Tag("th", c, x);
    public static string Tr(string c, params string[] x) => Tag("tr", c, x);
    public static string Td(string c, params string[] x) => Tag("td", c, x);
    public static string Div(string c, params string[] x) => Tag("div", c, x);
}
