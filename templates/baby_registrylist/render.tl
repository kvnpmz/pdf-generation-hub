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
    local columnsHtml = {}

    for _, column in ipairs(config.columns) do
        local sectionsHtml = {}
        
        for _, section in ipairs(column) do
            local itemsHtml = {}
            
            for _, item in ipairs(section.items) do
                table.insert(itemsHtml, dom.li("", item))
            end

            if section.title == "Other" then
                for _ = 1, 8 do
                    table.insert(itemsHtml, dom.li("blank-line", ""))
                end
            end
            
            table.insert(sectionsHtml, dom.section("", 
                dom.h2("", section.title), dom.ul("", table.concat(itemsHtml))))
        end
        
        table.insert(columnsHtml, dom.div("column", table.concat(sectionsHtml)))
    end

    local bodyHtml = {}
    table.insert(bodyHtml, dom.h1("", config.header))

    table.insert(bodyHtml, table.concat(columnsHtml))
    local html = table.concat(bodyHtml)

    return html 
end

return M as contracts.TemplateRender
