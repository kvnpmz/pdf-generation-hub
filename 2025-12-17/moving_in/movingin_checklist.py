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
    "Living Room": [
        "Sofa",
        "Armchairs",
        "Coffee Table",
        "Side Table",
        "Floor Lamp",
        "Ceiling Light",
        "Curtains / Blinds",
        "Doormat",
        "Rug",
        "TV Display",
        "Speakers",
        "Wall Art / Decorations",
        "Bookshelf",
    ],
     "Bedroom": [
        "Bed Frame",
        "Mattress",
        "Sheets",
        "Pillows",
        "Blanket / Comforter",
        "Nightstand",
        "Lamp",
        "Wardrobe / Closet",
        "Clothes Hangers",
        "Laundry Basket",
    ],
     "Cookware": [
        "Frying Pan",
        "Saucepan",
        "Stock Pot",
        "Baking Sheet",
        "Mixing Bowls",
        "Cutting Board",
        "Knife Set",
        "Spatula",
        "Ladle",
        "Tongs",
        "Grater",
        "Measuring Cups / Spoons",
        "Colander / Strainer"
    ],
     "Dining": [
        "Dining Table",
        "Dining Chairs",
        "Placemats",
        "Coasters",
        "Tablecloth",
    ],
     "Cleaning": [
        "Broom / Dustpan",
        "Mop / Bucket",
        "Vacuum Cleaner",
        "All-purpose Cleaner",
        "Glass Cleaner",
        "Sponges / Cloths",
        "Garbage Bags",
    ],
     "Dinnerware & Utensils": [
        "Dinner Plates",
        "Side Plates",
        "Bowls",
        "Cups / Mugs",
        "Glasses",
        "Forks",
        "Spoons",
        "Knives",
        "Teaspoons",
        "Serving Spoons",
        "Serving Forks",
    ],
     "Office": [
        "Desk",
        "Desk Chair",
        "Desk Lamp",
        "Computer / Laptop",
        "Monitor",
        "Keyboard",
        "Mouse",
        "Headphones",
        "Printer",
        "Pens / Pencils",
        "Paper / Notepad",
        "Markers / Highlighters",
    ],
     "Kitchen Supplies": [
        "Food Storage Containers",
        "Trash Bin",
        "Plastic Wrap",
        "Paper Towels & Holder",
        "Dish Soap",
        "Dishwashing Brush / Sponge",
        "Dish Drying Rack",
        "Kitchen Towels",
        "Cutlery Tray",
        "Oven Mitts"
    ],
     "Bathroom": [
        "Towels",
        "Washcloths",
        "Shower Curtain",
        "Bath Mat",
        "Toilet Brush / Plunger",
        "Trash Bin",
        "Soap",
        "Shampoo / Conditioner",
        "Toothbrush / Toothpaste",
        "Toothbrush Holder",
    ],
     "Kitchen Appliances": [
        "Refrigerator / Freezer",
        "Microwave",
        "Oven / Cooktop",
        "Kettle",
        "Toaster",
        "Coffee Machine",
        "Blender / Food Processor",
        "Rice Cooker",
    ],
     "Home Organization": [
        "Storage Boxes / Bins",
        "Drawer Organizers",
        "Hooks / Wall Hangers",
        "Shelving Units",
        "Closet Organizers",
        "Laundry Hamper",
        "Post-it Notes / Tape",
    ] 
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "checklist_moving.pdf",
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
story.append(Paragraph("ESSENTIAL MOVING-IN CHECKLIST", title_style))

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

print("PDF created: checklist_moving.pdf")

