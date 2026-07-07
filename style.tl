local M = {}
local contracts = require("contracts")

global ROOT_PATH: string

local function read_file(path: string): string
    local f = io.open(path, "r")
    if not f then return nil end
    local content = f:read("*a")
    f:close()
    return content
end

function M.Apply(html: string, config: contracts.Config): string
    local base = read_file(ROOT_PATH .. "/templates/base/style.css") or ""
    local themeFilename = (config.style or "style") .. ".css"

    local themePath = ROOT_PATH .. "/templates/" .. config.template .. "/" .. themeFilename
    local theme = ""

    local content = read_file(themePath)
    if content then
        theme = content
    end

    if config.overrides then
        local overridesPath = ROOT_PATH .. "/templates/" .. config.template .. "/" .. config.overrides .. ".css"
        local overridesContent = read_file(overridesPath)

        if overridesContent then
            theme = theme .. overridesContent
        end
    end
    
    local styleBlock = "<style>" .. base .. "</style><style>" .. theme .. "</style></head>"
    local start, finish = html:find("</head>", 1, true)

    if start then
        return html:sub(1, start - 1) .. styleBlock .. html:sub(finish + 1)
    end

    return html
end

return M
