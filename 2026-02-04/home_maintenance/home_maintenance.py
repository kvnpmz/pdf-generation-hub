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

big_font = 20
small_font = 15

data = {
    "MONTHLY": [
        "Test smoke detectors", 
        "Deep cleaning", 
        "Inspect fire extinguishers",
        "Clean garbage disposal", 
        "Unclog drains", 
        "Clean range hood/filter",
        "Replace HVAC filters (every 3 months)"
    ],
    "SPRING": [
        "Wash outside windows and siding",
        "Clean gutters and downspouts",
        "Pump the septic tank (if you have one)",
        "Inspect roof and chimney for any damage or leaks",
        "Service air conditioning system",
        "Apply pre-emergent to lawn",
        "Re-seal the deck, fence, and other outdoor woodwork",
        "Inspect driveway and other exterior concrete pathways",
        "Inspect your sprinkler heads, test the irrigation system",
        "Spray for mosquitos and other bugs",
        "Repair damaged screen doors and windows",
        "Sharpen lawnmower blades"
    ],
    "SUMMER": [
        "Clean the grill and refill propane tank",
        "Mulch garden beds",
        "Exterior paint touch-ups",
        "Inspect and clean dryer vent",
        "Clean refrigerator coils",
        "Clean bathroom vent fans",
        "Test your home alarm",
        "Sanitize trash and recycle bin",
        "Fertilize the lawn"
    ],
    "FALL": [
        "Service heating system",
        "Schedule a chimney sweep",
        "Put outdoor furniture and grill into storage",
        "Seal cracks on windows and doors",
        "Turn off outdoor water",
        "Winterize sprinkler system",
        "Rake leaves",
        "Clean gutters and downspouts",
        "Overseed and aerate the lawn",
        "Ensure pipes are well insulated",
        "Check attic vents"
    ],
    "WINTER": [
        "Prepare for a storm (water, non-perishables, batteries, flashlights etc.)",
        "Protect entryways (mats, weather stripping)",
        "Check insulation and add to areas that need more",
        "Test smoke & CO detectors; replace batteries",
        "Clean gutters and downspouts",
        "Insulate hot water tank",
        "Purchase a humidifier",
        "Secure steps and handrails",
        "Install storm windows and doors",
        "Remove window screens",
        "Set heat to 55 or higher"
    ]
}

# Build PDF
doc = SimpleDocTemplate(
    "maintenance_checklist.pdf",
    pagesize=letter,
    leftMargin=0.3 * inch,
    rightMargin=0.40 * inch,
    topMargin=0.3 * inch,
    bottomMargin=0.5 * inch,
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
    spaceBefore=0.30 * inch,
    spaceAfter=0.25 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=small_font,
    leading=small_font,
    fontName=FONT_REGULAR,
    spaceBefore=0.1 * inch,
    spaceAfter=0.11 * inch,
)

story = []

# Document title
story.append(Paragraph("HOME MAINTENANCE CHECKLIST", title_style))

# Add categories + items as flowables (not tables)
for cat, items in data.items():
    story.append(Paragraph(cat, category_style))
    for item in items:
        text = f'<font name="DejaVuSans">○</font> {item}'
        story.append(Paragraph(text, item_style))

# Create 6-column newspaper-style frame layout
page_width, page_height = letter

# Create full-width title frame + 6-column content frames
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
gutter = 25  # small space between columns

usable_width = page_width - doc.leftMargin - doc.rightMargin # use the FULL page width
column_width = (usable_width - gutter * (num_cols - 1)) / num_cols

content_frames = []
content_frame_height = usable_height - 0.55*inch 

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

all_frames = [title_frame] + content_frames

template = PageTemplate(id="six_column_layout", frames=all_frames)
doc.addPageTemplates([template])

# Build the PDF
doc.build(story)

print("PDF created: maintenance_checklist.pdf")

