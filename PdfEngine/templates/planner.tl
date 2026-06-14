return function(layout)
    local html = string.format([[
        <h1>%s</h1>
        <header>
            <div class="header-item">MONTH: <span class="line"></span></div>
            <div class="header-item">YEAR: <span class="line"></span></div>
        </header>
        <main>
    ]], layout.header)

    local function row(label, isHeader)
        local cls = isHeader and 'row row-title' or 'row'
        return string.format('<div class="%s"><div>%s</div><div>%s</div></div>',
            cls,
            label or "",
            isHeader and "Amount" or ""
        )
    end

    for _, col in ipairs(layout.columns) do
        html = html .. '<div class="column">'
--        for _, section in ipairs(col) do
--            local cssClass = section.is_summary and 'section class="summary"' or 'section'
--            html = html .. string.format('<%s><div class="section-title"><div>%s</div><div>Amount</div></div>', cssClass, section.title)

        for _, section in ipairs(col) do
            html = html .. '<section>'
            html = html .. row(section.title, true)
            html = html .. '<div class="item-container">'

            for i = 1, section.max_rows do
                local item = section.items and section.items[i]
                local label = ""

                if type(item) == "table" and item.label then
                    label = item.label
                elseif type(item) == "string" then
                    label = item
                end

                html = html .. row(label, false)
            end

            html = html .. '</div></section>'           
--            for i = 1, section.max_rows do
--                local item = section.items and section.items[i]
--                local label = ""
--                
--                if type(item) == "table" and item.label then
--                    label = item.label
--                elseif type(item) == "string" then
--                    label = item
--                end
--
--                html = html .. string.format(
--                    '<div class="item"><div>%s</div><div></div></div>', 
--                    label
--                )
--            end
--            html = html .. '</section>'
        end
        html = html .. '</div>'
    end
    local html = html .. '</main>'
    return html
end

