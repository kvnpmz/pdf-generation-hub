local dom = require("dom")
local contracts = require("contracts")

local record Config
   header: string
end

local M = {}

function M.render(config: Config): string
--    local headerHtml = {}
    local headerHtml = ""
    --table.insert(headerHtml, dom.header("", dom.h1("", config.header), dom.h2("", "Date:")))
    headerHtml = headerHtml .. dom.h1("", config.header) .. dom.h2("", "Date:")

    local columnHtml = {}
    local sections = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Notes"}

    for i, section in ipairs(sections) do
        local lines = ""
        local className = "tab-color-" .. i

        for _ = 1, 9 do
            --table.insert(lines, dom.tag("input", "line " .. className, ""))
            lines = lines .. dom.div("line-wrapper " .. className, dom.tag("textarea", "line-input ", ""))
        end

        --table.insert(columnHtml, dom.section("", dom.h3(className, section), dom.div("line-container", table.unpack(lines))) )
        table.insert(columnHtml, dom.section("", dom.h3(className, section), dom.div("line-container", lines)))
    end

    --local html = table.concat(headerHtml) .. dom.main("", table.unpack(columnHtml))
--    local html = headerHtml .. dom.main("", table.unpack(columnHtml))
    local html = headerHtml .. table.concat(columnHtml)
    return html
end

return M as contracts.TemplateRender
