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
    local first, rest = config.header:match("^(%S+)%s*(.*)$")

    local headerHtml = dom.h1("", string.format("%s ", first), 
        dom.div("", rest)
    )
    local renderedColumns = {}

    for columnIndex, columnSections in ipairs(config.columns) do
        local columnNodes = {}
        local columnTitle = (columnIndex == 1) and "Foods To Eat Freely" or "Foods To Limit and Avoid"
        local themeClass = (columnIndex == 1) and "green-theme" or "red-theme"
        local sectionContainer = {}

        for _, section in ipairs(columnSections) do
            local listItems = {}
            for _, item in ipairs(section.items) do
                table.insert(listItems, dom.li("", item))
            end

            local listClass = ""
            if #listItems >= 6 then
                listClass = "three-column-list"
            elseif #listItems >= 4 then
                listClass = "two-column-list"
            end

            local sectionNode = dom.section(themeClass,
                dom.h3(themeClass, section.title),
                dom.ul(listClass, table.unpack(listItems))
            )
            table.insert(sectionContainer, sectionNode)
        end

        table.insert(columnNodes, dom.h2(themeClass, columnTitle))
        table.insert(columnNodes, dom.div("section-container", table.unpack(sectionContainer)))
        table.insert(renderedColumns, dom.div("column", table.unpack(columnNodes)))
    end

    return headerHtml .. dom.main("", table.unpack(renderedColumns))
end

return M as contracts.LayoutProcessor
