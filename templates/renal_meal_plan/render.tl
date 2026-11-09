local dom = require("dom")
local contracts = require("contracts")

local record Day
    name: string
    breakfast: string
    lunch: string
    dinner: string
    k: string
    na: string
    phos: string
    fluid: string
end

local record Target
    label: string
    value: string
end

local record ShopItem
    name: string
    qty: string
end

local record Aisle
    name: string
    items: {ShopItem}
end

local record Rule
    label: string
    text: string
end

local record Config
    header_line1: string
    header_line2: string
    kicker: string
    subtitle: string
    plan_card_title: string
    plan_card_fields: {string}
    targets: {Target}
    menu_title: string
    menu_note: string
    day_note: string
    days: {Day}
    shopping_title: string
    shopping_note: string
    aisles: {Aisle}
    rules: {Rule}
end

local M = {}

local function build_header(config: Config): string
    local fields: {string} = {}

    for _, label in ipairs(config.plan_card_fields) do
        table.insert(
            fields,
            dom.div(
                "plan-field",
                dom.tag("span", "field-label", label),
                dom.tag("span", "field-line", "")
            )
        )
    end

    local card = dom.div(
        "plan-card",
        dom.tag("span", "plan-card-title", config.plan_card_title),
        table.unpack(fields)
    )

    local main = dom.div(
        "head-main",
        dom.tag("span", "kicker", config.kicker),
        dom.h1("", config.header_line1, dom.tag("span", "head-line", config.header_line2)),
        dom.tag("span", "subtitle", config.subtitle)
    )

    return dom.header("grocery-head", main, card)
end

local function build_targets(config: Config): string
    local chips: {string} = {}

    for _, target in ipairs(config.targets or {}) do
        table.insert(
            chips,
            dom.div(
                "target",
                dom.tag("span", "target-label", target.label),
                dom.tag("span", "target-value", target.value)
            )
        )
    end

    return dom.section("target-strip", table.unpack(chips))
end

local function build_menu(config: Config): string
    local rows: {string} = {}

    for _, day in ipairs(config.days or {}) do
        table.insert(
            rows,
            dom.tr(
                "",
                dom.td("day", day.name),
                dom.td("", day.breakfast),
                dom.td("", day.lunch),
                dom.td("", day.dinner),
                dom.td("num", day.k),
                dom.td("num", day.na),
                dom.td("num", day.phos),
                dom.td("num", day.fluid)
            )
        )
    end

    local table_html = dom.table(
        "menu-table",
        dom.thead(
            "",
            dom.tr(
                "",
                dom.th("col-day", "Day"),
                dom.th("col-meal", "Breakfast"),
                dom.th("col-meal", "Lunch"),
                dom.th("col-meal", "Dinner"),
                dom.th("col-num", "K mg"),
                dom.th("col-num", "Na mg"),
                dom.th("col-num", "Phos mg"),
                dom.th("col-num", "Fluid oz")
            )
        ),
        dom.tbody("", table.unpack(rows))
    )

    return dom.section(
        "menu-block",
        dom.div(
            "block-head",
            dom.h2("", config.menu_title),
            dom.tag("span", "block-note", config.menu_note)
        ),
        table_html,
        dom.tag("span", "day-note", config.day_note)
    )
end

local function build_aisle(aisle: Aisle): string
    local items: {string} = {}

    for _, item in ipairs(aisle.items or {}) do
        table.insert(
            items,
            dom.li(
                "item",
                dom.tag("span", "check", ""),
                dom.tag("span", "item-name", item.name),
                dom.tag("span", "qty", item.qty)
            )
        )
    end

    return dom.section(
        "aisle",
        dom.h3("", aisle.name),
        dom.ul("item-list", table.unpack(items))
    )
end

local function build_shopping(config: Config): string
    local cards: {string} = {}

    for _, aisle in ipairs(config.aisles or {}) do
        table.insert(cards, build_aisle(aisle))
    end

    return dom.section(
        "shop-block",
        dom.div(
            "block-head",
            dom.h2("", config.shopping_title),
            dom.tag("span", "block-note", config.shopping_note)
        ),
        dom.div("aisle-grid", table.unpack(cards))
    )
end

local function build_rules(config: Config): string
    local rules: {string} = {}

    for _, rule in ipairs(config.rules or {}) do
        table.insert(
            rules,
            dom.div(
                "rule",
                dom.tag("span", "rule-label", rule.label),
                dom.tag("span", "rule-text", rule.text)
            )
        )
    end

    return dom.section("rules-strip", table.unpack(rules))
end

function M.render(config: Config): string
    return build_header(config)
        .. build_targets(config)
        .. dom.main(
            "",
            build_menu(config),
            build_shopping(config),
            build_rules(config)
        )
end

return M as contracts.TemplateRender
