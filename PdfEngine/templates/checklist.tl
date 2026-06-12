return function(layout)
    local pastelColors = {"#FFD180", "#8C9EFF", "#C8E6C9", "#80D8FF", "#FFA8FF", "#BAFFC9", "#FFAB91", "#EA80FC"}
    local headerHtml = string.format('<h1>%s</h1>', layout.header)
    local columnsHtml = ""
    local sectionCounter = 1

    for _, colItems in ipairs(layout.columns) do
        local colContent = ""

            for _, section in ipairs(colItems) do
                local color = pastelColors[(sectionCounter - 1) % #pastelColors + 1]

                local listItemsHtml = ""
                for _, item in ipairs(section.items or {}) do
                    listItemsHtml = listItemsHtml .. string.format('<li>%s</li>', item)
                end
                
                local sectionHtml = string.format(
                    '<h2>%s</h2><ul>%s</ul>',
                    section.title or "",
                    listItemsHtml
                )

                colContent = colContent .. string.format(
                    '<section style="border: 1.5pt solid %s;">%s</section>', 
                    color, sectionHtml
                )
                sectionCounter = sectionCounter + 1
            end
        columnsHtml = columnsHtml .. string.format('<div class="column">%s</div>', colContent)
    end

    html = headerHtml .. string.format('<main>%s</main>', columnsHtml)
    return html
end
