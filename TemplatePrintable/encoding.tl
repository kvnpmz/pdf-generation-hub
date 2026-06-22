local M = {}

function M.escape_html(string: string): string
    local result: {string} = {}
    for _, code in utf8.codes(string) do
        if code > 127 then
            table.insert(result, string.format("&#x%X;", code))
        else
            table.insert(result, utf8.char(code))
        end
    end
    return table.concat(result)
end

function M.escape_css(string: string): string
    local result: {string} = {}
    for _, code in utf8.codes(string) do
        if code > 127 then
            table.insert(result, string.format("\\%04X", code))
        else
            table.insert(result, utf8.char(code))
        end
    end
    return table.concat(result)
end

return M
