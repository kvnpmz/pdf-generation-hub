local dom = require("dom")
local contracts = require("contracts")

local record GuideRow
    length: string
    tops: string
    bottoms: string
    socks: string
    laundry: string
end

local record Guide
    title: string
    columns: {string}
    rows: {GuideRow}
    note: string
end

local record Group
    sub: string
    items: {string}
end

local record Section
    title: string
    items: {string}
    groups: {Group}
end

local record Limit
    value: string
    label: string
end

local record Config
    header: string
    subtitle: string
    guide: Guide
    columns: {{Section}}
    limits: {Limit}
end

local M = {}

local function renderGuide(guide: Guide): string
    local headCells = {}
    for _, label in ipairs(guide.columns) do
        table.insert(headCells, dom.th("", label))
    end

    local rows = {}
    for _, row in ipairs(guide.rows) do
        table.insert(rows, dom.tr("",
            dom.td("trip", row.length),
            dom.td("", row.tops),
            dom.td("", row.bottoms),
            dom.td("", row.socks),
            dom.td("laundry", row.laundry)
        ))
    end

    return dom.div("guide",
        dom.h2("", guide.title),
        dom.table("guide-table",
            dom.thead("", dom.tr("", table.unpack(headCells))),
            dom.tbody("", table.unpack(rows))
        ),
        dom.div("guide-note", guide.note)
    )
end

local function renderItem(item: string): string
    return dom.div("item",
        dom.div("checkbox"),
        dom.div("qty-line"),
        dom.div("label", item)
    )
end

local function renderSection(section: Section): string
    local nodes = {}

    if section.groups then
        for _, group in ipairs(section.groups) do
            table.insert(nodes, dom.div("sub-header", group.sub))
            for _, item in ipairs(group.items or {}) do
                table.insert(nodes, renderItem(item))
            end
        end
    else
        for _, item in ipairs(section.items or {}) do
            table.insert(nodes, renderItem(item))
        end
    end

    table.insert(nodes, 1, dom.h2("", section.title))
    return dom.section("", table.unpack(nodes))
end

function M.render(config: Config): string
    local parts = {}

    table.insert(parts, dom.header("",
        dom.h1("", config.header),
        dom.div("subtitle", config.subtitle)
    ))

    table.insert(parts, renderGuide(config.guide))

    local columns = {}
    for _, columnSections in ipairs(config.columns) do
        local nodes = {}
        for _, section in ipairs(columnSections) do
            table.insert(nodes, renderSection(section))
        end
        table.insert(columns, dom.div("column", table.unpack(nodes)))
    end

    table.insert(parts, dom.main("", table.unpack(columns)))

    local limitCells = {}
    for _, limit in ipairs(config.limits) do
        table.insert(limitCells, dom.div("limit",
            dom.div("limit-value", limit.value),
            dom.div("limit-label", limit.label)
        ))
    end
    table.insert(parts, dom.div("limits", table.unpack(limitCells)))

    return table.concat(parts)
end

return M as contracts.TemplateRender
