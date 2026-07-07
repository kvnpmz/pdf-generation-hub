local record Field
    key: string
    label: string
end

local record Section
    title: string
    type: string
    id: integer
    fields: {Field}
end

local record Column
    items: {Section}
end

local record Config
    header: string
    columns: {Column}
end

local contracts = require("contracts")
local M = {}

local function render_headers(fields: {Field}): string
    local html = '<div></div>'
    for _, field in ipairs(fields or {}) do
        html = html .. string.format('<div class="col-%s">%s</div>',
            field.key, field.label)
    end
    return html
end

local function render_section(title: string, rows: integer, fields: {Field}): string
    local grid_class = "grid-" .. title:lower():gsub("%s+", "-")
    local row_html = ""

    local footerMap: {string: string} = {
        Bills = "Total Bills Paid",
        Spending = "Total Spending",
        ["Savings Goal"] = "Total Savings",
        ["Extra Debts"] = "Total Debt Paid",
    }

    local footer_text = footerMap[title] or ""

    for _ = 1, (rows or 1) do
        local cells = ""
        for _, field in ipairs(fields or {}) do
            cells = cells .. string.format('<div class="box-%s"></div>', field.key)
        end
        row_html = row_html .. string.format([[
            <div class="row-items %s"><div class="line-grow"></div>%s</div>
        ]], grid_class, cells)
    end

    local section_header_html = render_headers(fields)

    return string.format([[
        <section class="budget-section">
            <div class="section-title">%s</div>
            <div class="section-header %s"> %s </div>
            %s
            <div class="section-footer %s">
                <div>%s</div>
                <div class="footer-box"></div>
                <div></div>
            </div>
        </section>
    ]],
    title, grid_class, section_header_html,
    row_html, grid_class, footer_text)
end

local function render_summary(): string
    local function row(label: string): string
        if label == nil then
            return '<div class="summary-row minus-text"><div></div>minus</div>'
        end
        return '<div class="summary-row">' .. label .. ' <div class="box-amount"></div></div>'
    end

    return [[
        <section class="summary-box">
            <div class="section-title">Budget Summary</div>
            ]] .. row("total income") .. [[
            ]] .. row(nil) .. [[
            ]] .. row("total bills paid") .. [[
            ]] .. row("total spending") .. [[
            ]] .. row("total savings") .. [[
            ]] .. row("total debt paid") .. [[
            ]] .. row("unused money") .. [[
        </section>
    ]]
end

function M.render(config: Config): string
    local html = '<main><div class="month-header">'
    local months = {
        "Jan", "Feb", "Mar", "Apr", "May",
        "June", "July", "Aug", "Sept",
        "Oct", "Nov", "Dec"
    }

    for _, month in ipairs(months) do
        html = html .. '<div>' .. month .. '</div>'
    end

    html = html .. '</div>'
    html = html .. '<header><h1>' .. config.header ..
        '</h1><div class="date-box"><div>Date</div></div></header>'

    html = html ..[[<div class="income-header">
                    <div class="income-title">Sources of Income</div>
                    <div>Budget</div><div>Actual</div></div>]]

    for _, source in ipairs({"Paycheck Amount", "Other Income Source"}) do
        html = html .. string.format([[
            <div class="income-row">
                <div>%s</div>
                <div class="line-grow"></div>
                <div class="box-amount"></div>
                <div class="box-amount"></div>
            </div>
        ]], source)
    end

    html = html .. [[<div class="section-footer grid-income">
        <div>Total Income</div><div class="footer-box">
        </div><div class="footer-box"></div></div>]]

    html = html .. '<div class="grid-layout">'

    for _, col in ipairs(config.columns) do
        html = html .. '<div class="column">'
        for _, section in ipairs(col.items) do
            if section.type == "summary" then
                html = html .. render_summary()
            else
                html = html .. render_section(section.title, section.id, section.fields)
            end
        end
        html = html .. '</div>'
    end
    html = html .. '</div></main>'
    return html
end

return M as contracts.TemplateRender
