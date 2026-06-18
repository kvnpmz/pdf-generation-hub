local utils = require("utils")

global function GenerateDocument(data: {any: any}): string
    local html = (data.renderTemplate as function(any): string)(data.config)

    html = string.format([[
        <!DOCTYPE html><html lang="en"><head><meta charset="utf-8"></head>
        <body>%s</body></html>
    ]], html)

    return utils.escape_html(html)
end 

return GenerateDocument
