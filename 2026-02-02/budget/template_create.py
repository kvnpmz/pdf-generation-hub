from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfbase import pdfmetrics

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

BLUE = colors.HexColor('#588AEA')
LIGHT_BLUE = colors.HexColor('#ACD6EA')
SKY_BLUE = colors.HexColor('#E7F3F7')

def create_budget_pdf(filename="budget_template.pdf"):
    """
    Generates a monthly budget template PDF using ReportLab.
    """
    doc = SimpleDocTemplate(
        filename,
        pagesize=letter,
        topMargin=0.25*inch,
        bottomMargin=0.25*inch,
        leftMargin=0.25*inch,
        rightMargin=0.25*inch
    )
    elements = []

    # --- Styles ---
    styles = getSampleStyleSheet()
    font_size = 12
    font_type = FONT_REGULAR
    bold_font = FONT_BOLD

    title_style = ParagraphStyle(
        name='TitleStyle',
        parent=styles['Heading1'],
        fontSize=28,
        alignment=1,  # Center
        textColor=BLUE,
    )
    
    header_style = ParagraphStyle(
        name='HeaderStyle',
        parent=styles['Normal'],
        fontSize=font_size,
    )

    # --- Title and Header ---
    elements.append(Paragraph("MONTHLY BUDGET TEMPLATE", title_style))
    elements.append(Spacer(1, 0.3*inch))

    header_data = [
        [Paragraph("MONTH: ____________________________", header_style), Paragraph("TOTAL INCOME: ________________", header_style)],
        [Paragraph("INCOME - EXPENSES: ________________", header_style), Paragraph("BALANCE: _____________________", header_style)]
    ]

    width, height = doc.pagesize 
    total_width = width - doc.leftMargin - doc.rightMargin
    col_widths = [total_width * 0.6, total_width * 0.4]
    row_heights = [18, 18]

    header_table = Table(header_data, colWidths=col_widths, rowHeights=row_heights)
    header_table.setStyle(TableStyle([
        ('ALIGN', (0,0), (-1,-1), 'CENTER'),
        ('VALIGN', (0,0), (-1,0), 'MIDDLE'),
        ('BOTTOMPADDING', (0,0), (-1,-1), 0),
        ('LEFTPADDING', (0,0), (-1,-1), 0),
        ('TOPPADDING', (0,0), (-1,-1), 0),
    ]))
    elements.append(header_table)
    elements.append(Spacer(1, 0.3*inch))

    # --- Table Data ---
    categories = [
        "MORTGAGE/RENT", "HOUSE MAINTENANCE/REPAIR", "HOUSE TAXES", 
        "HOUSE INSURANCE", "ELECTRICITY", "WATER", "SEWAGE", 
        "GAS/OIL", "GROCERIES", "TRASH", "LANDSCAPING/GRASS CUT", 
        "INTERNET", "CABLE/STREAMING SERVICES", "CELL PHONE", 
        "CAR PAYMENT", "CAR GAS", "CAR INSURANCE", "CAR REPAIRS", 
        "CAR INSP./REGISTRATION", "MEDICAL/PRESCRIPTIONS", 
        "COMPUTER/SOFTWARE", "WHOLESALE CLUB FEES", "GYM FEES", 
        "HAIR/GROOMING SERVICES", "CLOTHING/SHOES", "LOANS", 
        "CREDIT CARDS/DEBT", "CHILD CARE", "LAWYER FEES", 
        "TAX PREPARATION", "ENTERTAINMENT/MOVIES", 
        "CHARITY/DONATIONS"
    ]

    header_cell_style = ParagraphStyle(
        'HeaderCellStyle',
        fontSize=font_size,
        alignment=1,
        textColor=colors.black,
    )

    table_data = [
        [
            Paragraph("EXPENSES CATEGORY", header_cell_style),
            Paragraph("BUDGETED", header_cell_style),
            Paragraph("ACTUAL", header_cell_style),
            Paragraph("BALANCE", header_cell_style),
            Paragraph("NOTES", header_cell_style),
            Paragraph("PAID", header_cell_style)
        ]
    ]

    cell_style = ParagraphStyle(
        'CellStyle',
        fontSize=font_size,
        textColor=colors.HexColor('#323232'),
    )

    paid_style = ParagraphStyle(
        'PaidStyle',
        fontSize=font_size,
        alignment=1
    )

    for category in categories:
        table_data.append([
            Paragraph(f'<font name="DejaVuSans" color={BLUE}>●</font> {category}', cell_style),
            "", "", "", "",
            Paragraph(f'<font name="DejaVuSans" color={LIGHT_BLUE}>●</font>', paid_style)
        ])

    table_data.append([Paragraph("TOTAL EXPENSES", cell_style), "", "", "", "", ""])

    col_ratios = [2.1, 0.9, 0.8, 0.8, 0.9, 0.5]
    col_widths = [total_width * r / sum(col_ratios) for r in col_ratios]
    row_heights = [18] + [18]*len(categories) + [18]

    budget_table = Table(table_data, colWidths=col_widths, rowHeights=row_heights)

    
    budget_table.setStyle(TableStyle([
        ('GRID', (0,0), (-1,-1), 0.5, BLUE),
        ('BACKGROUND', (0, 0), (-1, 0), LIGHT_BLUE),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 4),
        ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('ALIGN', (0, 1), (0, -1), 'LEFT'),
        ('BACKGROUND', (1, -1), (-1, -1), LIGHT_BLUE),
        ('LEFTPADDING', (0, 1), (0, -1), 4),
        ('RIGHTPADDING', (0, 1), (0, -1), 0),
        ('TOPPADDING', (0, 0), (-1, -1), 0),
        ('BOTTOMPADDING', (0, 1), (0, -1), 4),
        ('BOTTOMPADDING', (-1, 1), (-1, -1), 6),
    ]))

    # Apply zebra striping to body rows (row 1 to second-to-last)
    for i in range(1, len(table_data)-1):
        if i % 2 == 0:
            # Even row index (considering header at 0) -> light blue
            budget_table.setStyle(TableStyle([
                ('BACKGROUND', (0, i), (-1, i), SKY_BLUE)
            ]))
        else:
            # Odd row index -> white
            budget_table.setStyle(TableStyle([
                ('BACKGROUND', (0, i), (-1, i), colors.white)
            ]))

    elements.append(budget_table)

    doc.build(elements)
    print(f"PDF created successfully: {filename}")


if __name__ == "__main__":
    create_budget_pdf()
