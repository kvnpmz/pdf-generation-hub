local record Group
    sub: string
    items: {string}
end

local record Section
    title: string
    items: {string}
    groups: {Group}
end

local record Config
    header: string
    columns: {{Section}}
end

local contracts = require("contracts")
local M = {}

function M.render(config: Config): string
    local html = "<h1>" .. config.header .. "</h1><main>"
    
    for _, col in ipairs(config.columns) do
        html = html .. '<div class="column">'
        for _, section in ipairs(col) do
            html = html .. '<section>'
            html = html .. '<h2>' .. section.title .. '</h2>'
            
            if section.groups then
                for _, group in ipairs(section.groups or {}) do
                    html = html .. '<div class="sub-header">' .. group.sub .. '</div>'
                    for _, item in ipairs(group.items or {}) do
                        html = html .. '<div class="item"><div class="checkbox"></div><div class="qty-line"></div><div class="label">' .. item .. '</div></div>'
                    end
                end
            else
                for _, item in ipairs(section.items) do
                    html = html .. '<div class="item"><div class="checkbox"></div><div class="qty-line"></div><div class="label">' .. item .. '</div></div>'
                end
            end
            html = html .. '</section>'
        end
        html = html .. '</div>'
    end
    
    html = html .. '</main>'
    return html
end

return M as contracts.RenderStrategy
