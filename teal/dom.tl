local record MRecord
    tag: function(string, string, ...: string): string
    header: function(string, ...: string): string
    main: function(string, ...: string): string
    section: function(string, ...: string): string
    h1: function(string, ...: string): string
    h2: function(string, ...: string): string
    h3: function(string, ...: string): string
    ul: function(string, ...: string): string
    li: function(string, ...: string): string
    table: function(string, ...: string): string
    thead: function(string, ...: string): string
    tbody: function(string, ...: string): string
    th: function(string, ...: string): string
    tr: function(string, ...: string): string
    td: function(string, ...: string): string
    div: function(string, ...: string): string
end

local M = { } as MRecord

function M.tag(name: string, className: string, ...: string): string
    local attributes = (className and className ~= "") and string.format(' class="%s"', className) or ""
    local content = table.concat({...}, "")
    
    if attributes == "" then
        return string.format("<%s>%s</%s>", name, content, name)
    end
    
    local indent = "\n"
    return string.format("%s<%s%s>%s</%s>", indent, name, attributes, content, name)
end

local tags = { "header", "main", "section", "h1", "h2", "h3", "ul", "li", "table", "thead", "tbody", "th", "tr", "td", "div" }
local M_as_map = M as {string:function(string, ...: string): string}

for _, tag in ipairs(tags) do
  M_as_map[tag] = function(className: string, ...:string): string
    return M.tag(tag, className, ...)
  end
end

return M
