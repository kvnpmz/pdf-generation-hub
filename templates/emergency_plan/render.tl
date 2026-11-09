local dom = require("dom")
local contracts = require("contracts")

local record Household
    people: string
    water: string
    water7: string
    meals: string
    meals7: string
end

local record MenuRow
    meal: string
    cells: {string}
end

local record ShopColumn
    title: string
    items: {string}
end

local record PrepStep
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
    household: {Household}
    quantity_notes: {string}
    menu_days: {string}
    menu_rows: {MenuRow}
    shopping: {ShopColumn}
    prep_title: string
    prep_steps: {PrepStep}
    footer_note: string
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
        "masthead",
        dom.div(
            "masthead-main",
            dom.tag("span", "kicker", config.kicker),
            dom.h1("", config.header_line1),
            dom.h1("line-two", config.header_line2),
            dom.tag("p", "subtitle", config.subtitle)
        ),
        dom.div(
            "plan-card",
            dom.tag("span", "plan-card-title", config.plan_card_title),
            table.unpack(fields)
        )
    )
end

local function build_quantity_table(config: Config): string
    -- colspan is not exposed by dom, so the grouped header row is assembled directly
    local groupRowHtml = '<tr><th class="label-col corner" rowspan="2">People</th>'
        .. '<th class="group-col" colspan="2">Water</th>'
        .. '<th class="group-col" colspan="2">Food</th></tr>'
        .. '<tr><th class="sub-col">3 days</th><th class="sub-col alt">7 days</th>'
        .. '<th class="sub-col">3 days</th><th class="sub-col alt">7 days</th></tr>'

    local bodyRows: {string} = {}

    for _, row in ipairs(config.household) do
        table.insert(
            bodyRows,
            dom.tr(
                "",
                dom.td("label-col", row.people),
                dom.td("strong-col", row.water),
                dom.td("muted-col", row.water7),
                dom.td("strong-col", row.meals),
                dom.td("muted-col", row.meals7)
            )
        )
    end

    local notes: {string} = {}
    for _, note in ipairs(config.quantity_notes) do
        table.insert(notes, dom.li("", note))
    end

    return dom.section(
        "qty-section",
        dom.div(
            "section-head",
            dom.tag("span", "section-num", "01"),
            dom.h2("", "How much to store")
        ),
        dom.div(
            "qty-body",
            dom.table(
                "qty-table",
                dom.thead("", groupRowHtml),
                dom.tbody("", table.unpack(bodyRows))
            ),
            dom.ul("qty-notes", table.unpack(notes))
        )
    )
end

local function build_menu(config: Config): string
    local headCells: {string} = { dom.th("meal-col", "Meal") }

    for _, day in ipairs(config.menu_days) do
        table.insert(headCells, dom.th("", day))
    end

    local bodyRows: {string} = {}

    for _, row in ipairs(config.menu_rows) do
        local cells: {string} = { dom.th("meal-col", row.meal) }

        for _, cell in ipairs(row.cells) do
            table.insert(cells, dom.td("", cell))
        end

        table.insert(bodyRows, dom.tr("", table.unpack(cells)))
    end

    return dom.section(
        "menu-section",
        dom.div(
            "section-head",
            dom.tag("span", "section-num", "02"),
            dom.h2("", "Three days, no cooking"),
            dom.tag("span", "section-aside", "Shelf-stable only · eat from the tin")
        ),
        dom.table(
            "menu-grid",
            dom.thead("", dom.tr("", table.unpack(headCells))),
            dom.tbody("", table.unpack(bodyRows))
        )
    )
end

local function build_shopping(config: Config): string
    local columns: {string} = {}

    for _, column in ipairs(config.shopping) do
        local items: {string} = {}

        for _, item in ipairs(column.items) do
            table.insert(items, dom.li("", item))
        end

        table.insert(
            columns,
            dom.div("shop-col", dom.h3("", column.title), dom.ul("check-list", table.unpack(items)))
        )
    end

    return dom.section(
        "shop-section",
        dom.div(
            "section-head",
            dom.tag("span", "section-num", "03"),
            dom.h2("", "Shopping list"),
            dom.tag("span", "section-aside", "2 people · 3 days · scale to your table")
        ),
        dom.div("shop-grid", table.unpack(columns))
    )
end

local function build_prep(config: Config): string
    local steps: {string} = {}

    for index, step in ipairs(config.prep_steps) do
        table.insert(
            steps,
            dom.div(
                "prep-step",
                dom.tag("span", "prep-index", string.format("%02d", index)),
                dom.tag("span", "prep-label", step.label),
                dom.tag("span", "prep-text", step.text)
            )
        )
    end

    return dom.section(
        "prep-section",
        dom.div(
            "section-head",
            dom.tag("span", "section-num", "04"),
            dom.h2("", config.prep_title),
            dom.tag("span", "section-aside", "Do these first")
        ),
        dom.div("prep-grid", table.unpack(steps))
    )
end

function M.render(config: Config): string
    local footer = dom.tag(
        "footer",
        "page-footer",
        dom.tag("span", "footer-text", config.footer_note),
        dom.tag("span", "footer-mark", "72H")
    )

    return build_header(config)
        .. dom.main(
            "",
            build_quantity_table(config),
            build_menu(config),
            build_shopping(config),
            build_prep(config)
        )
        .. footer
end

return M as contracts.TemplateRender
