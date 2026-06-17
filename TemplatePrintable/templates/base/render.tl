local type Config = record
    id: string
    output_name: string
    template: string
    header: string
end

local type Data = record
    config: Config
    renderTemplate: function(any): string
end

local utils = require("utils") -- Ensure utils is available

global function GenerateDocument(data: Data): string
    local html = data.renderTemplate(data.config)

    html = string.format([[
        <!DOCTYPE html><html lang="en"><head><meta charset="utf-8"></head>
        <body>%s</body></html>
    ]], html)

    return utils.escape_html(html)
end 

return GenerateDocument
