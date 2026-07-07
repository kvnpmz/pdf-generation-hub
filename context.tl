local M = {}
local contracts = require("contracts")

function M.Execute(docId: string): contracts.RenderContext
    local config = require("documents." .. docId .. ".config") as contracts.Config
    local template = require("templates." .. config.template .. ".render") as contracts.TemplateRender

    return {
        config = config,
        render = template.render
    }
end

return M
