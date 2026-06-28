local M = {}
local contracts = require("contracts")

function M.Execute(docId: string): (contracts.Config, any)
    local configPath = "documents." .. docId .. ".config"
    local config = require(configPath) as contracts.Config
    
    local templatePath = "templates." .. config.template .. ".render"
    local templateModule = require(templatePath) as contracts.TemplateRender

    local templateRender = templateModule.render as function(any): string

    return {
        config = config,
        render = templateRender
    }
end

return M
