local dom = require("dom")
local contracts = require("contracts")

local record Target
    label: string
    value: string
end

local record Food
    name: string
    mg: string
end

local record Column
    title: string
    caption: string
    items: {Food}
end

local record Swap
    from: string
    to: string
end

local record Tally
    label: string
    cells: {string}
    target: string
end

local record Config
    header: string
    badge: string
    subtitle: string
    targets: {Target}
    columns: {Column}
    swaps: {Swap}
    tally: Tally
    note: string
end

local M = {}

local function build_header(config: Config): string
    return dom.header(
        "guide-head",
        dom.h1("", config.header),
        dom.tag("span", "guide-badge", config.badge)
    ) .. dom.tag("p", "guide-subtitle", config.subtitle)
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

    return dom.section("targets", dom.div("target-grid", table.unpack(cells)))
end

local function build_columns(config: Config): string
    local classMap = { "left", "middle", "right" }
    local columnNodes: {string} = {}

    for i, column in ipairs(config.columns) do
        local rows: {string} = {}

        for _, food in ipairs(column.items) do
            table.insert(
                rows,
                dom.li(
                    "food",
                    dom.tag("span", "food-name", food.name),
                    dom.tag("span", "food-mg", food.mg)
                )
            )
        end

        table.insert(
            columnNodes,
            dom.div(
                "column " .. (classMap[i] or "default"),
                dom.h2("", column.title),
                dom.tag("span", "column-caption", column.caption),
                dom.ul("food-list", table.unpack(rows))
            )
        )
    end

    return dom.main("", table.unpack(columnNodes))
end

local function build_swaps(config: Config): string
    local cells: {string} = {}

    for _, swap in ipairs(config.swaps) do
        table.insert(
            cells,
            dom.div(
                "swap",
                dom.tag("span", "swap-from", swap.from),
                dom.tag("span", "swap-arrow", "→"),
                dom.tag("span", "swap-to", swap.to)
            )
        )
    end

    return dom.section(
        "swaps",
        dom.tag("span", "swaps-label", "Smart swaps"),
        dom.div("swap-grid", table.unpack(cells))
    )
end

local function build_tally(config: Config): string
    local tally = config.tally
    local cells: {string} = {}

    for _, label in ipairs(tally.cells) do
        table.insert(
            cells,
            dom.div(
                "tally-cell",
                dom.tag("span", "tally-name", label),
                dom.tag("span", "tally-line", "")
            )
        )
    end

    table.insert(
        cells,
        dom.div(
            "tally-cell",
            dom.tag("span", "tally-name", "Total"),
            dom.tag("span", "tally-line", ""),
            dom.tag("span", "tally-target", tally.target)
        )
    )

    return dom.section(
        "tally",
        dom.tag("span", "tally-label", tally.label),
        dom.div("tally-grid", table.unpack(cells))
    )
end

function M.render(config: Config): string
    return build_header(config)
        .. build_targets(config)
        .. build_columns(config)
        .. build_swaps(config)
        .. build_tally(config)
        .. dom.tag("p", "guide-note", config.note)
end

return M as contracts.TemplateRender
