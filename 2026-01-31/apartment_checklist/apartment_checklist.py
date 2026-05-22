from reportlab.platypus import SimpleDocTemplate, Frame, PageTemplate, Paragraph, Spacer
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.lib.units import inch
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
CORE_FONTNAME = "LiberationSans"
if CORE_FONTNAME == "LiberationSans":
    FONT_REGULAR = f"{CORE_FONTNAME}-Regular"
else:
    FONT_REGULAR = f"{CORE_FONTNAME}"
FONT_BOLD = f"{CORE_FONTNAME}-Bold"

pdfmetrics.registerFont(TTFont('DejaVuSans', '/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Regular', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

big_font = 12
small_font = 11

data = {
    "ENTRANCE": [
        "Coat Rack", "Doormat", "Couch", "Cushions", "Chairs", 
        "Coffee Table", "TV", "Book Case", 
        "Floor Lamp", "Speakers", "Power Sockets", "Curtains", 
        "Wall Art", "Ceiling Lights"
    ],
    "DINING AREA": [
        "Dining Table", "Chairs", "Placemats", "Tablecloth", "Pot Holders"
    ],
    "APPLIANCES": [
        "Refrigerator", "Microwave", "Oven", "Cooker", 
        "Kettle", "Toaster", "Coffee Machine"
    ],
    "GLASSWARE": [
        "Wine Glasses", "Water Glasses", "Pitcher"
    ],
    "COOKWARE": [
        "Frying Pan", "Wok", "Pots", "Saucepans"
    ],
    "OFFICE": [
        "Desk", "Desk Light", "Book Shelf", "Desk Chair", 
        "Keyboard / Mouse", "Mouse Mat", "Speakers", "Printer", 
        "Computer", "Laptop Stand", "Pen", "Pencil", 
        "Markers", "Paper", "Post-it Notes", "Envelopes", "Stamps", 
        "Paperclips", "Tape", "Organizers"
    ],
    "CUTLERY": [
        "Knives", "Forks", "Spoons", "Teaspoons"
    ],
    "UTENSILS": [
        "Serving Spoons", "Ladle", "Spatula", "Whisk", "Tongs", 
        "Pizza Cutter", "Peeler", "Scissors", "Bottle Opener", 
        "Corkscrew", "Colander", "Measuring Cup"
    ],
    "CLEANING": [
        "Dishwashing Brush", "Dishwashing Tub", "Dish Rags", "Towels", 
        "Dish Drainer", "Papertowel Holder", "Cleaning Cloths", 
        "Dish Soap", "Dishwasher Tablets", "Garbage Bags"
    ],
    "KNIVES": [
        "Chef's Knife", "Pairing Knife", "Slicing Knife", 
        "Bread Knife", "Utility Knife", "Cutting Board"
    ],
    "DINNERWARE": [ "Plates", "Side Plates", "Bowls", "Deep Plates", 
        "Serving Bowls", "Coffee Mugs", "Tea Cups"
    ],
    "KITCHEN MISC.": [
        "Food Containers", "Jars", "Spice Rack", "Bin", 
        "Oven Mitts", "Cutlery Tray", "Lighter"
    ],
    "BEDROOM": [
        "Mattress", "Duvet", "Duvet Covers", "Bed Sheets", "Pillows",
        "Pillow Cases", "Closet", "Chest Of Drawers",
        "Night Table ", "Night Light", "Chair",
        "Under-Bed Storage Boxes", "Curtains", "Curtain Rails",
        "Alarm Clock", "Clothing Hangers", "Ceiling Lights"
    ],
    "BATHROOM": [
        "Bath Mat", "Shower Curtain", "Mirror", "Medicine Cabinet",
        "Towel Holder", "Towel Rail", "Bath Towels", "Hand Towels",
        "Shower Caddy", "Scale", "Hair Dryer", "Waste Bin",
        "Shelves", "Soap Dispenser", "Soap", "Shampoo",
        "Shower Gel", "Toothpaste", "Toothbrush"
    ],
    "CLEANING": [
        "Vacuum Cleaner", "Broom", "Dust Pan", "Bucket", "Mop",
        "Cleaning Cloths", "Sponges", "Cleaning Gloves",
        "All-Purpose Cleaner", "Bleach", "Toilet Cleaner",
        "Window Cleaner", "Glass Wipes", "Step Ladder"
    ],
    "LAUNDRY": [
        "Washing Machine", "Laundry Basket", "Drying Rack",
        "Detergent", "Iron", "Ironing Board"
    ],
    "TOILET": [
        "Toilet Brush", "Toilet Paper", "Toilet Paper Holder",
        "Toilet Plunger", "Waste Bin", "Air Freshener",
        "Hand Soap", "Hand Towel"
    ],
    "TOOLS": [
        "Hammer", "Nails", "Screwdriver Set", "Screws", "Saw",
        "Wrench", "Pliers", "Scissors", "Tape Measure"
    ],
    "OTHER": [
        "Smoke Detector", "Batteries", "Power Outlet",
        "Power Extension Cords", "Storage Boxes", "Candles",
        "Light Bulbs", "Flash Light", "Memo Board", "Magnets", "Vase"
    ]
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "list_apartment.pdf",
    pagesize=letter,
    leftMargin=0.25 * inch,
    rightMargin=0.25 * inch,
    topMargin=0.25 * inch,
    bottomMargin=0.25 * inch,
)

styles = getSampleStyleSheet()
title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=30,
    fontName=FONT_BOLD,
    textColor=colors.HexColor("#00BFFF"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=big_font,
    leading=big_font,
    fontName=FONT_BOLD,
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=small_font,
    leading=small_font,
    fontName=FONT_REGULAR,
    spaceBefore=0.1 * inch,
    spaceAfter=0.101 * inch,
)

story = []

# Document title
story.append(Paragraph("FIRST APARTMENT CHECKLIST", title_style))

# ------------------------------------------------------------
# Add categories + items as flowables (not tables)
# ------------------------------------------------------------
for cat, items in data.items():
    story.append(Paragraph(cat, category_style))
    for item in items:
        text = f'<font name="DejaVuSans">○</font> {item}'
        story.append(Paragraph(text, item_style))

# ------------------------------------------------------------
# Create 6-column newspaper-style frame layout
# ------------------------------------------------------------
page_width, page_height = letter

# ------------------------------------------------------------
# Create full-width title frame + 6-column content frames
# ------------------------------------------------------------
page_width, page_height = letter
usable_width = page_width - doc.leftMargin - doc.rightMargin
usable_height = page_height - doc.topMargin - doc.bottomMargin

# Full-width title frame (top area)
title_frame = Frame(
    doc.leftMargin,
    page_height - doc.topMargin - 0.3 * inch,   # title band height
    usable_width,
    0.5*inch,
    showBoundary=0
)

# Six equal content frames below it

num_cols = 5
gutter = 10  # small space between columns

usable_width = page_width - doc.leftMargin - doc.rightMargin # use the FULL page width
column_width = (usable_width - gutter * (num_cols - 1)) / num_cols

content_frames = []
content_frame_height = usable_height - 0.45*inch 

for i in range(num_cols):
    x = doc.leftMargin + i * (column_width + gutter)
    content_frames.append(
        Frame(
            x,
            doc.bottomMargin,
            column_width,
            content_frame_height,
            leftPadding=0,
            rightPadding=0,
            topPadding=0,
            bottomPadding=0,
            showBoundary=0
        )
    )

# column_width = usable_width / 6
# content_frames = []
# 
# content_frame_height = usable_height - 0.45*inch 
# for i in range(6):
    # content_frames.append(
        # Frame(
            # doc.leftMargin + i * column_width,
            # doc.bottomMargin,
            # column_width,
            # content_frame_height,
            # showBoundary=0
        # )
    # )

all_frames = [title_frame] + content_frames

template = PageTemplate(id="six_column_layout", frames=all_frames)
doc.addPageTemplates([template])

# Build the PDF
doc.build(story)

print("PDF created: list_apartment.pdf")

