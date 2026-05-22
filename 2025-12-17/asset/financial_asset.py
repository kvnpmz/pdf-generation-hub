from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.pdfbase.pdfmetrics import stringWidth
from reportlab.lib import colors
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

filename = "assets_financial.pdf"
# Create PDF
c = canvas.Canvas(filename, pagesize=letter)
width, height = letter

font_name = "LiberationSans-Bold"

# Title
c.setFillColor(colors.HexColor("#143D87"))
c.setFont(font_name, 24)
title_text = "FINANCIAL ASSETS TEMPLATE"

title_width = stringWidth(title_text, font_name, 24)
c.drawString((width - title_width) / 2, height - 50, title_text)

font_size = 16

# Function to draw a section
def draw_section(title, y_start):
    c.setFillColor(colors.HexColor("#0D0D0D"))
    c.setFont(font_name, font_size)
    c.drawString(50, y_start, title)

    text_width = stringWidth(title, font_name, font_size)

    # Draw colored underline (e.g., blue)
    underline_y = y_start - 3  # slightly below the text
    c.setStrokeColor(colors.HexColor("#5085E5"))  # underline color
    c.setLineWidth(2)
    c.line(50, underline_y, 50 + text_width, underline_y)  # underline exactly matches text

    c.setStrokeColor(colors.HexColor("#0D0D0D"))
    y = y_start - 20
    for i in range(4):
        # Left column checkbox
        c.circle(50, y, 5)
        # Right column checkbox
        c.circle(350, y, 5)  # <-- Added this line
        # Lines
        c.line(70, y, 300, y)   # left column line
        c.line(370, y, 580, y)  # right column line (shifted a bit to not overlap circle)
        y -= 25
    return y

y_position = height - 100

# Draw sections
y_position = draw_section("ACTIVE STREAMS OF INCOME", y_position)
y_position = draw_section("PASSIVE STREAMS OF INCOME", y_position - 20)
y_position = draw_section("INVESTMENTS", y_position - 20)
y_position = draw_section("SAVINGS", y_position - 20)

# Notes section
notes_title = "NOTES"
c.setFont(font_name, font_size)

notes_width = stringWidth(notes_title, font_name, font_size)
notes_x = (width - notes_width) / 2
notes_y = y_position - 20

# Draw centered NOTES title
c.drawString(notes_x, notes_y, notes_title)

# Centered underline
underline_y = notes_y - 3
c.setStrokeColor(colors.HexColor("#5085E5"))
c.setLineWidth(2)
c.line(notes_x, underline_y, notes_x + notes_width, underline_y)

# Notes lines
c.setStrokeColor(colors.HexColor("#0D0D0D"))
c.setLineWidth(1)
c.setDash(10, 4)

for i in range(4):
    c.line(50, notes_y - 30 - i * 20, 580, notes_y - 30 - i * 20)

c.setDash()

# Save PDF
c.save()
print(f"PDF created successfully: {filename}")

