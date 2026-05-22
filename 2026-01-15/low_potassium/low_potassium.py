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
big_font = 16
small_font = 13

data = {
    "Vegetables": [
        "Lettuce (iceberg, romaine)",
        "Cucumber",
        "Green beans",
        "Bell peppers (red, green, yellow)",
        "Cauliflower",
        "Cabbage",
        "Carrots (cooked, limit raw)",
        "Zucchini",
        "Summer squash",
        "Eggplant",
        "Celery",
        "Onions",
        "Radishes",
        "Mushrooms (fresh, not canned)"
    ],
    "Grains and Cereals": [
        "White rice",
        "White bread",
        "Pasta (white)",
        "Couscous",
        "Oatmeal",
        "Grits",
        "Tortillas (corn or flour)",
        "Low-potassium cereals (cornflakes, puffed rice, etc.)"
    ],
    "Beverages": [
        "Apple juice",
        "Cranberry juice",
        "Lemonade",
        "Iced tea (homemade, unsweetened)",
        "Coffee (limit to 1 cup per day)",
        "Tea (herbal, limit to 1 cup per day)"
    ],
    "Fruits": [
        "Applesauce (unsweetened)",
        "Apples",
        "Berries (strawberries, blueberries, raspberries)",
        "Grapes",
        "Pineapple",
        "Pears",
        "Watermelon",
        "Peaches (fresh or canned in juice, drained)",
        "Plums",
        "Cherries",
        "Cranberries",
        "Lemons and limes"
    ],
    "Dairy and Alternatives": [
        "Milk (limit to 1/2 cup per day, use lower potassium substitutes if needed)",
        "Cream cheese",
        "Ricotta cheese",
        "Cottage cheese (small amounts)",
        "Sherbet",
        "Sour cream (small amounts)",
        "Almond milk (unsweetened, lower potassium variety)",
        "Rice milk (unsweetened, lower potassium variety)"
    ],
    "Snacks and Sweets": [
        "Popcorn (unsalted, air-popped)",
        "Pretzels (unsalted)",
        "Rice cakes",
        "Graham crackers",
        "Jelly beans",
        "Marshmallows",
        "Hard candies"
    ],
    "Proteins": [
        "Chicken (skinless)",
        "Turkey (skinless)",
        "Beef (lean cuts)",
        "Pork (lean cuts)",
        "Eggs",
        "Tofu (soft or silken)",
        "Fish (such as cod, tilapia, haddock)",
        "Shellfish (shrimp, crab)"
    ],
    "Condiments and Seasonings": [
        "Vinegar",
        "Mustard",
        "Mayonnaise",
        "Oil (olive, vegetable)",
        "Sugar",
        "Honey",
        "Jelly or jam (small amounts)",
        "Herbs and spices (fresh or dried, avoid potassium chloride-based salt substitutes)"
    ],
    "Cooking Essentials": [
        "White flour",
        "Cornstarch",
        "Olive oil",
        "Vegetable oil",
        "Butter",
        "Sugar"
    ],
    "Miscellaneous": [
        "Gelatin desserts",
        "Rice noodles",
        "Pita bread (white)",
        "Unsalted crackers"
    ]
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "potassium_low.pdf",
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
    fontName="LiberationSans-Bold",
    textColor=colors.HexColor("#00BFFF"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=big_font,
    leading=big_font,
    fontName="LiberationSans-Bold",
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=small_font,
    leading=small_font,
    fontName="LiberationSans",
    spaceBefore=0.1 * inch,
    spaceAfter=0.101 * inch,
)

story = []

# Document title
story.append(Paragraph("Low Potassium Food List", title_style))

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

num_cols = 3
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

print("PDF created: potassium_low.pdf")

