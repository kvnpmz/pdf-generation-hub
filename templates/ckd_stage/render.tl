local dom = require("dom")
local contracts = require("contracts")

local record Target
    label: string
    value: string
end

local record Column
    title: string
    items: {string}
end

local record Config
    header: string
    badge: string
    targets: {Target}
    target_note: string
    columns: {Column}
end

local M = {}

local function build_header(config: Config): string
    return dom.header(
        "stage-head",
        dom.h1("", config.header),
        dom.tag("span", "stage-badge", config.badge)
    )
end

local function build_targets(config: Config): string
    local cells: {string} = {}

    for _, target in ipairs(config.targets) do
        table.insert(
            cells,
            dom.div(
                "target",
                dom.tag("span", "target-label", target.label),
                dom.tag("span", "target-value", target.value)
            )
        )
    end

    return dom.section(
        "targets",
        dom.div("target-grid", table.unpack(cells)),
        dom.tag("p", "target-note", config.target_note)
    )
end

local function build_columns(config: Config): string
    local classMap = { "left", "middle", "right" }
    local columnNodes: {string} = {}

    for i, column in ipairs(config.columns) do
        local listItems: {string} = {}

        for _, item in ipairs(column.items) do
            table.insert(listItems, dom.li("", item))
        end

        table.insert(
            columnNodes,
            dom.div(
                "column " .. (classMap[i] or "default"),
                dom.h2("", column.title),
                dom.ul("", table.unpack(listItems))
            )
        )
    end

    return dom.main("", table.unpack(columnNodes))
end

function M.render(config: Config): string
    return build_header(config) .. build_targets(config) .. build_columns(config)
end

return M as contracts.TemplateRender
