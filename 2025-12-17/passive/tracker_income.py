from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.lib.colors import black, Color
from reportlab.lib.units import inch
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

def create_passive_income_tracker(filename="passive_income_tracker.pdf"):
    c = canvas.Canvas(filename, pagesize=letter)
    width, height = letter

    # --- Global Font Settings (uniform everywhere) ---
    FONT_NAME = "LiberationSans"
    FONT_SIZE = 16

    # --- Colors ---
    LIGHT_BLUE = Color(147/255, 203/255, 237/255)
    DARK_TEXT_BLUE = Color(47/255, 126/255, 174/255)

    # --- Coordinates ---
    MARGIN = 0.4 * inch
    INNER_WIDTH = width - 2 * MARGIN
    
    HEADER_TOP = height - 0.65 * inch
    HEADER_Y = height - 1.5 * inch
    
    TABLE_START_Y = HEADER_Y - 0.5 * inch
    TABLE_END_Y = MARGIN + 0.5 * inch
    TABLE_HEIGHT = TABLE_START_Y - TABLE_END_Y
    NUM_ROWS = 15

    # Column widths
    COL1_W = INNER_WIDTH * 0.45
    COL2_W = INNER_WIDTH * 0.275
    COL3_W = INNER_WIDTH * 0.275

    COL1_X = MARGIN
    COL2_X = COL1_X + COL1_W
    COL3_X = COL2_X + COL2_W
    COL_END_X = COL3_X + COL3_W

    # --- Title (still same font size; just colored) ---
    c.setFont(FONT_NAME, 28)
    c.setFillColor(DARK_TEXT_BLUE)
    c.drawCentredString(width / 2, HEADER_TOP, "PASSIVE INCOME")

    c.setFont(FONT_NAME, 22)
    c.setFillColor(LIGHT_BLUE)
    c.drawCentredString(width / 2, HEADER_TOP - 0.3 * inch, "Tracker")

    c.setFont(FONT_NAME, FONT_SIZE)

    # --- Daily / Monthly / Date ---
    c.setFillColor(black)
    c.drawString(MARGIN, HEADER_Y + 0.1 * inch, "DAILY TOTAL")

    c.setFillColor(LIGHT_BLUE)
    c.rect(MARGIN, HEADER_Y - 0.3 * inch, INNER_WIDTH * 0.5, 0.3 * inch, fill=1, stroke=0)

    line_spacing = 0.15 * inch

    c.setFillColor(black)
    c.drawString(MARGIN + INNER_WIDTH * 0.55, HEADER_Y + line_spacing, "MONTHLY TOTAL")
    c.line(MARGIN + INNER_WIDTH * 0.81, HEADER_Y + line_spacing, COL_END_X, HEADER_Y + line_spacing)

    c.drawString(MARGIN + INNER_WIDTH * 0.55, HEADER_Y - line_spacing, "DATE")
    c.line(MARGIN + INNER_WIDTH * 0.65, HEADER_Y - line_spacing, COL_END_X, HEADER_Y - line_spacing)

    # --- Table Header Bars ---
    c.setFillColor(LIGHT_BLUE)
    HEADER_BAR_H = 0.25 * inch
    BAR_Y = TABLE_START_Y - HEADER_BAR_H

    # --- Table Header Text (same font size, no bold) ---
    c.setFillColor(black)
    header_y = TABLE_START_Y - 0.15 * inch

    # Project left aligned
    c.drawString(COL1_X + 5, header_y, "PROJECT")

    # Helper for centering
    def draw_centered(text, x_start, width, y):
        c.drawCentredString(x_start + width / 2, y, text)

    draw_centered("TOTAL INVESTED", COL2_X, COL2_W, header_y)
    draw_centered("DAILY PROFIT", COL3_X, COL3_W, header_y)

    # --- Table Grid ---
    c.setStrokeColor(LIGHT_BLUE)
    c.setLineWidth(1.8)
    c.line(COL1_X, BAR_Y, COL_END_X, BAR_Y)

    c.setStrokeColor(black)
    c.setLineWidth(1)
    # Horizontal grid lines (internal only)
    ROW_HEIGHT = (BAR_Y - TABLE_END_Y) / NUM_ROWS
    for i in range(1, NUM_ROWS):  # skip first (top) and last (bottom) line
        y = BAR_Y - i * ROW_HEIGHT
        c.line(COL1_X, y, COL_END_X, y)

    # Vertical grid lines (internal only)
    c.line(COL2_X, TABLE_END_Y, COL2_X, BAR_Y)  # internal
    c.line(COL3_X, TABLE_END_Y, COL3_X, BAR_Y)  # internal

    c.line(COL1_X, TABLE_END_Y, COL_END_X, TABLE_END_Y)

    c.line(COL2_X, TABLE_END_Y, COL2_X, BAR_Y)
    c.line(COL3_X, TABLE_END_Y, COL3_X, BAR_Y)

    # --- Bottom Text ---

    c.setStrokeColor(LIGHT_BLUE)
    c.setLineWidth(1.8)
    c.line(MARGIN, MARGIN + 0.3 * inch, COL_END_X, MARGIN + 0.3 * inch)
    c.drawString(MARGIN, MARGIN, "INVESTING PROFIT IN:")
    c.line(MARGIN, MARGIN - 0.15 * inch, COL_END_X, MARGIN - 0.15 * inch)

    c.showPage()
    c.save()

# Run
create_passive_income_tracker()
print("PDF 'passive_income_tracker.pdf' created successfully.")

