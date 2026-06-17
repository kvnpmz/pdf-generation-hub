global record Config
    id: string
    output_name: string
    template: string
    header: string
end

local type RenderFunction = function(any): string

local M = {}
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

function M.Compose(docId: string): (Config, any)
    local configPath = "documents." .. docId .. ".config"
    print("documents." .. docId .. ".config")
    local config = require(configPath) as Config
    
    local templatePath = "templates." .. config.template .. ".render"
    local rawRender = require(templatePath) as RenderFunction

    local renderStrategy = function(data: any): string
        return rawRender(sanitize_unicode(data))
    end
    
    return config, renderStrategy
end

return M
