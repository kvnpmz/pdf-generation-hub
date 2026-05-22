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

big_font = 18
small_font = 13

data = {
    "Grains And Cereals": [
        "Instant oatmeal",
        "Brown rice",
        "Quinoa",
        "Instant couscous",
        "Pasta",
        "Dry cereal (granola, cornflakes)",
        "Crackers (whole grain, rye)",
        "Rice cakes",
        "Granola bars",
        "Protein bars",
        "Whole grain tortilla chips"
    ],
    "Ready-To-Eat And Instant Foods": [
        "MREs (Meals Ready-to-Eat)",
        "Canned pasta meals",
        "Instant noodles",
        "Instant rice",
        "Shelf-stable protein shakes"
    ],
    "Dry And Dehydrated Foods": [
        "Instant mashed potatoes",
        "Dried soup mixes",
        "Freeze-dried meals (camping food)",
        "Black beans",
        "Lentils",
        "Chickpeas",
        "Split peas",
        "Dried quinoa",
        "Dried bulgur",
        "Chia seeds",
        "Flaxseeds"
    ],
    "Canned And Jarred Goods": [
        "Green beans",
        "Corn",
        "Peas",
        "Carrots",
        "Spinach",
        "Peaches",
        "Pineapple",
        "Pears",
        "Applesauce",
        "Tuna",
        "Salmon",
        "Chicken",
        "Sardines",
        "Beans (black, kidney, chickpeas, lentils)",
        "Soups (chicken noodle, vegetable, tomato)",
        "Stews (beef stew, chili)",
        "Pasta meals (spaghetti, ravioli)",
        "Nut butters (peanut butter, almond butter)",
        "Pasta sauce",
        "Salsa",
        "Jams and preserves"
    ],
    "Dairy And Alternatives": [
        "Powdered milk",
        "Evaporated milk",
        "Sweetened condensed milk",
        "Shelf-stable almond milk",
        "Shelf-stable soy milk",
         "Shelf-stable coconut milk" 
    ],
    "Condiments And Cooking Essentials": [
        "Salt",
        "Pepper",
        "Sugar",
        "Honey",
        "Vinegar",
        "Ketchup",
        "Mustard",
        "Hot sauce",
        "Soy Sauce",
        "Olive oil",
        "Vegetable oil",
        "Baking soda",
        "Flour"
    ],
    "Snacks And Sweets": [
        "Trail mix",
        "Nuts (almonds, walnuts, peanuts)",
        "Dried fruits (raisins, apricots, cranberries)",
        "Popcorn (pre-popped, shelf-stable)",
        "Chocolate bars",
        "Hard candies",
        "Fruit snacks",
        "Energy bars"
    ],
    "Beverages": [
        "Bottled water",
        "Electrolyte drinks (powder or liquid)",
        "Shelf-stable juice boxes",
        "Instant coffee",
        "Tea bags",
        "Hot chocolate mix"
    ],
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "foods_emergency.pdf",
    pagesize=letter,
    leftMargin=0.4 * inch,
    rightMargin=0.4 * inch,
    topMargin=0.3 * inch,
    bottomMargin=0.3 * inch,
)

styles = getSampleStyleSheet()
title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=36,
    fontName=FONT_BOLD,
    textColor=colors.HexColor("#30949D"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=big_font,
    leading=big_font,
    fontName=FONT_BOLD,
    spaceBefore=0.11 * inch,
    spaceAfter=0.16 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=small_font,
    leading=small_font,
    fontName=FONT_REGULAR,
    spaceBefore=0.1 * inch,
    spaceAfter=0.1 * inch,
)

story = []

# Document title
story.append(Paragraph("EMERGENCY FOOD LIST", title_style))

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
    doc.leftMargin - 0.1 * inch,
    page_height - doc.topMargin - 0.25 * inch,   # title band height
    usable_width,
    0.5*inch,
    showBoundary=0
)

# Six equal content frames below it

num_cols = 3
gutter = 10  # small space between columns

usable_width = page_width - doc.leftMargin - doc.rightMargin # use the FULL page width
column_width = (usable_width - gutter * (num_cols - 1)) / num_cols

content_frames = []
content_frame_height = usable_height - 0.55 * inch 

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

print("PDF created: foods_emergency.pdf")

