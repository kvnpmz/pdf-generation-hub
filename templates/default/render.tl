local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {{Section}}
end

require("contracts")
local M = {}

function M.render(config: Config): string
    local headerHtml = string.format('<h1>%s</h1>', config.header)

    local columnsHtml = ""
    for _, colItems in ipairs(config.columns) do
        local colContent = ""

            for _, section in ipairs(colItems) do
                local rowHtml = string.format(
                    '<div class="section-title">%s</div>' ..
                    '<div class="section-subtitle">%s</div>' ..
                    '<div class="item">%s</div>',
                    section.title or "", 
                    section.subtitle or "",
                    section.item or ""
                )
            colContent = colContent .. string.format('<section>%s</section>', rowHtml)
        end
        columnsHtml = columnsHtml .. string.format('<div class="column">%s</div>', colContent)
    end

    html = headerHtml .. string.format('<main>%s</main>', columnsHtml)
    return html
end

return M as TemplateRender
