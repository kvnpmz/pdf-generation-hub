from reportlab.platypus import SimpleDocTemplate, Frame, PageTemplate, Paragraph, Spacer
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.lib.units import inch
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# Register the TTF font (provide the full path to your .ttf file)
pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))

FONT_NAME = "LiberationSans"

data = {
    "Living Area": [
        "Coat Rack",
        "Doormat",
        "Sofa",
        "Cushions",
        "Armchairs",
        "Coffee Table",
        "TV / Media Unit",
        "Bookshelf",
        "Floor Lamp",
        "Speakers",
        "Power Sockets",
        "Curtains / Blinds",
        "Wall Art",
        "Ceiling Lights"
    ],

    "Dining Area": [
        "Dining Table",
        "Dining Chairs",
        "Placemats",
        "Tablecloth",
        "Napkins",
        "Coasters"
    ],

    "Office / Study": [
        "Desk",
        "Desk Lamp",
        "Desk Chair",
        "Bookshelf",
        "Computer / Laptop",
        "Laptop Stand",
        "Keyboard",
        "Mouse",
        "Mouse Pad",
        "Monitor Speakers",
        "Headphones",
        "Printer",
        "Smartphone Stand",
        "Pens / Pencils",
        "Markers",
        "Paper"
    ],

    "Organization & Supplies": [
        "Post-it Notes",
        "Envelopes",
        "Stamps",
        "Paper Clips",
        "Adhesive Tape",
        "Drawer Organizers"
    ],

    "Appliances": [
        "Fridge / Freezer",
        "Microwave",
        "Oven",
        "Cooktop",
        "Kettle",
        "Toaster",
        "Coffee Machine"
    ],

    "Dinnerware": [
        "Dinner Plates",
        "Side Plates",
        "Bowls",
        "Deep Plates",
        "Serving Bowls",
        "Coffee Mugs",
        "Tea Cups"
    ],

    "Cutlery": [
        "Knives",
        "Forks",
        "Spoons",
        "Teaspoons"
    ],

    "Knives & Boards": [
        "Chef’s Knife",
        "Paring Knife",
        "Slicing Knife",
        "Bread Knife",
        "Utility Knife",
        "Cutting Board"
    ],

    "Glassware": [
        "Water Glasses",
        "Wine Glasses"
    ],

    "Cookware": [
        "Frying Pan",
        "Saucepan",
        "Stock Pot",
        "Baking Sheet"
    ],

    "Kitchen Utensils": [
        "Serving Spoons",
        "Ladle",
        "Spatula",
        "Whisk",
        "Tongs",
        "Grater",
        "Pizza Cutter",
        "Vegetable Peeler",
        "Kitchen Scissors",
        "Can Opener",
        "Corkscrew",
        "Colander",
        "Measuring Cup",
    ],

    "Cleaning": [
        "Dish Brush",
        "Dishwashing Tub",
        "Dishcloths",
        "Kitchen Towels",
        "Dish Drainer",
        "Paper Towel Holder",
        "Cleaning Cloths",
        "Dish Soap",
        "Dishwasher Tablets",
        "Garbage Bags"
    ],

    "Storage & Misc.": [
        "Food Containers",
        "Storage Jars",
        "Spice Rack",
        "Trash Bin",
        "Oven Mitts",
        "Cutlery Tray",
        "Lighter",
        "Aluminum Foil"
    ]
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "second_apartment.pdf",
    pagesize=letter,
    leftMargin=0.30 * inch,
    rightMargin=0.05 * inch,
    topMargin=0.35 * inch,
    bottomMargin=0.35 * inch,
)

styles = getSampleStyleSheet()
title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=30,
    fontName="LiberationSans-Bold",
    textColor=colors.HexColor("#4E84E5"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=14,
    leading=14,
    fontName="LiberationSans-Bold",
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=10,
    leading=10,
    fontName="LiberationSans",
    spaceBefore=0.1 * inch,
    spaceAfter=0.1 * inch,
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
        story.append(Paragraph(f"☐ {item}", item_style))

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
column_width = usable_width / 3
content_frames = []

content_frame_height = usable_height - 0.45*inch 
for i in range(3):
    content_frames.append(
        Frame(
            doc.leftMargin + i * column_width + 0.4 * inch,
            doc.bottomMargin,
            column_width,
            content_frame_height,
            showBoundary=0
        )
    )

all_frames = [title_frame] + content_frames

template = PageTemplate(id="six_column_layout", frames=all_frames)
doc.addPageTemplates([template])

# Build the PDF
doc.build(story)

print("PDF created: second_apartment.pdf")

