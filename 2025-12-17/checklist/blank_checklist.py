from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import letter
from reportlab.lib.units import inch
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

def create_list_pdf(filename):
    c = canvas.Canvas(filename, pagesize=letter)
    width, height = letter

    # --- Configuration ---
    margin = 0.7 * inch
    line_height = 0.37 * inch
    num_lines = 25 # Number of lines per column, adjust as needed

    # --- Page Header and Info ---
    c.setFont('LiberationSans-Bold', 18)
    c.drawString(margin, height - margin, "LIST")

    c.setFont('LiberationSans', 10)
    date_label_x = width - margin - 1.5 * inch
    c.drawString(date_label_x, height - margin - 0.1 * inch, "DATE:")
    c.line(date_label_x + 0.45 * inch, height - margin - 0.1 * inch, width - margin, height - margin - 0.1 * inch)

    # --- Draw Columns ---
    
    # Starting Y position for the first line
    start_y = height - margin - 0.75 * inch

    # Function to draw a column
    def draw_column(x_start):
        # Draw lines and circles
        for i in range(num_lines):
            y_pos = start_y - i * line_height
            circle_offset = 0.18 * inch 

            # Draw the line
            line_x_end = x_start + (width / 2) - margin - 0.2 * inch # Adjust line length
            if i == 0:
                c.line( x_start, y_pos, line_x_end, y_pos)
            else: 
                c.line( x_start + 0.2 * inch, y_pos, line_x_end, y_pos)
                # Draw the circle/bullet
                radius = 0.08 * inch
                c.circle(x_start + radius, y_pos + circle_offset - radius / 2, radius, stroke=1, fill=0)

    # Column 1 (Left)
    col1_x_start = margin
    draw_column(col1_x_start)

    # Column 2 (Right)
    col2_x_start = width / 2 + 0.1 * inch
    draw_column(col2_x_start)

    # --- Save PDF ---
    c.save()

# Example usage:
create_list_pdf("empty_checklist.pdf")

print("PDF 'empty_checklist.pdf' created successfully.")
