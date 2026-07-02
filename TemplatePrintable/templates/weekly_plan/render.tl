local dom = require("dom")
local contracts = require("contracts")

local record Config
   header: string
end

local M = {}

function M.render(config: Config): string
    local html = {}
    table.insert(html, dom.h1("", config.header))
    table.insert(html, dom.h2("", "Date:"))

    local sections = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Notes"}

    for i, section in ipairs(sections) do
        local lines = {}
        local className = "tab-color-" .. i

        for _ = 1, 9 do
            table.insert(lines, dom.div("line-box " .. className, dom.tag("textarea", "line-input", "")))
        end

        table.insert(html, dom.section("", dom.h3(className, section), dom.div("line-container", table.concat(lines))))
    end

    return table.concat(html)
end

return M as contracts.TemplateRender
