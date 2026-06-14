return function(layout)
    local classMap = { "left", "middle", "right" }
    local columnsHtml = ""

    for i, col in ipairs(layout.columns) do
        local cssClass = classMap[i]
        local sectionData = col[1]
        local itemsHtml = "<ul>"
        for j, item in ipairs(sectionData) do
            if j > 1 then
                itemsHtml = itemsHtml .. string.format("<li>%s</li>", item)
            end
        end
        itemsHtml = itemsHtml .. "</ul>"

        columnsHtml = columnsHtml .. string.format([[
            <div class="column %s"><h2 class="%s">%s</h2>%s</div>]],
                cssClass, cssClass, sectionData.title, itemsHtml)
    end

    return string.format([[<h1>%s</h1><main>%s</main>]], layout.header, columnsHtml)
end
