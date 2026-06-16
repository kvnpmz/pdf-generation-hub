return function(layout)
    local html = string.format([[
        <h1>%s</h1>
        <header>
            <div>MONTH: <span class="line"></span></div>
            <div>YEAR: <span class="line"></span></div>
        </header>
        <main>
    ]], layout.header)

    for _, col in ipairs(layout.columns) do
        html = html .. '<div class="column">'
        for _, section in ipairs(col) do
            local label = ""
            html = html .. string.format('<section><div class="row"><div>%s</div><div>Amount</div></div>', section.title)

            for i = 1, section.max_rows do
                local item = section.items and section.items[i]
                local label = ""
                
                if item then -- Only if an item exists
                    if type(item) == "table" and item.label then
                        label = item.label
                    elseif type(item) == "string" then
                        label = item
                    end
                end

                html = html .. string.format('<div class="row"><div>%s</div><div></div></div>', label)
            end
            html = html .. '</section>'
        end
        html = html .. '</div>'
    end
    return html
end

