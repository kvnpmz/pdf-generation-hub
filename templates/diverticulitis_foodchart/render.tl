local dom = require("dom")
local contracts = require("contracts")

local record NestedItem
    title: string
    items: {string}
end

local record DetailItem
    name: string
    description: string
end

local record Item
   title: string
   items: {string}
   name: string
   description: string
end

local record Section
    title: string
    type: string
    items: {Item}
end

local record Column
    column_name: string
    sections: {Section}
end

local record Config
    header: string
    columns: {Column}
end

local M = {}

local function concat_map(list: {string}, func: function(string): string): string
    local result: {string} = {}
    for i, v in ipairs(list) do
        result[i] = func(v)
    end
    return table.concat(result)
end

function M.render(config: Config): string
    local columns_html = {}
    
    for columnIndex, column in ipairs(config.columns) do
        local sections_html = {}

        for _, section in ipairs(column.sections) do
            local content = { dom.h3("", section.title) }

            local listClass = ""
            if #section.items > 1 then
                listClass = "two-column-list"
            else
                listClass = "one-column-list"
            end

            if section.type == "simple" then
                local items = section.items as {string}
                table.insert(content, dom.ul(listClass, concat_map(items, function(item: string): string 
                    return dom.li("", item) 
                end)))
            elseif section.type == "nested" then
                for _, n in ipairs(section.items) do
                    local nested = n as NestedItem
                    local nested_html = {} 
                    table.insert(nested_html, dom.tag("h4", "", nested.title))

                    local nestedClass = (#nested.items > 1) and "two-column-list" or "one-column-list"
                    table.insert(
                        nested_html,
                        dom.ul(
                            nestedClass, 
                            concat_map(
                                nested.items, 
                                function(item: string): string
                                    return dom.li("", item) 
                                end
                            )
                        )
                    )
                    table.insert(content, dom.div("nested-item", table.concat(nested_html)))
                end
            elseif section.type == "detailed" then
                for _, d in ipairs(section.items) do
                    local detail = d as DetailItem
                    local detail_html =  {}
                    table.insert(detail_html, detail.name)
                    table.insert(detail_html, " ")
                    table.insert(detail_html, detail.description)
                    table.insert(content, dom.div("detail-item", table.concat(detail_html)))
                end
            end
            table.insert(sections_html, dom.section("", table.concat(content)))
        end

        local column_html = {}
        table.insert(column_html, dom.h2("", column.column_name))

        for _, section_html in ipairs(sections_html) do
            table.insert(column_html, section_html)
        end

        local columnClass = (columnIndex == 1) and "green" or "red"
        table.insert(columns_html, dom.div("column " .. columnClass, table.concat(column_html)))
    end

    local html = {}
    table.insert(html, dom.h1("", config.header))
    table.insert(html, table.concat(columns_html))
    return table.concat(html)
end

return M as contracts.TemplateRender
