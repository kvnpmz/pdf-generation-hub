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
    "Entrance" : [
        "Coat Rack",
        "Doormat",
        "Couch / Sofa",
        "Cushions",
        "Chairs",
        "Coffee Table",
        "TV / Media Unit",
        "Book Case / Shelf",
        "Floor Lamp",
        "Speakers",
        "Power Sockets",
        "Curtains / Blinds",
        "Wall Art",
        "Ceiling Lights"
    ],

    # --- DINING AREA ---
    "Dining Area" : [
        "Dining Table",
        "Chairs",
        "Placemats",
        "Tablecloth",
        "Napkins",
        "Coasters"
    ],

    # --- OFFICE / STUDY AREA ---
    "Office Study Area" : [
        "Desk",
        "Desk Light",
        "Book Shelf",
        "Desk Chair",
        "Keyboard / Mouse",
        "Mouse Mat",
        "Speakers",
        "Headphones",
        "Printer",
        "Computer / Laptop",
        "Laptop Stand",
        "Smartphone Stand",
        "Pen",
        "Pencil",
        "Markers",
        "Paper"
    ],

    # --- KITCHEN (MISC) ---
    "Kitchen Misc." : [
        "Post-it Notes",
        "Envelopes",
        "Stamps",
        "Paperclips",
        "Tape",
        "Organizers"
    ],

    # --- KITCHEN: APPLIANCES ---
    "Kitchen Appliances" : [
        "Fridge / Freezer",
        "Microwave",
        "Oven",
        "Cooker",
        "Kettle",
        "Toaster",
        "Coffee Machine"
    ],

    # --- KITCHEN: DINNERWARE ---
    "Kitchen Dinnerware" : [
        "Plates",
        "Side Plates",
        "Bowls",
        "Deep Plates",
        "Serving Bowls",
        "Coffee Mugs",
        "Tea Cups"
    ],

    # --- KITCHEN: CUTLERY ---
    "Kitchen Cutlery" : [
        "Knives",
        "Forks",
        "Spoons",
        "Teaspoons"
    ],

    # --- KITCHEN: KNIVES (SPECIFIC) ---
    "Kitchen Knives" : [
        "Chef's Knife",
        "Pairing Knife",
        "Slicing Knife",
        "Bread Knife",
        "Utility Knife",
        "Cutting Board"
    ],

    # --- KITCHEN: GLASSWARE ---
    "Kitchen Glassware" : [
        "Water Glasses",
        "Wine Glasses",
    ],

    # --- COOKWARE ---
    "Cookware" : [
        "Frying Pan",
        "Baking Sheet Pan",
        "Pots",
        "Saucepans"
    ],

    # --- KITCHEN UTENSILS ---
    "Kitchen Utensils" : [
        "Serving Spoons",
        "Ladle",
        "Spatula",
        "Whisk",
        "Tongs",
        "Pizza Cutter",
        "Peeler",
        "Scissors",
        "Bottle Opener",
        "Corkscrew",
        "Colander",
        "Measuring Cup",
        "Can Opener",
    ],

    # --- CLEANING / DISHES ---
    "Cleaning Dishes" : [
        "Dishwashing Brush",
        "Dishwashing Tub",
        "Dish Rags",
        "Towels",
        "Dish Drainer",
        "Papertowel Holder",
        "Cleaning Cloths",
        "Dish Soap",
        "Dishwasher Tablets",
        "Garbage Bags"
    ],

    # --- OTHER ---
    "Other" : [
        "Food Containers",
        "Jars",
        "Spice Rack",
        "Bin",
        "Oven Mitts",
        "Cutlery Tray",
        "Lighter",
        "Aluminum Foil",
    ],
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "list_apartment.pdf",
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
    textColor=colors.HexColor("#00BFFF"),
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

print("PDF created: list_apartment.pdf")

