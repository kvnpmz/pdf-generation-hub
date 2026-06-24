local dom = require("dom")
local contracts = require("contracts")

local record Config
    header: string
end

local M = {}

function M.render(config: Config): string
    local function dotted_line(isShopping: boolean): string
        local count = isShopping and 14 or 29
        return dom.div("dotted-line", string.rep("⬤", count))
    end

    local function breakfast_row(): string
        return dom.div("meal-row", "B") .. dotted_line(false)
    end
    
    local function lunch_row(): string
        return dom.div("meal-row", "L") .. dotted_line(false)
    end

    local function dinner_row(): string
        return dom.div("meal-row", "D") ..  dotted_line(false)
    end

    local function day_box(name: string, class: string): string
        return dom.div("day-box " .. class,
            dom.h3("", name),
            breakfast_row(), lunch_row(), dinner_row()
        )
    end

    local function shopping_row(): string
        return dom.div("shopping-row", "●") .. dotted_line(true)
    end

    local function shopping_category(category_title: string): string
        local rows = {}
        local count = 10
        for _ = 1, count do
            rows[#rows + 1] = shopping_row()
        end

        local function remove_period(text: string): string
            return (text:gsub("%.", ""))
        end

        return dom.div(
            "sub-col category-" .. remove_period(category_title),
            dom.h3("", category_title),
            table.unpack(rows) 
        )
    end

    local fullPage = dom.h1("", config.header) .. 
        dom.header("", dom.div("", "Week Of:")) ..
        dom.main("",
            dom.div("column",
                day_box("Sunday", "bg-sunday"),
                day_box("Monday", "bg-monday"),
                day_box("Tuesday", "bg-tuesday"),
                day_box("Wednesday", "bg-wednesday"),
                day_box("Thursday", "bg-thursday"),
                day_box("Friday", "bg-friday"),
                day_box("Saturday", "bg-saturday")
            ),
            dom.div("column",
                dom.h2("", "Shopping List"),
                dom.div("sub-grid", shopping_category(string.lower("Dairy")), shopping_category(string.lower("Produce"))),
                dom.div("sub-grid", shopping_category(string.lower("Grains")), shopping_category(string.lower("Meat"))),
                dom.div("sub-grid", shopping_category(string.lower("Frozen")), shopping_category(string.lower("Misc.")))
            )
        )

    return fullPage
end

return M as contracts.RenderStrategy

