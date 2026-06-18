local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {{Section}}
end

require("contracts")
local M = {}

function M.render(config: Config): string
    local html = "<h1>" .. config.header .. "</h1><main>"
    
    for _, column in ipairs(config.columns) do
        html = html .. '<div class="column">' -- Start column container
        for _, section in ipairs(column) do
            html = html .. "<section><h2>" .. section.title .. "</h2><ul>"
            for _, item in ipairs(section.items or {}) do
                html = html .. "<li>" .. item .. "</li>"
            end
            html = html .. "</ul></section>"
        end
        html = html .. "</div>" -- Close column container
    end
    
    html = html .. "</main>"
    return html
end

return M as RenderStrategy
