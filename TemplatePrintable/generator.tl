local M = {}

local render = require("templates.base.render")
local context = require("context")
local styleProvider = require("style_provider")

local record RenderResult
    html: string
    outputName: string
end

function M.Render(documentId: string): RenderResult
    local renderContext = context.Execute(documentId)

    local config = renderContext.config
    local renderTemplate = renderContext.render

    local html = render.GenerateDocument({
        config = config,
        renderTemplate = renderTemplate,
    })

    html = styleProvider.Apply(html, config)

    return {
        html = html,
        outputName = config.output_name or config.id or "output"
    }
end

return M
