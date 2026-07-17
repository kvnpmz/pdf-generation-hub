local M = {}

local contracts = require("contracts")
local context = require("context")
local style = require("style")

function M.Render(config: contracts.Config): contracts.RenderResult
    local renderContext = context.Execute(config)
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
