from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.lib import colors
from reportlab.platypus import Table, TableStyle

def create_budget_planner(filename):
    c = canvas.Canvas(filename, pagesize=letter)
    width, height = letter

    # --- Header ---
    c.setFont("Helvetica-Bold", 26)   # Smaller headline
    c.drawCentredString(width / 2, height - 50, "Budget Planner")
    
    c.setFont("Helvetica", 10)
    c.drawString(100, height - 80, "MONTH: _________________")
    c.drawString(350, height - 80, "YEAR: _________________")

    # Helper: draw tables with tighter row spacing
    def draw_grid_table(title, total_rows, x, y, w, h_row=14):
        c.setFont("Helvetica-Bold", 9)
        
        # We'll draw total_rows - 1 in the table
        main_rows = total_rows - 1

        data = [[title.upper(), 'AMOUNT']] + [['', ''] for _ in range(main_rows)]
        col_widths = [w * 0.65, w * 0.35]
        t = Table(data, colWidths=col_widths, rowHeights=h_row)
        
        style = TableStyle([
            ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
            ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, -1), 8),
            ('BOX', (0, 0), (-1, -1), 1.25, colors.black),
        ])
        t.setStyle(style)

        # Draw main table
        t.wrapOn(c, x, y)
        table_height = h_row * len(data)
        t.drawOn(c, x, y - table_height)

        # Draw 18th row as small box on the right
        last_box_x = x + col_widths[0]  # right column
        last_box_y = y - table_height - h_row  # right below table
        last_box_w = col_widths[1]
        last_box_h = h_row

        c.rect(last_box_x, last_box_y, last_box_w, last_box_h, fill=0, stroke=1)

    # --- Left Column Tables ---
    draw_grid_table("Income",   5,  80, height - 110, 200, h_row=18)
    draw_grid_table("Bills",   18,  80, height - 240, 200, h_row=18)
    draw_grid_table("Savings",  4,  80, height - 620, 200, h_row=18)

    # --- Right Column Tables ---
    draw_grid_table("Expenses", 18, 330, height - 110, 200, h_row=18)
    draw_grid_table("Debt",      6, 330, height - 470, 200, h_row=18)

    # --- Summary Table ---
    summary_data = [
        ['SUMMARY', 'AMOUNT'],
        ['INCOME', ''],
        ['BILLS', ''],
        ['SAVINGS', ''],
        ['EXPENSES', ''],
        ['DEBT', '']
    ]

    st = Table(summary_data, colWidths=[130, 70], rowHeights=16)
    st.setStyle(TableStyle([
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, -1), 8),
        ('GRID', (0, 0), (-1, -1), 1, colors.black),  # <-- full gridlines
    ]))

    st.wrapOn(c, 330, height - 620)
    st.drawOn(c, 330, height - 620 - (16 * 6))

    # (Removed gray border completely)

    c.save()


if __name__ == "__main__":
    create_budget_planner("budget_planner.pdf")
    print("PDF generated: budget_planner.pdf")

