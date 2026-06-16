return function(layout)
    local headers = {"PAID", "BILLS", "DUE DATE", "AMOUNT DUE", "AMOUNT PAID", "UNPAID BALANCE", "NOTES"}
    
    local html = string.format([[
        <header><h1>%s</h1><div class="dates"><div class="field"><span>MONTH:</span><div class="line"></div></div><div class="field"><span>YEAR:</span><div class="line"></div></div></div></header><main><table><thead><tr>%s</tr></thead><tbody>
    ]], layout.header, table.concat(
        (function() 
            local t = {} 
            for _, h in ipairs(headers) do table.insert(t, "<th>" .. h .. "</th>") end 
            return t 
        end)(), ""))

    local row_count = (layout and layout.row_count) or 20
    for i = 1, row_count do
        html = html .. "<tr>"
        html = html .. '<td></td>' -- PAID column
        for j = 2, #headers do
            html = html .. "<td></td>"
        end
        html = html .. "</tr>"
    end
    
    return html .. "</tbody></table><footer>NOTES:</footer></main>"
end
