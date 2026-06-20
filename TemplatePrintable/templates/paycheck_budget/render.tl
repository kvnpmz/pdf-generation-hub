local record Group
    sub: string
    items: {string}
end

local record Section
    title: string
    items: {string}
    groups: {Group}
end

local record Config
    header: string
    columns: {{Section}}
end

local contracts = require("contracts")
local M = {}

function M.render(config: Config): string
    local html = '<main><div class="month-header">'
    local months = {"Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sept", "Oct", "Nov", "Dec"}
    for _, month in ipairs(months) do
        html = html .. '<div>' .. month .. '</div>'
    end
    html = html .. '</div>'
    html = html .. '<header><h1>' .. config.header .. '</h1><div class="date-box"><div>DATE:</div></div></header>'

    html = html .. '<div class="income-title">Sources of Income</div>'
    html = html .. '<div class="income-header"><span></span><span>Budget</span><span>Actual</span></div>'

    for _, source in ipairs({"Paycheck Amount", "Other Income"}) do
        html = html .. string.format([[
            <div class="income-row">
                <span>%s</span>
                <div class="line-grow"></div>
                <div class="box-amount"></div>
                <div class="box-amount"></div>
            </div>
        ]], source)
    end

    local function render_section(title, rows, fields)
        local first_col = "auto"
        local header_cols = first_col
        local row_cols = first_col

        for i = 1, #fields do
            local is_last = (i == #fields and #fields > 2)

            if is_last and title == "Bills" then
                header_cols = header_cols .. " 25pt"
                row_cols = row_cols .. " 14pt"
            else
                header_cols = header_cols .. " 50pt"
                row_cols = row_cols .. " 50pt"
            end
        end

        local header_style = string.format('style="grid-template-columns: %s;"', header_cols)
        local row_style = string.format('style="grid-template-columns: %s;"', row_cols)
        local row_html = ""
        for _ = 1, (rows or 1) do
            local cells = ""

            for _, field in ipairs(fields or {}) do
                cells = cells .. string.format(
                    '<div class="box-%s"></div>',
                    field.key
                )
            end

            row_html = row_html .. string.format([[
                <div class="row-items" %s>
                    <div class="line-grow"></div>
                    %s
                </div>
            ]], row_style, cells)
        end

        local function render_headers(fields)
            local html = '<div></div>'
            for _, field in ipairs(fields or {}) do
                html = html .. string.format(
                    '<span class="col-%s">%s</span>',
                    field.key,
                    field.label
                )
            end
            return html
        end

        local section_header_html = render_headers(fields)
        local footer_class = (title == "Bills") and "footer-bills"

        return string.format([[
            <section class="budget-section">
                <div class="section-title">%s</div>
                <div class="section-header" %s> %s </div>
                %s
                <div class="section-footer %s" %s>
                    <span>Total %s Paid</span>
                    <div class="footer-box"></div>
                    <div></div>
                </div>
            </section>
        ]],
        title,
        header_style,
        section_header_html,
        row_html,
        footer_class,
        row_style,
        title)
    end

    local function render_summary()
        local function row(label)
            if not label then return '<div class="summary-row">minus</div>' end
            return '<div class="summary-row">' .. label .. ' <span class="box-amount"></span></div>'
        end

        return [[
            <section class="summary-box">
                <div class="section-title">budget summary</div>
                ]] .. row("total income") .. [[
                ]] .. row() .. [[
                ]] .. row("total bills paid") .. [[
                ]] .. row("total spending") .. [[
                ]] .. row("total savings") .. [[
                ]] .. row("total debt paid") .. [[
                ]] .. row("unused money") .. [[
            </section>
        ]]
    end

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

return M as contracts.RenderStrategy
