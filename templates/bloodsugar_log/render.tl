local dom = require("dom")
local contracts = require("contracts")

local record Config
   header: string
end

local M = {}

function M.render(config: Config): string
    local html = {}

    local grid = {}

    table.insert(grid, dom.div("cell header", "Date"))
    table.insert(grid, dom.div("cell header", "Time"))
    table.insert(grid, dom.div("cell header", "Level"))
    table.insert(grid, dom.div("cell header", "Notes"))

    for _ = 1, 25 do
        table.insert(grid, dom.div("cell", dom.tag("textarea", "", "")))
        table.insert(grid, dom.div("cell", dom.tag("textarea", "", "")))
        table.insert(grid, dom.div("cell", dom.tag("textarea", "", "")))
        table.insert(grid, dom.div("cell", dom.tag("textarea", "", "")))
    end

    table.insert(html, dom.h1("", config.header))

    table.insert(html,
        dom.div("grid", table.concat(grid))
    )

    return table.concat(html)
end

return M as contracts.TemplateRender
