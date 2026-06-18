local record TemplateModule
   render: function(any): string
end

local M = {}
require("types")
local utils = require("utils")

local function sanitize_unicode(data: any): any
    if type(data) == "string" then
        return utils.escape_html(data as string)
    elseif type(data) == "table" then
        local t = {}
        for k, v in pairs(data as {any:any}) do
            t[k] = sanitize_unicode(v)
        end
        return t
    end
    return data
end

function M.Execute(docId: string): (Config, any)
    local configPath = "documents." .. docId .. ".config"
    local config = require(configPath) as Config
    
    local templatePath = "templates." .. config.template .. ".render"
    local module = require(templatePath) as RenderStrategy
    local rawRender = module.render

    local renderStrategy = function(data: any): string
        return rawRender(sanitize_unicode(data))
    end

    return config, renderStrategy
end

return M
