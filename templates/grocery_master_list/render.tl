local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    items: {string}
    staples: {string}
end

local record Config
    header: string
    store_label: string
    date_label: string
    legend_tick: string
    legend_star: string
    columns: {{Section}}
end

local M = {}

local function staple_lookup(section: Section): {string:boolean}
    local lookup: {string:boolean} = {}

    for _, name in ipairs(section.staples or {}) do
        lookup[name] = true
    end

    return lookup
end

local function build_field(label: string, extraClass: string): string
    return dom.tag(
        "span",
        "field " .. extraClass,
        dom.tag("span", "field-label", label),
        dom.tag("span", "field-rule", "")
    )
end

local function build_header(config: Config): string
    local legend = dom.tag(
        "span",
        "legend",
        dom.tag("span", "legend-box", ""),
        dom.tag("span", "legend-text", config.legend_tick),
        dom.tag("span", "legend-star", "★"),
        dom.tag("span", "legend-text", config.legend_star)
    )

    return dom.header(
        "list-head",
        dom.h1("", config.header),
        dom.div(
            "head-line",
            build_field(config.store_label, "field-store"),
            legend,
            build_field(config.date_label, "field-date")
        )
    )
end

local function build_item(name: string, isStaple: boolean): string
    local rowClass = isStaple and "item staple" or "item"
    local cells = { dom.tag("span", "item-name", name) }

    if isStaple then
        table.insert(cells, dom.tag("span", "item-star", "★"))
    end

    return dom.li(rowClass, table.unpack(cells))
end

local function build_section(section: Section): string
    local lookup = staple_lookup(section)
    local rows: {string} = {}

    for _, item in ipairs(section.items) do
        table.insert(rows, build_item(item, lookup[item] == true))
    end

    -- one write-in row per section for anything the list does not cover
    table.insert(rows, dom.li("blank-line", ""))

    return dom.section("", dom.h2("", section.title), dom.ul("", table.unpack(rows)))
end

local function build_columns(config: Config): string
    local columns: {string} = {}

    for _, columnItems in ipairs(config.columns) do
        local sectionHtml: {string} = {}

        for _, section in ipairs(columnItems) do
            table.insert(sectionHtml, build_section(section))
        end

        table.insert(columns, dom.div("column", table.unpack(sectionHtml)))
    end

    return dom.main("list-grid", table.unpack(columns))
end

local function build_footer(): string
    return dom.tag(
        "footer",
        "totals",
        build_field("Est. budget", "total-field"),
        build_field("Total spent", "total-field"),
        build_field("Notes", "total-field total-field-wide")
    )
end

function M.render(config: Config): string
    return build_header(config) .. build_columns(config) .. build_footer()
end

return M as contracts.TemplateRender
