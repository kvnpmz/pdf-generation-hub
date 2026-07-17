local M = {}
local contracts = require("contracts")

function M.Execute(config: contracts.Config): contracts.RenderContext
    local moduleName = "templates." .. config.template .. ".render"
    local template = require(moduleName) as contracts.TemplateRender

    return { 
        config = config, 
        render = template.render 
    }
end

return M
