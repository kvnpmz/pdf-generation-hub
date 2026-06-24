local dom = require("dom")
local contracts = require("contracts")

local record Section
    title: string
    items: {string}
end

local record Config
    header: string
    columns: {Section}
end

local M = {}

function M.render(config: Config): string
    local function dotted_line(isShopping)
        local count = isShopping and 21 or 44
        return dom.div("dotted-line", string.rep("⬤", count))
    end

    local function breakfast_row()
        return dom.div("meal-row", "B") .. dotted_line(false)
    end
    
    local function lunch_row()
        return dom.div("meal-row", "L") .. dotted_line(false)
    end

    local function dinner_row()
        return dom.div("meal-row", "D") .. dotted_line(false)
    end

    local function day_box(name, class)
        return dom.div("day-box " .. class,
            dom.div("day-title", name),
            breakfast_row(), lunch_row(), dinner_row()
        )
    end

    local function shopping_row()
        return dom.div("meal-row", "○") .. dotted_line(true)
    end

    local function shopping_category(title)
        local rows = {}
        local count = 10
        for i = 1, count do rows[#rows + 1] = shopping_row() end

        return dom.div("sub-col", dom.div("list-title", title), table.unpack(rows))
    end

    local fullPage = dom.h1("title", config.header) .. 
        dom.header("", dom.div("", "Week Of:")) ..
        dom.main("",
            dom.div("column",
                day_box("Sunday", "bg-blue"),
                day_box("Monday", "bg-green"),
                day_box("Tuesday", "bg-orange"),
                day_box("Wednesday", "bg-pink"),
                day_box("Thursday", "bg-peach"),
                day_box("Friday", "bg-purple"),
                day_box("Saturday", "bg-red")
            ),
            dom.div("column",
                dom.div("shopping-header", "Shopping List"),
                dom.div("sub-grid", shopping_category("Dairy"), shopping_category("Produce")),
                dom.div("sub-grid", shopping_category("Grains"), shopping_category("Meat")),
                dom.div("sub-grid", shopping_category("Frozen"), shopping_category("Misc."))
            )
        )

    return fullPage
end

return M as contracts.RenderStrategy

