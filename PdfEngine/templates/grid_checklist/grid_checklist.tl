return function(layout)
--    local pastelColors = {"#8FB8FF", "#FF9AA2", "#FFD36E"}
    local headerHtml = string.format('<h1>%s</h1>', layout.header)
    local columnsHtml = ""
    local sectionCounter = 1

    for _, colItems in ipairs(layout.columns) do
        local colContent = ""

            for _, section in ipairs(colItems) do
--                local color = pastelColors[(sectionCounter - 1) % #pastelColors + 1]

                local listItemsHtml = ""
                for _, item in ipairs(section.items or {}) do
                    listItemsHtml = listItemsHtml .. string.format('<li>%s</li>', item)
                end
                
                local sectionHtml = string.format(
--                    '<h2 style="background-color: %s;">%s</h2><ul>%s</ul>',
--                    color,
                    '<h2>%s</h2><ul>%s</ul>',
                    section.title or "",
                    listItemsHtml
                )

                colContent = colContent .. string.format(
--                    '<section style="border: 1.5pt solid %s;">%s</section>', 
--                    color,
                    '<section>%s</section>', 
                    sectionHtml
                )
                sectionCounter = sectionCounter + 1
            end
        columnsHtml = columnsHtml .. string.format('<div class="column">%s</div>', colContent)
    end

    html = headerHtml .. string.format('<main>%s</main>', columnsHtml)
    return html
end
