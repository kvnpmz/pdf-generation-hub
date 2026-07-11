using NLua;

namespace TemplatePrintable.Core;

public interface IRenderer
{
    RenderResult Render(LuaTable config);
}
