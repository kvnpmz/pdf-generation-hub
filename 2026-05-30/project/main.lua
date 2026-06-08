local SIZES = {
    {w = 612, h = 792, name = "letter"},
    {w = 595, h = 842, name = "a4"}
}

local config = {
    page = {margin_v = 36, margin_h = 50.4},
    style = {font = "Inter", header_size = "24pt", row_size = "10pt", header_color = "#C2185B", letter_spacing = 2.0}, 
    layout = {header_h = 27, sub_h = 15, gap = 8, col_gap = 20, sec_header_h = 25}
}

local function render_planner(size)
    local t0 = os.clock()
    config.page.W = size.w
    config.page.H = size.h
    
    local W, H = config.page.W, config.page.H
    local MARGIN_V, MARGIN_H = config.page.margin_v, config.page.margin_h
    local HEADER_H = config.layout.header_h
    local SEC_HEADER_H = config.layout.sec_header_h
    local GAP, COLUMN_GAP = config.layout.gap, config.layout.col_gap

    local USABLE_WIDTH = W - (MARGIN_H * 2)
    local TOTAL_TABLE_WIDTH = (USABLE_WIDTH - COLUMN_GAP) / 2
    local LABEL_W = TOTAL_TABLE_WIDTH * 0.65
    local AMOUNT_W = TOTAL_TABLE_WIDTH * 0.35

    local HEADER_BOTTOM = MARGIN_V + HEADER_H
    local SUBHEADER_H = 13
    local SUBHEADER_BASELINE = HEADER_BOTTOM + SUBHEADER_H + GAP 

    local left_col = { {t="Income", r=6}, {t="Bills", r=15}, {t="Savings", r=6} }
    local right_col = { {t="Expenses", r=15}, {t="Debt", r=6}, {t="Summary", r=6} }
    local summary_labels = {"Income", "Bills", "Savings", "Expenses", "Debt", ""}

    local function get_row_h(sections, total_rows)
        local usable_h = H - (MARGIN_V * 2) - HEADER_H - GAP - SUBHEADER_H - GAP
        return (usable_h - (#sections * SEC_HEADER_H) - ((#sections - 1) * GAP)) / total_rows
    end

    local function count_total_rows(sections)
        local total = 0
        for _, s in ipairs(sections) do
            total = total + s.r
        end
        return total
    end

    local TOTAL_ROWS_LEFT = count_total_rows(left_col)
    local ROW_H1 = get_row_h(left_col, TOTAL_ROWS_LEFT)

    local TOTAL_ROWS_RIGHT = count_total_rows(right_col)
    local ROW_H2 = get_row_h(right_col, TOTAL_ROWS_RIGHT)

    local svg = {string.format('<svg xmlns="http://www.w3.org/2000/svg" width="%fpt" height="%fpt" viewBox="0 0 %f %f">',W, H, W, H)}
    table.insert(svg, '<rect width="100%" height="100%" fill="white"/>')

    local function add_text(x, y, text, opts)
        opts = opts or {} 
        
        local font = config.style.font
        local size = opts.size or config.style.row_size
        local weight = opts.weight or "bold"
        local fill = opts.color or "#121212"
        local anchor = opts.anchor or "start"
        local spacing = config.style.letter_spacing
        
        local element = string.format(
            '<text x="%f" y="%f" font-family="%s" font-size="%s" font-weight="%s" fill="%s" text-anchor="%s" letter-spacing="%f">%s</text>',
            x, y, font, size, weight, fill, anchor, spacing, text
        )

        table.insert(svg, element)
    end

    local function add_rect(x, y, w, h, opts)
        opts = opts or {}
        local fill = opts.fill or "none"
        local stroke = opts.stroke or "#121212"
        local width = opts.width or "1"

        local style = string.format('fill="%s" stroke="%s" stroke-width="%s"', fill, stroke, width)
        table.insert(svg, string.format('<rect x="%f" y="%f" width="%f" height="%f" %s/>', x, y, w, h, style))
    end

    local function add_line(x1, y1, x2, y2, opts)
        opts = opts or {}
        local stroke = opts.stroke or "#121212"
        local width = opts.width or "1"
        
        table.insert(svg, string.format(
            '<line x1="%f" y1="%f" x2="%f" y2="%f" stroke="%s" stroke-width="%s"/>',
            x1, y1, x2, y2, stroke, width
        ))
    end

    add_text(
        config.page.W / 2, 
        config.page.margin_v + config.layout.header_h, 
        string.upper("Budget Planner"), 
        { 
            size = config.style.header_size,
            weight = "900",
            color = config.style.header_color,
            anchor = "middle"
        }
    )

    add_text(
        MARGIN_H, 
        SUBHEADER_BASELINE, 
        string.upper("Month:")
    )

    add_line(
        MARGIN_H + 69, SUBHEADER_BASELINE, 
        MARGIN_H + TOTAL_TABLE_WIDTH, SUBHEADER_BASELINE
    )

    add_text(
        MARGIN_H + TOTAL_TABLE_WIDTH + COLUMN_GAP, 
        SUBHEADER_BASELINE, 
        string.upper("Year:")
    )

    add_line(
        MARGIN_H + TOTAL_TABLE_WIDTH + COLUMN_GAP + 52, SUBHEADER_BASELINE, 
        MARGIN_H + (TOTAL_TABLE_WIDTH * 2) + COLUMN_GAP, SUBHEADER_BASELINE
    )

    local function render_table(x, y, title, rows_data, rowH)
        local is_array = type(rows_data) == "table"
        local row_count = is_array and #rows_data or rows_data
        
        add_rect(x, y, LABEL_W, SEC_HEADER_H)
        add_text(x + LABEL_W / 2, y + 17.5, string.upper(title), { anchor = "middle" })
        add_rect(x + LABEL_W, y, AMOUNT_W, SEC_HEADER_H)
        add_text(x + LABEL_W + AMOUNT_W / 2, y + 17.5, string.upper("Amount"), { anchor = "middle" })

        local cur_y = y + SEC_HEADER_H
        for i = 1, row_count do
            local is_last = (i == row_count)
            
            add_line(x, cur_y, x + LABEL_W, cur_y) 
            
            add_line(x + LABEL_W, cur_y, x + LABEL_W, cur_y + rowH) 
            
            if not is_last then
                add_line(x, cur_y, x, cur_y + rowH)
            end
            
            if not is_last then
                add_line(x, cur_y + rowH, x + LABEL_W, cur_y + rowH)
            end

            add_rect(x + LABEL_W, cur_y, AMOUNT_W, rowH)
            
            if is_array then
                local text = string.upper(tostring(rows_data[i]))
                add_text(x + (LABEL_W / 2), cur_y + (rowH/2) + 4, text, { anchor = "middle", weight = "normal" })
            end
            cur_y = cur_y + rowH
        end
        return cur_y
    end

    local table_start_y = MARGIN_V + HEADER_H + GAP + SUBHEADER_H + GAP
    local y1, y2 = table_start_y, table_start_y

    for _, s in ipairs(left_col) do y1 = render_table(MARGIN_H, y1, s.t, s.r, ROW_H1) + GAP end
    for _, s in ipairs(right_col) do 
        local data = (s.t == "Summary") and summary_labels or s.r
        y2 = render_table(MARGIN_H + TOTAL_TABLE_WIDTH + COLUMN_GAP, y2, s.t, data, ROW_H2) + GAP 
    end

    local svg_name = "budget_planner_" .. size.name .. ".svg"
    local pdf_name = "budget_planner_" .. size.name .. ".pdf"

    table.insert(svg, "</svg>")
    local f = io.open(svg_name, "w")
    f:write(table.concat(svg, "\n"))
    f:close()

    os.execute("inkscape " .. svg_name .. " --export-filename=" .. pdf_name .. " > /dev/null 2>&1")
    os.remove(svg_name)
    local t1 = os.clock()

    print(string.format("Execution time for %s: %.5f seconds", size.name, t1 - t0))
end

for _, s in ipairs(SIZES) do
    render_planner(s)
end
