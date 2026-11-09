local dom = require("dom")
local contracts = require("contracts")

local record Item
    name: string
    level: string
end

local record Aisle
    name: string
    items: {Item}
end

local record Config
    header: string
    aisles: {Aisle}
end

local M = {}

local function build_header(config: Config): string
    return dom.header("grocery-head", dom.h1("", config.header))
end

local function legend_pill(level: string, label: string): string
    return dom.tag(
        "span",
        "legend-pill",
        dom.tag("span", "legend-dot " .. level, ""),
        dom.tag("span", "legend-label", label)
    )
end

local function build_legend(): string
    return dom.section(
        "legend",
        dom.div(
            "legend-pills",
            legend_pill("go", "Enjoy freely"),
            legend_pill("care", "Small portion"),
            legend_pill("no", "Skip or swap")
        )
    )
end

local function build_aisle(aisle: Aisle): string
    local rows: {string} = {}

    for _, item in ipairs(aisle.items) do
        table.insert(
            rows,
            dom.li(
                "item " .. item.level,
                dom.tag("span", "check", ""),
                dom.tag("span", "item-name", item.name)
            )
        )
    end

    return dom.section(
        "aisle",
        dom.div("aisle-head", dom.h2("", aisle.name)),
        dom.ul("item-list", table.unpack(rows))
    )
end

local function build_aisles(config: Config): string
    local cards: {string} = {}

    for _, aisle in ipairs(config.aisles) do
        table.insert(cards, build_aisle(aisle))
    end

    return dom.main("aisle-grid", table.unpack(cards))
end

function M.render(config: Config): string
    return build_header(config) .. build_legend() .. build_aisles(config)
end

return M as contracts.TemplateRender
