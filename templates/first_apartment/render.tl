local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {{Section}}
end

local M = {}

function M.render(config: Config): string
    local headerHtml = dom.h1("", config.header)

    local columnsHtml = {}

    for _, columnItems in ipairs(config.columns) do
        local columnContent = {}

        for _, section in ipairs(columnItems) do
            local itemsHtml = {}

            for _, item in ipairs(section.items or {}) do
                table.insert(itemsHtml, dom.li("", item))
            end
            
            table.insert(columnContent, dom.section("", dom.h2("", section.title),
                dom.ul("", table.unpack(itemsHtml))))
        end
        table.insert(columnsHtml, dom.div("column", table.unpack(columnContent)))
    end

    table.insert(columnsHtml, 1, headerHtml)
    return table.concat(columnsHtml)
end

return M as contracts.TemplateRender
