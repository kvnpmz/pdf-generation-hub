local contracts = require("contracts")
local type Config = contracts.Config
local utils = require("utils")

global function GenerateDocument(data: Data): string
    local html = data.renderTemplate(data.config)

    html = string.format([[
        <!DOCTYPE html><html lang="en"><head><meta charset="utf-8"></head>
        <body>%s</body></html>
    ]], html)

    return utils.escape_html(html)
end 

return GenerateDocument
