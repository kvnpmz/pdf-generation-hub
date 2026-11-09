local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    items: {string}
end

local record Column
    title: string
    kicker: string
    sections: {Section}
end

local record Config
    header: string
    subtitle: string
    footer: string
    columns: {Column}
end

local M = {}

local function render_section(section: Section): string
    local listItems: {string} = {}

    for _, item in ipairs(section.items) do
        table.insert(listItems, dom.li("", item))
    end

    return dom.section(
        "block",
        dom.tag("h3", "block-title", section.title),
        dom.ul("items", table.unpack(listItems))
    )
end

function M.render(config: Config): string
    local headerHtml = dom.header(
        "",
        dom.h1("", config.header),
        dom.tag("p", "subtitle", config.subtitle)
    )

    local columnHtml: {string} = {}
    local classMap = { "have", "low", "out" }

    for columnIndex, column in ipairs(config.columns) do
        local sections: {string} = {}

        for _, section in ipairs(column.sections) do
            table.insert(sections, render_section(section))
        end

        table.insert(
            columnHtml,
            dom.div(
                "column " .. classMap[columnIndex],
                dom.div(
                    "column-head",
                    dom.tag("span", "kicker", column.kicker),
                    dom.h2("", column.title)
                ),
                dom.div("column-body", table.unpack(sections))
            )
        )
    end

    local footerHtml = dom.tag("footer", "page-footer",
        dom.tag("span", "footer-text", config.footer),
        dom.tag("span", "footer-mark", "tpr")
    )

    return headerHtml .. dom.main("board", table.unpack(columnHtml)) .. footerHtml
end

return M as contracts.TemplateRender
