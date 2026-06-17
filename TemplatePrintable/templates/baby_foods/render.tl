local record Section
    title: string
    color: string
    items: {string}
end

local record Layout
    header: string
    columns: {{Section}}
end

local function render(config: Layout): string
    local headerHtml = string.format('<h1>%s<strong>ñ</strong></h1>', config.header)
    local columnsHtml = "" 

    for _, colItems in ipairs(config.columns) do
        local colContent = "" 

        for _, section in ipairs(colItems) do 
            local itemsList = "" 

            for _, item in ipairs(section.items) do 
                itemsList = itemsList .. string.format('<li>%s <span class="dots"></span></li>',
                item)
            end

            local sectionHtml = string.format( '<div class="section-title">%s</div>' .. '<ul>%s</ul>', section.title or "", itemsList )
            colContent = colContent .. string.format('<section class="%s">%s</section>', section.color or "", sectionHtml)
        end

        columnsHtml = columnsHtml .. string.format('<div class="column">%s</div>', colContent)
    end

    return headerHtml .. string.format('<main>%s</main>', columnsHtml)

end

return render
