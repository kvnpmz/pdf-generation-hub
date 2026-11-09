local dom = require("dom")
local contracts = require("contracts")

local record Item
    name: string
    note: string
    level: string
end

local record Aisle
    name: string
    tag: string
    items: {Item}
end

local record Config
    header: string
    badge: string
    subtitle: string
    aisles: {Aisle}
    footer: string
end

local M = {}

local function build_header(config: Config): string
    return dom.header(
        "grocery-head",
        dom.tag("span", "eyebrow", config.badge .. " · renal shopping guide"),
        dom.h1("", config.header),
        dom.tag("p", "grocery-subtitle", config.subtitle)
    )
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
        dom.tag("span", "legend-title", "How to shop"),
        dom.div(
            "legend-pills",
            legend_pill("go", "Enjoy freely"),
            legend_pill("care", "Half portion"),
            legend_pill("no", "Skip or swap")
        ),
        dom.tag("span", "legend-key", "POTASSIUM · PHOSPHORUS · SODIUM CHECKED")
    )
end

local function build_aisle(index: number, aisle: Aisle): string
    local rows: {string} = {}

    for _, item in ipairs(aisle.items) do
        local note = ""
        if item.note ~= "" then
            note = dom.tag("span", "item-note", item.note)
        end

        table.insert(
            rows,
            dom.li(
                "item " .. item.level,
                dom.tag("span", "check", ""),
                dom.tag("span", "item-name", item.name),
                note
            )
        )
    end

    return dom.section(
        "aisle",
        dom.div(
            "aisle-head",
            dom.tag("span", "aisle-number", string.format("%02d", index)),
            dom.h2("", aisle.name),
            dom.tag("span", "aisle-tag", aisle.tag)
        ),
        dom.ul("item-list", table.unpack(rows))
    )
end

local function build_aisles(config: Config): string
    local cards: {string} = {}

    for index, aisle in ipairs(config.aisles) do
        table.insert(cards, build_aisle(index, aisle))
    end

    return dom.main("aisle-grid", table.unpack(cards))
end

function M.render(config: Config): string
    return build_header(config)
        .. build_legend()
        .. build_aisles(config)
        .. dom.tag(
            "p",
            "grocery-tip",
            dom.tag("span", "tip-label", "TIP"),
            config.footer
        )
end

return M as contracts.TemplateRender
