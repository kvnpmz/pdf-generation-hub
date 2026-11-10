local dom = require("dom")
local contracts = require("contracts")

local record SwapRow
    food: string
    serving: string
    potassium: string
    swap: string
end

local record Group
    name: string
    note: string
    rows: {SwapRow}
end

local record Rule
    label: string
    text: string
end

local record Config
    kicker: string
    header: string
    header_second_line: string
    subtitle: string
    card_title: string
    card_fields: {string}
    columns: {string}
    groups: {Group}
    rules: {Rule}
end

local M = {}

local function build_header(config: Config): string
    local fields: {string} = {}

    for _, label in ipairs(config.card_fields or {}) do
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
        dom.tag("span", "plan-card-title", config.card_title),
        table.unpack(fields)
    )

    local main = dom.div(
        "head-main",
        dom.tag("span", "kicker", config.kicker),
        dom.h1(
            "",
            config.header,
            dom.tag("span", "h1-sub", config.header_second_line)
        ),
        dom.tag("span", "subtitle", config.subtitle)
    )

    return dom.header("swap-head", main, card)
end

local function build_column_head(config: Config): string
    local cells: {string} = {}

    for _, label in ipairs(config.columns or {}) do
        table.insert(cells, dom.th("", label))
    end

    return dom.table(
        "swap-table col-head",
        dom.thead("", dom.tr("", table.unpack(cells)))
    )
end

local function build_group(group: Group): string
    local rows: {string} = {}

    for _, row in ipairs(group.rows or {}) do
        table.insert(
            rows,
            dom.tr(
                "",
                dom.td("food", row.food),
                dom.td("serving", row.serving),
                dom.td("potassium", row.potassium),
                dom.td("swap", row.swap)
            )
        )
    end

    local table_html = dom.table(
        "swap-table",
        dom.tbody("", table.unpack(rows))
    )

    return dom.section(
        "group",
        dom.div(
            "group-head",
            dom.tag("span", "group-name", group.name),
            dom.tag("span", "group-note", group.note)
        ),
        table_html
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
    local groups: {string} = {}

    for _, group in ipairs(config.groups or {}) do
        table.insert(groups, build_group(group))
    end

    return build_header(config)
        .. dom.main(
            "",
            build_column_head(config),
            dom.div("groups", table.unpack(groups)),
            build_rules(config)
        )
end

return M as contracts.TemplateRender
