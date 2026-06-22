local record Section
   title: string
   items: {string}
   sections: {Section} 
end

local record Column
   title: string
   sections: {Section}
end

local record Config
   header: string
   columns: {Column} 
end

local contracts = require("contracts")
local M = {}

function M.render(config: Config): string
    local classMap = { "left", "middle", "right" }
    local columnsHtml = ""

    for i, col in ipairs(config.columns) do
        local cssClass = classMap[i]
        local colTitle = col.title 
        local sectionsHtml = ""

        for _, section in ipairs(col.sections) do
            local itemsHtml = ""
            for _, item in ipairs(section.items) do
                itemsHtml  = itemsHtml  .. string.format('<li>%s</li>', item)
            end

            sectionsHtml = sectionsHtml .. string.format(
                '<ul><h3>%s</h3>%s</ul>', 
                section.title, itemsHtml
            )
        end

        columnsHtml = columnsHtml .. string.format(
            '<div class="column"><h2 class="%s">%s</h2><div class="sections-container %s">%s</div></div>', 
            cssClass, colTitle, cssClass, sectionsHtml
        )
    end

    return string.format([[<h1>%s</h1><main>%s</main>]], config.header, columnsHtml)
end

return M as contracts.RenderStrategy
