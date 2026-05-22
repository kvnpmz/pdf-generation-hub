from reportlab.platypus import SimpleDocTemplate, Frame, PageTemplate, Paragraph, Spacer, Image, Table, TableStyle
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.lib.units import inch
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# --- ICON MAP ---
ICON_FOLDER = "../icons/"

icon_map = {
    "GENERAL": ICON_FOLDER + "check.png",
    "KITCHEN + LAUNDRY": ICON_FOLDER + "stove.png",
    "BATHROOM": ICON_FOLDER + "bathtub.png",
    "REPAIRS": ICON_FOLDER + "wrench.png",
    "ELECTRICAL": ICON_FOLDER + "electric.png",
    "EXTERIOR": ICON_FOLDER + "house.png",
    "HEATING + COOLING": ICON_FOLDER + "heating.png",
    "WINDOWS + DOORS": ICON_FOLDER + "window.png",
    "GARAGE": ICON_FOLDER + "garage.png"
}

# --- DATA ARRAY ---
home_owner_checklist = [
    {"category": "GENERAL", "tasks": ["Do you have all of the required documents for closing?", "Are all items that were included with the sale present?", "Have any unwanted items been removed?", "Has the seller removed garbage and any construction debris?", "Is there any new damage to the floors or walls caused by movers?", "Is the property clean?"]},
    {"category": "KITCHEN + LAUNDRY", "tasks": ["Are there any signs of mold or water damage?", "Are all included appliances present and appear to be in good working order?", "If the refrigerator was removed by the seller, is there any damage done to the wall or floor?"]},
    {"category": "BATHROOM", "tasks": ["Are there any signs of mold or water damage?", "Are the toilets operational?", "Do the faucets leak?", "Do the sinks and tubs drain properly?", "Is the hot water hot?"]},
    {"category": "REPAIRS", "tasks": ["Have all of the previously agreed upon repairs been completed?", "Did the seller provide all of the warranties and bills for the repairs?"]},
    {"category": "ELECTRICAL", "tasks": ["Are all of the lights in working order?", "Are all of the outlets in working order?", "Are all of the circuit breakers clean and working?", "Are smoke detectors installed where they are required? Are they operating properly?"]},
    {"category": "EXTERIOR", "tasks": ["Are any window screens missing or damaged?", "Is there any new damage to doors, decks, siding, etc.?", "If there is an irrigation system, is it working?"]},
    {"category": "HEATING + COOLING", "tasks": ["Does the thermostat operate correctly?", "Is the heating system working properly?", "Is the cooling system working properly?"]},
    {"category": "WINDOWS + DOORS", "tasks": ["Are all the latches and locks functional?", "Are there any broken windows? Do they open and close properly?"]},
    {"category": "GARAGE", "tasks": ["Does the garage door and garage door opener operate properly?", "Does the garage door operate properly?"]}
]

# --- PDF SETUP ---
try:
    pdfmetrics.registerFont(TTFont('LiberationSans', '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))
    pdfmetrics.registerFont(TTFont('LiberationSans-Bold', '/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf'))
    FONT_NAME = "LiberationSans"
except:
    FONT_NAME = "Helvetica"

doc = SimpleDocTemplate(
    "homeowner_checklist.pdf",
    pagesize=letter,
    leftMargin=0.5 * inch,
    rightMargin=0.5 * inch,
    topMargin=0.2 * inch,
    bottomMargin=0.3 * inch,
)

styles = getSampleStyleSheet()

title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=24,
    fontName=f"{FONT_NAME}-Bold" if FONT_NAME != "Helvetica" else "Helvetica-Bold",
    textColor=colors.HexColor("#4682B4"),
    alignment=1,
)

category_style = ParagraphStyle(
    "Category",
    fontSize=16,
    fontName=f"{FONT_NAME}-Bold" if FONT_NAME != "Helvetica" else "Helvetica-Bold",
    leading=16,
    spaceAfter=20,
    textColor=colors.black,
)

task_style = ParagraphStyle(
    "Tasks",
    fontSize=13.4,
    fontName=FONT_NAME,
    leading=14,
    leftIndent=16,
    firstLineIndent=-12,
    spaceBefore=5,
    spaceAfter=10
)

# --- STORY ---
story = []
story.append(Paragraph("NEW HOME OWNERS CHECKLIST", title_style))

# --- BUILD CONTENT ---
for section in home_owner_checklist:

    # --- Icon ---
    icon_path = icon_map.get(section["category"], None)
    if icon_path:
        icon_img = Image(icon_path, width=18, height=18)
    else:
        icon_img = Spacer(18, 18)

    # --- Category text ---
    category_text = Paragraph(section["category"], category_style)

    # --- Table: icon + text ---
    category_row = Table(
        [[icon_img, category_text]],
        colWidths=[22, None]
    )
    category_row.setStyle(TableStyle([
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("TOPPADDING", (0, 0), (-1, -1), 8),   # No padding on the left of the icon
        ("LEFTPADDING", (0, 0), (0, 0), 0),   # No padding on the left of the icon
        ("LEFTPADDING", (1, 0), (1, 0), 5),   # <--- ADJUST THIS for spacing between icon/text
        ("RIGHTPADDING", (0, 0), (-1, -1), 0),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
    ]))

    story.append(category_row)

    # --- Tasks ---
    for task in section["tasks"]:
        story.append(Paragraph(f"☐ {task}", task_style))

# --- LAYOUT (2 Column Flow) ---
page_width, page_height = letter
usable_width = page_width - doc.leftMargin - doc.rightMargin
usable_height = page_height - doc.topMargin - doc.bottomMargin

title_band_height = 0.5 * inch
title_frame = Frame(
    doc.leftMargin,
    page_height - doc.topMargin - title_band_height,
    usable_width,
    title_band_height,
    showBoundary=0
)

num_cols = 2
gutter = 0.2 * inch
column_width = (usable_width - (gutter * (num_cols - 1))) / num_cols
content_height = usable_height - title_band_height

content_frames = []
for i in range(num_cols):
    x = doc.leftMargin + i * (column_width + gutter)
    content_frames.append(Frame(x, doc.bottomMargin, column_width, content_height, showBoundary=0))

template = PageTemplate(id="clean_layout", frames=[title_frame] + content_frames)
doc.addPageTemplates([template])

# --- BUILD PDF ---
doc.build(story)

print("PDF created: homeowner_checklist.pdf")

