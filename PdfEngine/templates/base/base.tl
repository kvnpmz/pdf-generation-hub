local type Layout = record
    id: string
    output_name: string
    template: string
    header: string
end

local type Data = record
    layout: Layout
    renderBody: function(any): string
end

global function GenerateDocument(data: Data): string
    local layout = data.layout
    local html = data.renderBody(layout)

    return string.format([[
        <!DOCTYPE html>
        <html lang="en">
        <head>
        <meta charset="utf-8">
        </head>
        <body><h1>hi</h1>%s</body>
        </html>
    ]], html)
end

return GenerateDocument
