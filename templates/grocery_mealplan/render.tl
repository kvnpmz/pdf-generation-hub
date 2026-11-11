local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    week_label: string
    servings_label: string
    legend_tick: string
    plan_title: string
    plan_note: string
    list_title: string
    list_note: string
    days: {string}
    meals: {string}
    columns: {{Section}}
    footer_budget: string
    footer_spent: string
    footer_notes: string
end

local M = {}

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
        dom.tag("span", "legend-text", config.legend_tick)
    )

    return dom.header(
        "sheet-head",
        dom.h1("", config.header),
        dom.div(
            "head-line",
            build_field(config.week_label, "field-week"),
            legend,
            build_field(config.servings_label, "field-servings")
        )
    )
end

local function build_plan(config: Config): string
    local headCells: {string} = { dom.th("day-col", "") }

    for _, meal in ipairs(config.meals) do
        table.insert(headCells, dom.th("meal-col", meal))
    end

    local rows: {string} = {}

    for _, day in ipairs(config.days) do
        local cells: {string} = { dom.th("day-cell", day) }

        for _ = 1, #config.meals do
            table.insert(cells, dom.td("meal-cell", ""))
        end

        table.insert(rows, dom.tr("", table.unpack(cells)))
    end

    local tableHtml = dom.table(
        "plan-grid",
        dom.thead("", dom.tr("", table.unpack(headCells))),
        dom.tbody("", table.unpack(rows))
    )

    return dom.section(
        "plan-block",
        dom.div(
            "block-head",
            dom.h2("", config.plan_title),
            dom.tag("span", "block-note", config.plan_note)
        ),
        tableHtml
    )
end

local function build_section(section: Section): string
    local rows: {string} = {}

    for _, item in ipairs(section.items) do
        table.insert(
            rows,
            dom.li(
                "item",
                dom.tag("span", "item-name", item),
                dom.tag("span", "item-qty", "")
            )
        )
    end

    -- two write-in rows for anything the list does not cover
    table.insert(rows, dom.li("blank-line", ""))
    table.insert(rows, dom.li("blank-line", ""))

    return dom.section("shop-section", dom.h2("", section.title), dom.ul("", table.unpack(rows)))
end

local function build_list(config: Config): string
    local columns: {string} = {}

    for _, columnSections in ipairs(config.columns) do
        local sectionHtml: {string} = {}

        for _, section in ipairs(columnSections) do
            table.insert(sectionHtml, build_section(section))
        end

        table.insert(columns, dom.div("shop-col", table.unpack(sectionHtml)))
    end

    return dom.section(
        "list-block",
        dom.div(
            "block-head",
            dom.h2("", config.list_title),
            dom.tag("span", "block-note", config.list_note)
        ),
        dom.div("shop-grid", table.unpack(columns))
    )
end

local function build_footer(config: Config): string
    return dom.tag(
        "footer",
        "totals",
        build_field(config.footer_budget, "total-field"),
        build_field(config.footer_spent, "total-field"),
        build_field(config.footer_notes, "total-field total-field-wide")
    )
end

function M.render(config: Config): string
    return build_header(config)
        .. dom.main("worksheet", build_plan(config), build_list(config))
        .. build_footer(config)
end

return M as contracts.TemplateRender
