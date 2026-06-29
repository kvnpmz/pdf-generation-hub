local dom = require("dom")
local contracts = require("contracts")

local type Item = string

local record Column
    title: string
    items: {Item}
end

local record Config
    header: string
    columns: {Column}
end

local M = {}

function M.render(config: Config): string
    local classMap = { "left", "middle", "right" }
    local columnNodes = {}
    for i, column in ipairs(config.columns) do
        local colClass = classMap[i] or "default"
            local listItems = {}

        for _, item in ipairs(column.items) do
            table.insert(listItems, dom.li("", item))
        end

        table.insert(
           columnNodes,
           dom.div(
                "column " .. colClass,
                dom.h2("", column.title),
                dom.ul("", table.unpack(listItems))
            )
        )
    end

    return dom.h1("", config.header) .. dom.main("", table.unpack(columnNodes))
end

return M as contracts.TemplateRender
