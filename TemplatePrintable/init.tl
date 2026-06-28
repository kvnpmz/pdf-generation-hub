local render = require("templates.base.render")
local context = require("context")
local style_provider = require("style_provider")

local record RenderResult
    html: string
    output_name: string
end

local function Render(document_id: string): RenderResult
    local config, render_template = context.Execute(document_id)

    local html = render.GenerateDocument({
        config = config,
        renderTemplate = render_template,
    })

    html = style_provider.Apply(html, config)

    return {
        html = html,
        output_name = config.output_name or config.id or "output"
    }
end

return {
    Render = Render
}
