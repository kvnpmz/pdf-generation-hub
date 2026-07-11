local M = {}

local context = require("context")
local style = require("style")

local record RenderResult
    html: string
    outputName: string
end

function M.Render(documentId: string): RenderResult
    local renderContext = context.Execute(documentId)

    local config = renderContext.config
    local renderTemplate = renderContext.render

    local html = renderTemplate(config)

    html = string.format(
        [[<!DOCTYPE html><html lang="en"><head><meta charset="utf-8"></head><body>%s</body></html>]],
        html
    )

    html = style.Apply(html, config)

    return {
        html = html,
        outputName = config.output_name or config.id or "output"
    }
end

return M
