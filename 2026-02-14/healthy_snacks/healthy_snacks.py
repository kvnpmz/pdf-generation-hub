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

data = {
* PISTACHIOS (25)
* FRESH FRUIT POPSICLE
* VEGGIES AND GUACAMOLE DIP
* BOWL OF FRESH BERRIES
* WATERMELON SLICES
* WALNUTS (10 - 15)
* FRESH VEGETABLE JUICE
* BAKED SWEET POTATO
* STEAMED EDAMAME
* MIXED NUTS (1/4 CUP)
* MEDJOOL DATES
* PECAN HALVES (10 - 15)
* WHOLE GRAIN TORTILLA WITH NUT BUTTER
* FRESH VEGGIES WITH HUMMUS
* SLICED CUCUMBERS
* FRESH PINEAPPLE CHUNKS
* MANDARIN ORANGES
* BANANA WITH PEANUT BUTTER
* CARROT STICKS
* PIECE OF DARK CHOCOLATE (72%)
* ALMONDS (23)
* BAKED ZUCCHINI CHIPS
* FRESH MANGO CHUNKS
* SMALL DINNER SALAD
* CASHEWS (10 - 15)
* RAW BANANA "ICE CREAM"
* COCONUT YOGURT WITH GRANOLA
* DIY APPLE CHIPS
* FRESH FRUIT SALAD
* CUP OF BROWN RICE
* HAZELNUTS (15 - 20)
* DIY RAW ENERGY BAR
* DRIED PRUNES OR APRICOTS
* FRESH FRUIT SKEWERS
* APPLE SLICES WITH ALMOND BUTTER
* PLAIN BAKED POTATO
* FRESH SMOOTHIE
* MUESLI WITH NON-DAIRY MILK
* BROCCOLI AND CAULIFLOWER FLORETS
* VEGGIE WRAP WITH SALSA
* OATMEAL WITH FRESH FRUIT
* GRILLED PEACHES WITH CINNAMON
* BAKED SWEET POTATO FRIES
* TWO BANANAS AND RAW ALMONDS
* CHOCOLATE COVERED BANANAS (FROZEN)
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
    fontName=FONT_BOLD,
    textColor=colors.HexColor("#4E84E5"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=14,
    leading=14,
    fontName=FONT_BOLD,
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=10,
    leading=10,
    fontName=FONT_REGULAR,
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
        text = f'<font name="DejaVuSans">▢</font> {item}'
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

