from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.lib.units import inch
from reportlab.lib import colors
from reportlab.pdfbase.pdfmetrics import stringWidth
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

def create_habit_tracker_pdf(filename="weekly_habit.pdf"):
    c = canvas.Canvas(filename, pagesize=letter)
    # Set all drawing elements to black

    page_width, page_height = letter

    # Define margins and block size
    margin = 0.5 * inch
    block_width = (page_width - 3 * margin) / 2
    block_height = (page_height - 5 * margin) / 4
    top_offset = -0.55 * inch
    top_anchor = page_height - margin - block_height + top_offset
    row_gap = 0.35 * inch   #  reduce this to tighten vertical spacing

    # Text and line specifications
    line_spacing = 0.3 * inch
    circle_radius = 0.08 * inch
    
    # Function to draw a single habit tracker block
    def draw_tracker_block(x_start, y_start):
        # Title/Header
        c.setFont("LiberationSans-Bold", 14)
        c.setStrokeColor(colors.HexColor("#1A1A1A"))
        c.setFillColor(colors.HexColor("#1A1A1A"))
        c.drawString(x_start, y_start + block_height - 0.2 * inch, "HABITS")
        c.setFillColor(colors.HexColor("#0562A4"))
        days = "M T W T F S S"
        circle_x_start = x_start + block_width * 0.55
        circle_spacing = 2 * circle_radius + 0.1 * inch
        for i, day in enumerate(days.split()):
            c.drawString(circle_x_start - 0.05 * inch + i * circle_spacing - 3, y_start + block_height - 0.2 * inch, day)

        # c.drawString(x_start + block_width - 2.5 * inch, y_start + block_height - 0.2 * inch,)
        
        # Habit lines
        c.setFont("LiberationSans", 9)
        line_vertical_offset = -0.1 * inch

        for i in range(4):
            line_y = y_start + block_height - 0.6 * inch - i * line_spacing + line_vertical_offset
            # Draw line for habit name
            c.line(x_start, line_y, x_start + block_width * 0.45, line_y)

            # Draw circles for days of the week
            circle_y_start = y_start + block_height - 0.5 * inch - i * line_spacing * 1.05
            for j in range(7):
                circle_x = circle_x_start + j * circle_spacing
                c.circle(circle_x, circle_y_start - circle_radius, circle_radius, fill=0) # 0 for no fill

        # Date line
        c.setFillColor(colors.HexColor("#1A1A1A"))
        c.drawString(x_start, y_start + 0.1 * inch, "DATE:")
        c.line(x_start + 0.5 * inch, y_start + 0.1 * inch, x_start + block_width * 0.45, y_start + 0.1 * inch)


    # Draw the 8 blocks (4 rows, 2 columns)
    for row in range(4):
        y_start = top_anchor - row * (block_height + row_gap)

        # Left column block
        x_start_left = margin
        draw_tracker_block(x_start_left, y_start)

        # Right column block
        x_start_right = margin * 2 + block_width
        draw_tracker_block(x_start_right, y_start)
    # for row in range(4):
        # vertical_offset = -0.5 * inch
        # y_start = margin + row * (block_height + margin) + vertical_offset
        
        # # # Left column block
        # x_start_left = margin
        # draw_tracker_block(x_start_left, y_start)
        
        # # # Right column block
        # x_start_right = margin * 2 + block_width
        # draw_tracker_block(x_start_right, y_start)

    # Main Page Title
    font_name = "LiberationSans-Bold"
    c.setFillColor(colors.HexColor("#034473"))
    c.setFont(font_name, 28)
    title_text = "WEEKLY HABIT TRACKER"

    title_width = stringWidth(title_text, font_name, 28)
    c.drawString((page_width - title_width) / 2, page_height - 50, title_text)

    c.save()

if __name__ == '__main__':
    create_habit_tracker_pdf()
    print("PDF 'weekly_habit.pdf' generated.")
