local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {{Section}}
end

local contracts = require("contracts")
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

return M as contracts.RenderStrategy

--function render_packing_list(data)
--    local html = '<h1>Travel Packing List</h1><div class="container">'
--    
--    for _, col in ipairs(data) do
--        html = html .. '<div class="column">'
--        for _, section in ipairs(col) do
--            html = html .. '<div class="section">'
--            html = html .. '<div class="header-row"><div class="header-qty">QTY</div><div class="header-title">' .. section.title .. '</div></div>'
--            
--            -- Handle sections with groups (like Clothes)
--            if section.groups then
--                for _, group in ipairs(section.groups) do
--                    html = html .. '<div class="sub-header">' .. group.sub .. '</div>'
--                    for _, item in ipairs(group.items) do
--                        html = html .. '<div class="item-row"><div class="checkbox"></div><div class="qty-line"></div><div class="label">' .. item .. '</div></div>'
--                    end
--                end
--            else
--                -- Handle standard lists
--                for _, item in ipairs(section.items) do
--                    html = html .. '<div class="item-row"><div class="checkbox"></div><div class="qty-line"></div><div class="label">' .. item .. '</div></div>'
--                end
--            end
--            html = html .. '</div>'
--        end
--        html = html .. '</div>'
--    end
--    
--    html = html .. '</div>'
--    return html
--end
