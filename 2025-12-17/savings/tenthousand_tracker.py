from reportlab.lib import colors
from reportlab.lib.pagesizes import letter
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph
from reportlab.lib.styles import getSampleStyleSheet
from reportlab.pdfgen import canvas
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

def create_savings_tracker(filename="savings_tracker.pdf"):
    # --- 1. Data Setup ---
    # The grid is split into two halves (Week 1-26 and Week 27-52).
    # We will combine the data structure for easier Table creation.
    # Headers for the two halves
    HEADERS = [
        ("DATE", "WEEK", "SAVE", "BALANCE", "✓"),
        ("DATE", "WEEK", "SAVE", "BALANCE", "✓")
    ]

    # Data for the grid (52 weeks)
    # This must be extracted carefully from your image: (Week, Save, Balance)
    WEEK_DATA_1 = [
        (1, "120.00", "120.00"), (2, "160.00", "280.00"), (3, "180.00", "460.00"), (4, "300.00", "760.00"),
        (5, "120.00", "880.00"), (6, "160.00", "1040.00"), (7, "180.00", "1,220.00"), (8, "300.00", "1,520.00"),
        (9, "120.00", "1,640.00"), (10, "160.00", "1,800.00"), (11, "180.00", "1,980.00"), (12, "300.00", "2,280.00"),
        (13, "120.00", "2,400.00"), (14, "160.00", "2,560.00"), (15, "180.00", "2,740.00"), (16, "300.00", "3,040.00"),
        (17, "120.00", "3,160.00"), (18, "160.00", "3,320.00"), (19, "180.00", "3,500.00"), (20, "300.00", "3,800.00"),
        (21, "120.00", "3,920.00"), (22, "160.00", "4,080.00"), (23, "180.00", "4,260.00"), (24, "300.00", "4,560.00"),
        (25, "120.00", "4,680.00"), (26, "160.00", "4,840.00")
    ]

    WEEK_DATA_2 = [
        (27, "180.00", "5,020.00"), (28, "300.00", "5,320.00"), (29, "120.00", "5,440.00"), (30, "160.00", "5,600.00"),
        (31, "180.00", "5,780.00"), (32, "300.00", "6,080.00"), (33, "120.00", "6,200.00"), (34, "160.00", "6,360.00"),
        (35, "180.00", "6,540.00"), (36, "300.00", "6,840.00"), (37, "120.00", "6,960.00"), (38, "160.00", "7,120.00"),
        (39, "180.00", "7,300.00"), (40, "300.00", "7,600.00"), (41, "120.00", "7,720.00"), (42, "160.00", "7,880.00"),
        (43, "180.00", "8,060.00"), (44, "300.00", "8,360.00"), (45, "140.00", "8,500.00"), (46, "180.00", "8,680.00"),
        (47, "180.00", "8,860.00"), (48, "300.00", "9,160.00"), (49, "180.00", "9,340.00"), (50, "180.00", "9,520.00"),
        (51, "180.00", "9,700.00"), (52, "300.00", "10,000.00")
    ]
    # Combine all data into a single table structure (row-by-row)
    table_data = [HEADERS[0] + HEADERS[1]] # The combined header row

    # Fill the data rows, pairing Week 1 with Week 27, Week 2 with Week 28, etc.
    for i in range(26):
        # Format the numbers as strings for dollar signs and two decimal places
        w1, s1, b1 = WEEK_DATA_1[i]
        w2, s2, b2 = WEEK_DATA_2[i]
        
        row = [
            "",  # Date 1
            str(w1),
            f"${s1}",
            f"${b1}",
            "",  # Checkbox 1
            "",  # Date 2
            str(w2),
            f"${s2}",
            f"${b2}",
            ""   # Checkbox 2
        ]
        table_data.append(row)

    # --- 2. Canvas for Title and Header Lines ---
    c = canvas.Canvas(filename, pagesize=letter)
    width, height = letter

    # Define colors and fonts
    LIGHT_BLUE = colors.Color(147/255, 203/255, 237/255)
    BLACK = colors.black
    FONT_SIZE = 16

    # 2a. Draw the main title
    c.setFont("LiberationSans-Bold", 36)
    c.setFillColor(LIGHT_BLUE)
    c.drawCentredString(width / 2.0, height - 50, "SAVE $10,000 A YEAR")

    # 2b. Draw the Name and Goal lines/placeholders
    c.setFillColor(BLACK)
    c.setFont("LiberationSans-Bold", FONT_SIZE)
    # "NAME" and line
    c.drawString(72, height - 85, "NAME")
    c.line(125, height - 85, width/2 - 20, height - 85)
    # "I'M SAVING FOR" and line
    c.drawString(width/2 + 20, height - 85, "I'M SAVING FOR")
    c.line(width/2 + 150, height - 85, width - 72, height - 85)

    # --- 3. Table Layout & Styling ---
    # X and Y coordinates for the Table's top-left corner
    LEFT_MARGIN   = 36   # 0.5 inch
    RIGHT_MARGIN  = 36
    TOP_MARGIN    = 36
    BOTTOM_MARGIN = 36

    X_START = LEFT_MARGIN
    Y_START = height - 110

    TABLE_WIDTH = width - LEFT_MARGIN - RIGHT_MARGIN   # = 540 pt
    ROW_HEIGHT = 24

    # Calculate column widths (must sum to TABLE_WIDTH)
    # (Date, Week, Save, Balance, Checkbox) x 2
    COL_WIDTHS = [
        TABLE_WIDTH * 0.10,  # Date
        TABLE_WIDTH * 0.07,  # Week
        TABLE_WIDTH * 0.11,  # Save
        TABLE_WIDTH * 0.17,  # Balance
        TABLE_WIDTH * 0.05,  # Checkbox
    ] * 2

    # Verify the sum of widths is equal to TABLE_WIDTH (or close)
    # print(sum(COL_WIDTHS)) # Should be approx 468

    # Create the ReportLab Table object
    table = Table(table_data, colWidths=COL_WIDTHS, rowHeights=[30] + [ROW_HEIGHT] * 26)

    # Table Style definition
    style = TableStyle([
        # Table Grid (All lines)
        ('GRID', (0, 0), (-1, -1), 1, BLACK),
        # Header Row (Background and Font)
        ('BACKGROUND', (0, 0), (-1, 0), LIGHT_BLUE),
        ('TEXTCOLOR', (0, 0), (-1, 0), BLACK),
        ('FONTNAME', (0, 0), (3, 0), 'LiberationSans-Bold'),
        ('FONTNAME', (5, 0), (8, 0), 'LiberationSans-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 11),
        ('FONTSIZE', (1, 1), (-1, -1), 11),

        # Alignment for all cells
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        
        # Specific alignment for money columns (right-aligned in your image)
        # Save columns (2, 7) and Balance columns (3, 8)
        ('ALIGN', (2, 1), (3, -1), 'CENTER'),
        ('ALIGN', (7, 1), (8, -1), 'CENTER'),
        
        # Specific alignment for Week columns (left-aligned in your image)
        # Week columns (1, 6)
        ('ALIGN', (1, 1), (1, -1), 'CENTER'),
        ('ALIGN', (6, 1), (6, -1), 'CENTER'),
        
        # Cell padding
        ('LEFTPADDING', (0, 0), (-1, -1), 3),
        ('RIGHTPADDING', (0, 0), (-1, -1), 3),
        
        # Bold text for Week and Balance (optional, based on image appearance)
        ('FONTNAME', (4, 1), (4, -1), 'Helvetica'),  # Checkbox 1
        ('FONTNAME', (9, 1), (9, -1), 'Helvetica'),  # Checkbox 2
        ('FONTNAME', (1, 1), (1, -1), 'LiberationSans-Bold'), # Week 1-26
        ('FONTNAME', (6, 1), (6, -1), 'LiberationSans-Bold'), # Week 27-52
        ('FONTNAME', (2, 1), (3, -1), 'LiberationSans-Bold'), # Balance 1-26
        ('FONTNAME', (7, 1), (8, -1), 'LiberationSans-Bold'), # Balance 27-52

        # Draw a thicker vertical line separating the two halves (between col 4 and 5)
        ('LINEAFTER', (4, 0), (4, -1), 2, BLACK),

        # Draw a thicker horizontal line at the bottom of the header
        ('LINEBELOW', (0, 0), (-1, 0), 2, BLACK),
    ])

    table.setStyle(style)

    # Get the table's total height after styling/data population
    table_width, table_height = table.wrapOn(c, 0, 0)
    
    # Draw the table on the canvas
    table.drawOn(c, X_START, Y_START - table_height)

    # --- 4. Finalize ---
    c.save()
    print(f"PDF created successfully: {filename}")

if __name__ == '__main__':
    create_savings_tracker()
