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
    "Bedroom": [
        "Bed Frame",
        "Mattress",
        "Sheets",
        "Pillows",
        "Blanket",
        "Mattress Protector",
        "Clothes Hangers",
        "Laundry Basket",
        "Under-bed Storage Boxes",
        "Desk Chair Cushion",
        "Small Rug",
        "Curtains or Blinds"
    ],
    
    "Kitchen": [
        "Mini Fridge",
        "Microwave",
        "Toaster",
        "Kettle",
        "Coffee Machine",
        "Blender",
        "Plates",
        "Bowls",
        "Cups",
        "Mugs",
        "Cutlery",
        "Cutting Board",
        "Knife Set",
        "Spatula",
        "Tongs",
        "Measuring Cups",
        "Measuring Spoons",
        "Food Storage Containers",
        "Plastic Wrap",
        "Sandwich Bags",
        "Dish Soap",
        "Sponge / Dish Brush",
        "Paper Towels",
    ],
    
    "Study": [
        "Desk",
        "Desk Chair",
        "Desk Lamp",
        "Laptop",
        "Monitor",
        "Keyboard",
        "Mouse",
        "Headphones",
        "Notepad",
        "Pens",
        "Pencils",
        "Markers",
        "Highlighters",
        "Sticky Notes",
        "Tape",
        "Desk Organizer",
        "Drawer Organizer"
    ],
    
    "Bathroom": [
        "Towels",
        "Washcloths",
        "Shower Curtain",
        "Bath Mat",
        "Soap",
        "Body Wash",
        "Shampoo",
        "Conditioner",
        "Toothbrush / Toothpaste",
        "Toothbrush Holder",
        "Razor",
        "Shaving Cream",
        "Hairbrush",
        "Comb",
        "Feminine Hygiene Products",
        "Trash Bin",
        "Cleaning Wipes",
        "Disinfectant"
    ],
    
    "Cleaning Supplies": [
        "Broom",
        "Dustpan",
        "Mop",
        "Bucket",
        "Vacuum Cleaner",
        "All-purpose Cleaner",
        "Glass Cleaner",
        "Microfiber Cloths",
        "Laundry Detergent",
        "Fabric Softener",
        "Hooks / Wall Hangers",
        "Storage Bins",
        "Shelving Units",
        "Closet Organizers",
        "Shoe Rack",
        "Drawer Organizers"
    ],
    
    "Electronics": [
        "Phone Charger",
        "Power Strip",
        "Extension Cord",
        "Alarm Clock",
        "Desk Fan",
        "Small Heater",
        "Bluetooth Speaker",
        "Lamp",
        "TV",
        "Streaming Device",
    ],
    
    "Decor": [
        "Wall Art",
        "Posters",
        "Photo Frames",
        "Throw Pillows",
        "Blankets",
        "Area Rug",
        "Indoor Plant",
        "Mirror"
    ]
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "dorm_checklist.pdf",
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
    fontSize=15,
    leading=15,
    fontName="LiberationSans-Bold",
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=11,
    leading=11,
    fontName="LiberationSans",
    spaceBefore=0.1 * inch,
    spaceAfter=0.1 * inch,
)

story = []

# Document title
story.append(Paragraph("COLLEGE DORM CHECKLIST", title_style))

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

print("PDF created: dorm_checklist.pdf")

