local type Config = record
    id: string
    output_name: string
    template: string
    header: string
end

local type Data = record
    layout: Config
    renderTemplate: function(any): string
end

global function GenerateDocument(data: Data): string
    local html = data.renderTemplate(data.config)

    return string.format([[
        <!DOCTYPE html><html lang="en"><head><meta charset="utf-8"></head><body>%s</body></html>
    ]], html)
end

return GenerateDocument
