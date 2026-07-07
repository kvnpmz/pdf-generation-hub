local dom = require("dom")
local contracts = require("contracts")

local record Section
   title: string
   items: {string}
   sections: {Section} 
end

local record Column
   title: string
   sections: {Section}
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

        for _, section in ipairs(column.sections) do
            local itemHtml = {}

            for _, item in ipairs(section.items) do
                table.insert(itemHtml, dom.li("", item))
            end

            table.insert(sectionHtml, dom.tag("h4", "", section.title))
            table.insert(sectionHtml, dom.ul("", table.unpack(itemHtml)))
        end

        table.insert(
            columnHtml,
            dom.div(
                "column", 
                dom.h3(cssClass, columnTitle), 
                dom.div("section-container " .. cssClass, table.unpack(sectionHtml))
            )
        )
    end

    local words = {}

    for word in config.header:gmatch("%S+") do
        table.insert(words, word)
    end

    local headerHtml = dom.header(
        "", 
        dom.h1("", string.format("%s %s %s", words[1], words[2], words[3])),
        dom.h2("", string.format("%s %s", words[4], words[5]))
    )

    local html = headerHtml .. dom.main("", table.concat(columnHtml))
    return html
end

return M as contracts.TemplateRender
