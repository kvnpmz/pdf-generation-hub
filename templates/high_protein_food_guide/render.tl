local contracts = require("contracts")
local dom = require("dom")

local record FoodItem
    name: string
    calories: string
    protein: string
end

local record Config
    header: string
    items: {FoodItem}
    footer: string
end

local M = {}

function M.render(config: Config): string
   local words: {string} = {}
    for word in config.header:gmatch("%S+") do
        table.insert(words, word)
    end

    local group1 = (words[1] or "") .. " " .. (words[2] or "")
    local group2 = (words[3] or "") .. " " .. (words[4] or "")

    local top_header = dom.h1("", group1)
    local bottom_header = dom.h1("", group2)

    local html_header = dom.header("", top_header, bottom_header)

    local food_cards: {string} = {}
    for _, item in ipairs(config.items) do
        local card = dom.div("food-card",
            dom.h3("", item.name),
            dom.div("food-stats",
                dom.div("stat-item", "Calories: " .. item.calories),
                dom.div("stat-item", "Protein: " .. item.protein)
                )
            )

        table.insert(food_cards, card)
    end

    local grid_container = dom.main("", table.unpack(food_cards))
    local footer = dom.div("footer", config.footer)

    return html_header .. grid_container .. footer
end

return M as contracts.TemplateRender

