local inventory = require("inventory")

local record PdfData
    title: string
    margin: string
    size: string
    containerHeight: string
    lineHeight: string
    bulletOffset: string
    inventoryContent: string
end

local letter = { width = 612, height = 792 }

local baseSettings = {
    marginTop = 36,
    marginBottom = 36,
    containerHeight = 680,
    lineHeight = 16.0,
    bulletOffset = 3.0,
}

local paperSettings = {
    Letter = baseSettings,
    A4 = {
        marginTop = baseSettings.marginTop - 37,
        marginBottom = baseSettings.marginBottom - 38,
        containerHeight = baseSettings.containerHeight + 76,
        lineHeight = baseSettings.lineHeight + 2.0,
        bulletOffset = baseSettings.bulletOffset + 1.0,
    }
}

local function buildInventoryHtml(inventoryColumns: {{Category}}): string
    local lines = {}

    local function add(text, indent)
        table.insert(lines, string.rep("    ", indent or 0) .. text)
    end

    for _, column in ipairs(inventoryColumns) do
        add('<!-- COLUMN -->', 0)
        add('<div class="category">', 0)
        add('<div class="column">', 1)

        for _, category in ipairs(column) do
            add('<section>', 2)
            add(string.format('<h2>%s</h2>', category.title), 3)
            add('<ul>', 3)
            for _, item in ipairs(category.items) do
                add(string.format('<li>%s</li>', item), 4)
            end
            add('</ul>', 3)
            add('</section>', 2)
        end

        add('</div>', 1)
        add('</div>', 0)
    end

    return table.concat(lines, "\n")
end

local function loadTemplate(path: string): string
    local file = io.open(path, "r")
    if not file then error("Could not open template file: " .. path) end
    local content = file:read("*a")
    file:close()
    return content
end

local function render(template: string, data: PdfData): string
    local html = template
    local css = loadTemplate("style.css")
    html = html:gsub("{{STYLE}}", css:gsub("%%", "%%%%"))
    html = html:gsub("{{TITLE}}", data.title)
    html = html:gsub("{{MARGIN}}", data.margin)
    html = html:gsub("{{SIZE}}", data.size)
    html = html:gsub("{{CONTAINER_HEIGHT}}", data.containerHeight)
    html = html:gsub("{{LINE_HEIGHT}}", data.lineHeight)
    html = html:gsub("{{BULLET_OFFSET}}", data.bulletOffset)
    html = html:gsub("{{INVENTORY_CONTENT}}", data.inventoryContent)
    return html
end

local formats = {"A4", "Letter"}
local htmlTemplate = loadTemplate("template.html")
local baseName = "output.pdf"

for _, format in ipairs(formats) do
    local config = paperSettings[format]
    local generatedContent = buildInventoryHtml(inventory)

    local data: PdfData = {
        title = "New Home Checklist",
        margin = string.format("%dpt 50.4pt %dpt", config.marginTop, config.marginBottom),
        size = string.format("%dpt %dpt", letter.width, letter.height),
        containerHeight = string.format("%dpt", config.containerHeight),
        lineHeight = string.format("%dpt", config.lineHeight),
        bulletOffset = string.format("%dpt", config.bulletOffset),
        inventoryContent = generatedContent
    }

    local content = render(htmlTemplate, data)
    exportPdf(content, baseName, format)
end

local file = io.open("inventory.html", "w")

if file then
    file:write(buildInventoryHtml(inventory))
    
    file:close()
else
    print("Error: Could not open file for writing.")
end

