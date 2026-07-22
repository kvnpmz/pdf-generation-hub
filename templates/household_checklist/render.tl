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

local function renderSection(section: Section, index: number): string
    local itemsHtml: {string} = {}

    for _, item in ipairs(section.items) do
        table.insert(itemsHtml, dom.li("", item))
    end

    return dom.section("",
        dom.h3("", tostring(index) .. ". " .. section.title),
        dom.ul("", table.unpack(itemsHtml))
    )
end

function M.render(config: Config): string
    local columns: {string} = {}
    local sectionNumber = 0

    for _, column in ipairs(config.columns) do
        local sections: {string} = {}

        for _, section in ipairs(column) do
            sectionNumber = sectionNumber + 1
            table.insert(sections, renderSection(section, sectionNumber))
        end

        table.insert(columns, dom.div("column", table.unpack(sections)))
    end

    local words = {}

    for word in config.header:gmatch("%S+") do
        table.insert(words, word)
    end

    return table.concat({
        dom.header("", dom.h1("", string.format("%s %s&nbsp", words[1], words[2])), dom.h2( "", words[3])),
        table.unpack(columns)
    })
end

return M as contracts.TemplateRender
