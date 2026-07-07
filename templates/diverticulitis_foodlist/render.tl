local dom = require("dom")
local contracts = require("contracts")

local record Column
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {Column}
end

local M = {}

function M.render(config: Config): string
    local classMap = { "left", "middle", "right" }
    local columnHtml = {}

    for i, column in ipairs(config.columns) do
        local cssClass = classMap[i]
        local columnTitle = column.title 

        local sectionHtml = {}
        local itemHtml = {}

        for _, item in ipairs(column.items) do
            table.insert(itemHtml, dom.li("", item))
        end

        table.insert(sectionHtml, dom.ul(cssClass, table.unpack(itemHtml)))

        table.insert(
            columnHtml,
            dom.div(
                "column", 
                dom.tag("h4", cssClass, columnTitle), 
                table.unpack(sectionHtml)
            )
        )
    end

    local words = {}

    for word in config.header:gmatch("%S+") do
        table.insert(words, word)
    end

    local headerHtml = dom.header(
        "", 
        dom.h1(
            "", 
            words[1]
        ),
        dom.div("", 
            dom.h2(
                "",
                words[2] .. " "
            ), 
            dom.h3(
                "", 
                string.format("%s %s", words[3], words[4])
            )
        )
    )

    local html = headerHtml .. dom.main("", table.unpack(columnHtml))
    return html
end

return M as contracts.TemplateRender
