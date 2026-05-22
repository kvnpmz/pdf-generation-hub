from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import LETTER
from reportlab.lib.colors import HexColor
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

def create_notebook_pdf(output_filename):
    c = canvas.Canvas(output_filename, pagesize=LETTER)
    width, height = LETTER

    # --- 1. Draw the Background Border ---
    # Earthy brown/taupe color for the thick border
    c.setFillColor(HexColor("#FFFFFF"))

    # --- 2. Draw the White Inner Paper ---
    margin = 20
    paper_width = width - (margin * 2)
    paper_height = height - (margin * 2)
    
    # --- 3. Add Title and Date ---
    c.setFillColor(HexColor("#1A1A1A"))
    title_text = "My Daily Checklist"
    c.setFont("LiberationSans-Bold", 30)
    title_y = height - 50
    c.drawCentredString(width / 2 - 100, title_y, title_text)

    c.setFont("LiberationSans", 14)
    date_text = "Date: __________________"
    c.drawString(width - 235, title_y, date_text)

    # --- 4. Draw Checklist Circles and Dashed Lines ---
    line_spacing = 45  # Space between lines
    start_y = height - 100
    left_padding = margin + 30
    circle_radius = 10
    
    c.setStrokeColor(HexColor("#1A1A1A")) # Light gray for lines
    c.setLineWidth(2)

    for i in range(15):  # Adjust number of lines based on spacing
        current_y = start_y - (i * line_spacing)
        
        # Ensure we don't draw off the bottom of the paper
        if current_y < margin:
            break

        # Draw the Circle
        c.setDash([]) # Solid line for circle
        c.circle(left_padding, current_y + 15, circle_radius, stroke=1, fill=0)

        # Draw the Line
        line_start_x = left_padding + 20
        line_end_x = width - margin - 30
        c.line(line_start_x, current_y, line_end_x, current_y)

    # --- 5. Finalize ---
    c.showPage()
    c.save()

if __name__ == "__main__":
    create_notebook_pdf("notebook_template.pdf")
    print("PDF created successfully!")
