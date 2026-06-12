function _G.GenerateDocument(data)
    local layout = data.layout
    local html = data.renderBody(data.layout)

    return string.format([[
        <!DOCTYPE html>
        <html>
        <head>
        </head>
        <body>%s</body>
        </html>
    ]], html)
end

return GenerateDocument
