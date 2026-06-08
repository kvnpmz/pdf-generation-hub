local SIZES = {
    {w = 612, h = 792, name = "letter"},
    {w = 595, h = 842, name = "a4"}
}

local config = {
    page = { margin_v = 36, margin_h = 50.4 },
    style = {
        font = "Inter",
        letter_spacing = 2.0,
        header_size = 30,
        title_size = 12,
        body_size = 10,
        primary_color = "#121212",
        bg_color = "#FFFFFF"
    },
    layout = {
        months_h = 15,
        main_header_h = 37,
        income_h = 80,
        gap = 20,
        col_gap = 20,
        col_widths = { due = 40, paid = 36, default = 60, summary = 90 }
    }
}

local Canvas = {}
Canvas.__index = Canvas

function Canvas.new(svg_table)
    return setmetatable({
        svg = svg_table,
        cursor_y = 0
    }, Canvas)
end

function Canvas:add_text(x, y, text, opts)
    opts = opts or {}
    local family = opts.family or config.style.font
    local raw_size = opts.size or config.style.body_size
    local size = tostring(raw_size) .. "pt"
    local color  = opts.color  or config.style.primary_color
    local weight = opts.weight or "bold"
    local anchor = opts.anchor or "start"
    local spacing = config.style.letter_spacing
    
    table.insert(self.svg, string.format(
        '<text x="%f" y="%f" font-family="%s" font-size="%s" font-weight="%s" text-anchor="%s" fill="%s" letter-spacing="%f">%s</text>',
        x, y, family, size, weight, anchor, color, spacing, text
    ))
end

function Canvas:add_rect(x, y, w, h, opts)
    opts = opts or {}
    local fill = opts.fill or "none"
    local stroke = opts.stroke or "#121212"
    local width = opts.width or "1"
    
    table.insert(self.svg, string.format(
        '<rect x="%f" y="%f" width="%f" height="%f" fill="%s" stroke="%s" stroke-width="%s"/>',
        x, y, w, h, fill, stroke, width
    ))
end

function Canvas:add_line(x1, y1, x2, y2, opts)
    opts = opts or {}
    local stroke = opts.stroke or "#121212"
    local width = opts.width or "1"
    
    table.insert(self.svg, string.format(
        '<line x1="%f" y1="%f" x2="%f" y2="%f" stroke="%s" stroke-width="%s"/>',
        x1, y1, x2, y2, stroke, width
    ))
end

function Canvas:render_row(x, y, row_height, widths, paddings, label)
    local cur_x = x
    
    if label then
        self:add_text(cur_x, y + (row_height/2) + 4, label, {weight = "normal"})
    end

    for i, w in ipairs(widths) do
        local p = paddings and paddings[i] or {h = 2, v = 2}
        local box_x = cur_x + p.h
        local box_y = y + p.v
        local box_w = w - (p.h * 2)
        local box_h = row_height - (p.v * 2)
        
        self:add_rect(box_x, box_y, box_w, box_h)
        cur_x = cur_x + w
    end
end

function Canvas:add_footer_line(x, y, width, padding)
    self:add_line(x + padding, y, x + width, y)
end

-- Renders a tracker component (Grid with Header, Data Rows, and Footer)
function Canvas:render_tracker(x, y, title, columns, data_rows, total_section_height, footer_label, col_width, bg_color)
    local total_slots = 1 + 1 + data_rows + 1 -- Title + ColumnHeaders + Data + Footer
    local row_height = total_section_height / total_slots
    local grid_w = self:get_sum_widths(columns)
    
    -- 1. Draw Background if color provided
    if bg_color then
        self:add_rect(x, y, col_width, total_section_height, {fill = bg_color, stroke = "none"})
    end

    local label_area_w = col_width - grid_w - 10 
    
    local col_start_x = x + label_area_w
    
    self:add_text(x, y + 10, title:upper())
    
    -- 2. Render Column Headers
    local header_y = y + row_height
    local cur_x = col_start_x
    for _, col in ipairs(columns) do
        self:add_text(cur_x + col.padding.h, header_y + (row_height/2) + 4, col.header:upper())
        cur_x = cur_x + col.width
    end
    
    -- 3. Render Data Rows
    local cur_y = header_y + row_height
    for i = 1, data_rows do
        self:add_line(x, cur_y + row_height - 2, col_start_x - 5, cur_y + row_height - 2)
        
        local cur_x = col_start_x
        for _, col in ipairs(columns) do
            local box_x = cur_x + col.padding.h
            local box_y = cur_y + col.padding.v
            local box_w = col.width - (col.padding.h * 2)
            local box_h = row_height - (col.padding.v * 2)
            self:add_rect(box_x, box_y, box_w, box_h)
            cur_x = cur_x + col.width
        end
        cur_y = cur_y + row_height
    end

    -- 4. Render Footer
    if footer_label then
        local total_width = 0
        for _, col in ipairs(columns) do total_width = total_width + col.width end
        
        self:add_text(col_start_x + total_width - 60, cur_y + (row_height/2) + 4, footer_label:upper(), {anchor = "end"})
        self:add_rect(col_start_x + total_width - 60, cur_y + 2, 60, row_height - 4, {width = "1.5"})
    end
    
    return y + total_section_height
end

function Canvas:render_tracker_row(x, y, row_height, columns, col_start_x)
    local cur_x = col_start_x
    
    self:add_line(x, cur_y + row_height - 2, col_start_x - 5, cur_y + row_height - 2)

    for _, col in ipairs(columns) do
        local box_x = cur_x + col.padding.h
        local box_y = y + col.padding.v
        local box_w = col.width - (col.padding.h * 2)
        local box_h = row_height - (col.padding.v * 2)
        
        self:add_line(x, y + row_height - 2, col_start_x - 5, y + row_height - 2)
        cur_x = cur_x + col.width
    end
end

function Canvas:get_sum_widths(columns)
    local sum = 0
    for _, c in ipairs(columns) do sum = sum + c.width end
    return sum
end

function Canvas:get_offset_for_col(columns, index)
    local offset = 0
    for i = index, #columns do
        offset = offset + columns[i].width
    end
    return offset
end

function Canvas:render_summary(x, y, height)
    local cur_y = y
    self:add_text(x, cur_y + 15, "BUDGET SUMMARY", {weight = "900"})
    
    local labels = {"Total Income", "Total Bills", "Total Spending", "Total Savings", "Total Debt"}
    for _, label in ipairs(labels) do
        cur_y = cur_y + height
        self:add_text(x, cur_y, label:upper(), {size = 10})
        self:add_rect(x + 100, cur_y - 10, 90, 15)
    end
end

function Canvas:render_months_header(x, y, width)
    local months = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"}
    local count = #months
    
    -- Assuming a fixed character width/spacing based on your config
    -- (3 chars * width of one char)
    local char_width = config.style.body_size * 0.6 
    local item_width = 3 * char_width 
    
    -- Total width taken by all items combined
    local total_text_w = count * item_width
    
    -- Calculate the gap between items
    -- There are 11 gaps for 12 items
    local gap_size = (width - total_text_w) / (count - 1)
    
    local cur_x = x
    for _, m in ipairs(months) do
        -- Use anchor="start" so the left side of the text hits cur_x
        self:add_text(cur_x, y, m, {anchor = "start", weight = "bold"})
        
        -- Move the cursor by the width of the item plus the calculated gap
        cur_x = cur_x + item_width + gap_size
    end
end

function Canvas:render_main_title(x, y, title, width)
    -- 1. Main Title
    self:add_text(x, y + 25, title:upper(), {size = 24, weight = "900"})
    
    -- 2. Date Box (Positioned to the right)
    local box_w = 80
    local box_h = 30
    local box_x = x + width - box_w
    
    self:add_rect(box_x, y, box_w, box_h)
    self:add_text(box_x + (box_w/2), y + 18, "DATE", {anchor="middle"})
end

-- Renders a single row in the income section
function Canvas:render_income_row(x, y, row_height, label, line_padding)
    local col_default_w = 60
    
    self:add_text(x, y + (row_height/2) + 4, label:upper(), {size = "10pt", weight = "normal"})
    
    -- Draw two boxes (Budget and Actual)
    local box_x = x + 400 -- Adjust based on your layout width
    for i = 1, 2 do
        self:add_rect(box_x + ((i-1) * col_default_w), y + 2, col_default_w - 4, row_height - 4)
    end
    
    -- Draw the underline
    self:add_line( x + line_padding, y + row_height - 2, box_x - 10, y + row_height - 2)
end

function Canvas:render_income_section(x, y, width, height)
    local row_h = height / 4 -- Assuming 4 rows total (Header + 2 data + Total)
    
    -- 1. Header Row
    self:add_text(x, y + 10, "SOURCES OF INCOME")
    self:add_text(x + 410, y + 10, "BUDGET", {anchor = "middle"})
    self:add_text(x + 470, y + 10, "ACTUAL", {anchor = "middle"})
    
    -- 2. Data Rows
    self:render_income_row(x, y + row_h, row_h, "Paycheck Amount", 110)
    self:render_income_row(x, y + (row_h * 2), row_h, "Other Income Source", 125)
    
    -- 3. Total Footer
    local footer_y = y + (row_h * 3)
    self:add_text(x + 400, footer_y + 15, "TOTAL INCOME", {anchor = "end"})

    -- Footer Boxes
    for i = 1, 2 do
        self:add_rect(x + 405 + ((i-1) * 60), footer_y + 2, 56, row_h - 4, {width="1.5"})
    end
end

function render_left_column(canvas, x, y, total_h, gap)
    -- 1. Calculate the budget for the children
    local available_h = total_h - gap
    local bills_h = available_h * 0.5
    local spending_h = available_h * 0.5
    
    -- 2. Apply "Parent" logic (The Correction)
    -- If the math is slightly off, adjust the last child to fill the parent
    local diff = available_h - (bills_h + spending_h)
    spending_h = spending_h + diff

    -- 3. Pass the "Allocated" heights to the children
    -- Now the children have no choice but to be exactly this size
    local next_y = canvas:render_tracker(x, y, "Bills", bills_cols, 10, bills_h)
    next_y = next_y + gap
    canvas:render_tracker(x, next_y, "Spending", spending_cols, 10, spending_h)
end

local bills_cols = {
    {header = "Due", width = 40, padding = {h = 2, v = 2}},
    {header = "Amount", width = 60, padding = {h = 2, v = 2}},
    {header = "Paid", width = 36, padding = {h = 10, v = 4}}
}

local spending_cols = {
    {header = "Budget", width = 60, padding = {h = 2, v = 2}},
    {header = "Actual", width = 60, padding = {h = 2, v = 2}}
}

local savings_cols = {
    {header = "Amount", width = 60, padding = {h = 2, v = 2}}
}

local debt_cols = {
    {header = "Amount", width = 60, padding = {h = 2, v = 2}}
}

function generate_document(size)
    local svg = {string.format('<svg xmlns="http://www.w3.org/2000/svg" width="%fpt" height="%fpt" viewBox="0 0 %f %f">', size.w, size.h, size.w, size.h)}
    local canvas = Canvas.new(svg)
    
    local MARGIN_H = config.page.margin_h
    local MARGIN_V = config.page.margin_v
    
    local content_width = size.w - (MARGIN_H * 2)
    local cur_y = MARGIN_V
    
    -- 1. Months Header
    canvas:render_months_header(MARGIN_H, cur_y, content_width)
    cur_y = cur_y + config.layout.months_h + config.layout.gap
    
    -- 2. Main Title (Missing part)
    canvas:render_main_title(MARGIN_H, cur_y, "PAYCHECK BUDGET", content_width)
    cur_y = cur_y + config.layout.main_header_h + config.layout.gap
    
    -- 3. Income Section (Missing part)
    canvas:render_income_section(MARGIN_H, cur_y, content_width, config.layout.income_h)
    cur_y = cur_y + config.layout.income_h + config.layout.gap

    -- 3. The Two-Column Layout
    local col_width = (size.w - (MARGIN_H * 2) - config.layout.col_gap) / 2
    local left_x = MARGIN_H
    local right_x = MARGIN_H + col_width + config.layout.col_gap
    
    -- THIS IS THE KEY: Calculate exactly how much space is left
    local page_bottom = size.h - MARGIN_V
    local remaining_h = page_bottom - cur_y
    
    -- 1. Split remaining_h into two major columns
    local left_column_total_h = remaining_h
    local right_column_total_h = remaining_h


    -- 3. Define ratios for the Right Column (3 components: Savings, Debts, Summary)
    -- We subtract 2 gaps (between 3 items) from the total height
    local right_available_h = right_column_total_h - (config.layout.gap * 2)
    local savings_h = right_available_h * 0.3
    local debts_h = right_available_h * 0.3
    local summary_h = right_available_h * 0.4

    -- 2. Define heights for Left Column (Total 1.0)
    -- We divide the available space equally between the two trackers
    local bills_h = left_column_total_h * 0.5 - (config.layout.gap / 2)
    local spending_h = left_column_total_h * 0.5 - (config.layout.gap / 2)

    -- 4. Render Left Column
    local left_y = cur_y
    left_y = canvas:render_tracker(left_x, left_y, "Bills", bills_cols, 10, bills_h, "Total Bills Paid", col_width, "#E0E0E0")
    left_y = left_y + config.layout.gap
    left_y = canvas:render_tracker(left_x, left_y, "Spending", spending_cols, 10, spending_h, "Total Spending", col_width, "#B1BEB1")

    -- 5. Render Right Column
    local right_y = cur_y
    right_y = canvas:render_tracker(right_x, right_y, "Savings", savings_cols, 6, savings_h, "Total Savings", col_width, "#C1C1C1")
    right_y = right_y + config.layout.gap
    right_y = canvas:render_tracker(right_x, right_y, "Debts", debt_cols, 6, debts_h, "Total Debt Paid", col_width, "#D1D1D1")
    right_y = right_y + config.layout.gap
    canvas:render_summary(right_x, right_y, summary_h / 6)

    -- Inside generate_document, right after defining remaining_h:
    print(string.format("DEBUG: Remaining Height Available: %.2f", remaining_h))
    print(string.format("DEBUG: Cur_Y start: %.2f", cur_y))

    -- After rendering the Left Column:
    print(string.format("DEBUG: Left Column Calculation - Bills: %.2f, Spending: %.2f", bills_h, spending_h))
    print(string.format("DEBUG: Left Column Rendered end: %.2f", left_y))

    -- After rendering the Right Column:
    print(string.format("DEBUG: Right Column Calculation - Savings: %.2f, Debts: %.2f, Summary: %.2f", savings_h, debts_h, summary_h))
    print(string.format("DEBUG: Right Column Rendered end: %.2f", right_y))
    local page_bottom = size.h - config.page.margin_v

    print(string.format("DEBUG: Page bottom: %.2f", page_bottom))
    print(string.format("DEBUG: Spending ends at: %.2f", left_y))
    print(string.format("DEBUG: Gap to bottom: %.2f", page_bottom - left_y))
    print(string.format("DEBUG: Total Vertical Consumption: %.2f", (left_y - MARGIN_V)))
    print(string.format("DEBUG: Total Page Budget: %.2f", (size.h - (MARGIN_V * 2))))
    table.insert(svg, "</svg>")
    
    -- Write file and export via Inkscape as in your prototype
    local f = io.open("budget_" .. size.name .. ".svg", "w")
    f:write(table.concat(svg, "\n"))
    f:close()
    
    os.execute("inkscape budget_" .. size.name .. ".svg --export-filename=budget_" .. size.name .. ".pdf")
end

for _, size in ipairs(SIZES) do
    print("Generating: " .. size.name)
    generate_document(size)
end

print("All documents generated successfully.")

