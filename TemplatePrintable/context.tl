local M = {}
local contracts = require("contracts")

function M.Execute(docId: string): (contracts.Config, any)
    local configPath = "documents." .. docId .. ".config"
    local config = require(configPath) as contracts.Config
    
    local templatePath = "templates." .. config.template .. ".render"
    local templateModule = require(templatePath)

    local module = templateModule as contracts.RenderStrategy
    local rawRender = module.render as function(any): string

    local renderStrategy = function(data: any): string
        return rawRender(data)
    end

    return config, renderStrategy
end

return M
