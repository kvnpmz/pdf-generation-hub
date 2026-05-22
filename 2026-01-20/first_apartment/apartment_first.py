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
big_font = 24
small_font = 16

data = {
    "Kitchen" : [
        "Table/chairs",
		"Pots & pans",
		"Dishes & bowls",
		"Cups/mugs",
		"Utensils",
		"Blender/mixer",
		"Coffee Pot/teapot",
		"Toaster",
		"Microwave",
		"Can/bottle opener",
		"Ovenmitts",
		"Dishtowels",
		"Microfiber cloths/duster",
		"Broom & dust pan",
		"Mop/bucket",
		"Dish soap",
		"Dish drying rack",
		"Sponges",
		"Paper towels stand",
		"Tupperware",
		"Wastepaper basket",
		"Water pitcher/filter",
		"Icetrays",
		"Measuring cups",
        "Knife set",
        "Storage bags",
    ],
    "Laundry" : [
        "Hamper",
		"Ironing board",
		"Iron"
    ],
    "Bathroom" : [
        "Bath towels",
		"Hand towels",
		"Bath mat",
		"Shower curtain",
		"Rug",
		"Cleaning supplies",
		"Scale",
		"Shower organizers",
		"Toilet plunger",
		"Toilet paper stand"
    ],
    "Living Room" : [
        "Couch",
		"Chairs",
		"Coffee table",
		"TV stand",
		"Floor Lamp",
		"Curtains/blinds"
    ],
    "Technology" : [
        "TV",
		"Computer/laptop",
		"Wifi modem/router",
		"Smartphone charger"
    ],
    "Bedroom" : [
        "Mattress",
		"Bed frame",
		"Pillows",
		"Comforter/sheets",
		"Blanket",
		"Dresser",
		"Curtains/blinds",
		"Nightstand",
		"Hangers",
		"Mirror",
		"Lamp",
		"Alarm clock"
    ],
    "Misc" : [
        "Vacuum",
		"Desk/chair",
		"Key hook",
		"Flashlight",
		"First Aid Kit",
		"Batteries",
		"Lightbulbs",
		"Surge protectors",
		"Fan",
		"Book shelf",
        "Duct Tape",
		"Screwdriver/basic tools",
		"Garbage bags"
    ]
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "first_apartment.pdf",
    pagesize=letter,
    leftMargin=0.5 * inch,
    rightMargin=0 * inch,
    topMargin=0.5 * inch,
    bottomMargin=0.34 * inch,
)

styles = getSampleStyleSheet()
title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=36,
    fontName="LiberationSans-Bold",
    textColor=colors.HexColor("#30949D"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=big_font,
    leading=big_font,
    fontName="LiberationSans-Bold",
    spaceBefore=0.11 * inch,
    spaceAfter=0.16 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=small_font,
    leading=small_font,
    fontName="LiberationSans",
    spaceBefore=0.1 * inch,
    spaceAfter=0.1 * inch,
)

story = []

# Document title
story.append(Paragraph("First Apartment Checklist", title_style))

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
    page_height - doc.topMargin - 0.25 * inch,   # title band height
    usable_width,
    0.5*inch,
    showBoundary=0
)

# Six equal content frames below it

num_cols = 3
gutter = 30  # small space between columns

usable_width = page_width - doc.leftMargin - doc.rightMargin # use the FULL page width
column_width = (usable_width - gutter * (num_cols - 1)) / num_cols

content_frames = []
content_frame_height = usable_height - 0.75*inch 

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

print("PDF created: first_apartment.pdf")

