using NLua;

public class Context
{
    public string DocumentId { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;

    public string OutputName { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;

    public required string BaseProjectName { get; set; }
    public LuaTable? Config { get; set; }

    public Flow? Flow { get; set; }
    public bool EnableImages { get; set; }
    public bool IsEditable { get; set; }
}
