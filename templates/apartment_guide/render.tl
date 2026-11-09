local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    tier: string
    items: {string}
end

local record Config
    header: string
    subtitle: string
    plan_card_title: string
    plan_card_fields: {string}
    columns: {{Section}}
    first_night_title: string
    first_night_items: {string}
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

    return dom.header(
        "apartment-head",
        dom.div(
            "head-main",
            dom.h1("", config.header),
            dom.tag("p", "subtitle", config.subtitle)
        ),
        dom.div(
            "plan-card",
            dom.tag("span", "plan-card-title", config.plan_card_title),
            table.unpack(fields)
        )
    )
end

local function build_column(sections: {Section}): string
    local nodes: {string} = {}
    local divider_used = false

    for _, section in ipairs(sections) do
        if section.tier == "extra" and not divider_used then
            table.insert(
                nodes,
                dom.div(
                    "tier-divider",
                    dom.tag("span", "", "Nice to have")
                )
            )
            divider_used = true
        end

        local listItems: {string} = {}

        for _, item in ipairs(section.items) do
            table.insert(listItems, dom.li("", item))
        end

        table.insert(
            nodes,
            dom.section(
                "block " .. (section.tier or "essential"),
                dom.h2("", section.title),
                dom.ul("", table.unpack(listItems))
            )
        )
    end

    return dom.div("column", table.unpack(nodes))
end

local function build_first_night(config: Config): string
    local items: {string} = {}

    for _, item in ipairs(config.first_night_items) do
        table.insert(items, dom.li("", item))
    end

    return dom.section(
        "first-night",
        dom.div("first-night-title", config.first_night_title),
        dom.ul("first-night-list", table.unpack(items))
    )
end

function M.render(config: Config): string
    local columns: {string} = {}

    for _, sections in ipairs(config.columns) do
        table.insert(columns, build_column(sections))
    end

    return build_header(config)
        .. dom.main("", table.unpack(columns))
        .. build_first_night(config)
end

return M as contracts.TemplateRender
