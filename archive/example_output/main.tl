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

local inventory = {
    -- COLUMN 1
    {
        { title = "Entrance", items = {"Coat Rack", "Doormat", "Couch", "Cushions", "Chairs", "Coffee Table", "TV", "Book Case", "Floor Lamp", "Speakers", "Power Sockets", "Curtains", "Wall Art", "Ceiling Lights"} },
        { title = "Dining area", items = {"Dining Table", "Chairs", "Placemats", "Tablecloth", "Pot Holders"} },
        { title = "Appliances", items = {"Refrigerator", "Microwave", "Oven", "Cooker", "Kettle", "Toaster", "Coffee Machine"} },
        { title = "Glassware", items = {"Wine Glasses", "Water Glasses", "Pitcher"} },
        { title = "Cookware", items = {"Frying Pan", "Wok", "Pots", "Saucepans"} }
    },
    -- COLUMN 2
    {
        { title = "Office", items = {"Desk", "Desk Light", "Book Shelf", "Desk Chair", "Keyboard / Mouse", "Mouse Mat", "Speakers", "Printer", "Computer", "Laptop Stand", "Pen", "Pencil", "Markers", "Paper", "Post-it Notes", "Envelopes", "Stamps", "Paperclips", "Tape", "Organizers"} },
        { title = "Cutlery", items = {"Knives", "Forks", "Spoons", "Teaspoons"} },
        { title = "Utensils", items = {"Serving Spoons", "Ladle", "Spatula", "Whisk", "Tongs", "Pizza Cutter", "Peeler", "Scissors", "Bottle Opener", "Corkscrew", "Colander", "Measuring Cup"} }
    },
    -- COLUMN 3
    {
        { title = "Cleaning", items = {"Vacuum Cleaner", "Broom", "Dust Pan", "Bucket", "Mop", "Cleaning Cloths", "Sponges", "Cleaning Gloves", "All-Purpose Cleaner", "Bleach", "Toilet Cleaner", "Window Cleaner", "Glass Wipes", "Step Ladder"} },
        { title = "Knives", items = {"Chef's Knife", "Pairing Knife", "Slicing Knife", "Bread Knife", "Utility Knife", "Cutting Board"} },
        { title = "Dinnerware", items = {"Plates", "Side Plates", "Bowls", "Deep Plates", "Serving Bowls", "Coffee Mugs", "Tea Cups"} },
        { title = "Kitchen misc.", items = {"Food Containers", "Jars", "Spice Rack", "Bin", "Oven Mitts", "Cutlery Tray", "Lighter"} }
    },
    -- COLUMN 4
    {
        { title = "Bedroom", items = {"Mattress", "Duvet", "Duvet Covers", "Bed Sheets", "Pillows", "Pillow Cases", "Closet", "Chest Of Drawers", "Night Table", "Night Light", "Chair", "Under-Bed Storage Boxes", "Curtains", "Curtain Rails", "Alarm Clock", "Clothing Hangers", "Ceiling Lights"} },
        { title = "Bathroom", items = {"Bath Mat", "Shower Curtain", "Mirror", "Medicine Cabinet", "Towel Holder", "Towel Rail", "Bath Towels", "Hand Towels", "Shower Caddy", "Scale", "Hair Dryer", "Waste Bin", "Shelves", "Soap Dispenser", "Soap", "Shampoo", "Shower Gel", "Toothpaste", "Toothbrush"} }
    },
    -- COLUMN 5
    {
        { title = "Laundry", items = {"Washing Machine", "Laundry Basket", "Drying Rack", "Detergent", "Iron", "Ironing Board"} },
        { title = "Toilet", items = {"Toilet Brush", "Toilet Paper", "Toilet Paper Holder", "Toilet Plunger", "Waste Bin", "Air Freshener", "Hand Soap", "Hand Towel"} },
        { title = "Tools", items = {"Hammer", "Nails", "Screwdriver Set", "Screws", "Saw", "Wrench", "Pliers", "Scissors", "Tape Measure"} },
        { title = "Other", items = {"Smoke Detector", "Batteries", "Power Outlet", "Power Extension Cords", "Storage Boxes", "Candles", "Light Bulbs", "Flash Light", "Memo Board", "Magnets", "Vase"} }
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

