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
    local headerHtml = dom.h1("", config.header)
    
    local buckets: {{Section}} = {{}, {}, {}, {}, {}}
    
    for i, section in ipairs(config.columns) do
        local bucketIndex = ((i - 1) % 5) + 1
        table.insert(buckets[bucketIndex], section)
    end

    local seq = {1, 2, 3, 4, 5, 6, 7, 8}
    for i = #seq, 2, -1 do
        local j = math.random(i)
        seq[i], seq[j] = seq[j], seq[i]
    end
    local colorIndex = 1

    local columnDivs = {}
    for _, colSections in ipairs(buckets) do
        local sectionNodes = {}
        for _, section in ipairs(colSections) do
            if section.title == "SPACER" then
                table.insert(sectionNodes, dom.div("spacer-element", ""))
            else
                local colorNum = seq[colorIndex]
                local colorClass = string.format("color-%d", colorNum)
                colorIndex = (colorIndex % #seq) + 1

                local listItems = {}
                for _, item in ipairs(section.items or {}) do
                    table.insert(listItems, dom.li("", item))
                end

                local sectionHeader = dom.h2(colorClass, section.title or "")
                local ul = dom.ul("", table.unpack(listItems))
                table.insert(sectionNodes, dom.section(colorClass .. "-border", sectionHeader, ul))
            end
        end
        table.insert(columnDivs, dom.div("column", table.unpack(sectionNodes)))
    end

    return headerHtml .. dom.main("", table.unpack(columnDivs))
end

return M as contracts.RenderStrategy
