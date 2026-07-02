local dom = require("dom")
local contracts = require("contracts")

local record Section
   title: string
   items: {string}
end

local record Config
   header: string
   columns: {{Section}}
end

local M = {}

function M.render(config: Config): string
    local function sectionClass(columnIndex: integer, sectionIndex: integer): string
        if columnIndex == 4 then
            if sectionIndex >= 1 and sectionIndex <= 5 then
                return "amber"
            else
                return "red"
            end
        end
        return ""
    end

    local function columnClass(i: integer): string
        local classes = { "column" }
        if i >= 1 and i <= 3 then
            table.insert(classes, "green")
        elseif i == 5 then
            table.insert(classes, "red")
        end
        return table.concat(classes, " ")
    end

    local columnHtml: {string} = {}

    for i, column in ipairs(config.columns) do
        local sectionHtml: {string} = {}

        for j, section in ipairs(column) do
            local itemsHtml: {string} = {}

            for _, item in ipairs(section.items) do
                table.insert(itemsHtml, dom.li("", item))
            end
            
            local sClass = sectionClass(i, j)
            table.insert(sectionHtml, dom.section(sClass, 
                dom.h3("", section.title),
                dom.ul("", table.unpack(itemsHtml))
            ))
        end

        columnHtml[i] = dom.div(columnClass(i), table.unpack(sectionHtml))
    end

    local header = dom.header("", 
        dom.h1("", config.header), 
        dom.h2("green", "Green: Enjoy Freely."), 
        dom.h2("amber", "Amber: Eat Some."), 
        dom.h2("red", "Red: Eat Little.")
    )
    
    local main = dom.main("", table.unpack(columnHtml))

    return table.concat({header, main})
end

return M as contracts.TemplateRender
