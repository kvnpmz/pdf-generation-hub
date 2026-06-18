local M = {}
local utils = require("utils")

function M.Apply(html: string, config: string): string
    local baseCss = utils.escape_css(io.open("templates/base/style.css", "r"):read("*a"))
    
    local styleFilename = (config.style or "style") .. ".css"
    local customPath = "templates/" .. config.template .. "styleFilename"

    local customCss = ""
    local f = io.open(customPath, "r")
    if f then
        customCss = utils.escape_css(f:read("*a"))
        f:close()
    end

    local styleBlock = "<style>" .. baseCss .. "</style><style>" .. customCss .. "</style></head>"

    local start, finish = html:find("</head>", 1, true)

    if start then
        return html:sub(1, start - 1) .. styleBlock .. html:sub(finish + 1)
    end

    return html
end

return M
