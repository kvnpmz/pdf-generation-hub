from reportlab.lib import colors
from reportlab.lib.pagesizes import LETTER
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer
from reportlab.lib.units import inch

def create_food_chart(filename):
    doc = SimpleDocTemplate(filename, pagesize=LETTER, topMargin=0.5*inch, bottomMargin=0.5*inch)
    elements = []
    
    # --- Styles ---
    styles = getSampleStyleSheet()
    
    title_style = ParagraphStyle(
        'Title', fontSize=32, fontName='Helvetica-Bold', 
        textColor=colors.HexColor("#2D6A4F"), alignment=1, spaceAfter=50
    )
    
    eat_header = ParagraphStyle(
        'EatHeader', fontSize=18, fontName='Helvetica-Bold', 
        textColor=colors.white, alignment=1, leading=24
    )
    
    avoid_header = ParagraphStyle(
        'AvoidHeader', fontSize=18, fontName='Helvetica-Bold', 
        textColor=colors.white, alignment=1, leading=24
    )
    
    sub_category = ParagraphStyle(
        'SubCat', fontSize=10, fontName='Helvetica-Bold', 
        alignment=1, spaceBefore=10, spaceAfter=5, textColor=colors.black
    )
    
    list_item = ParagraphStyle(
        'ListItem', fontSize=9, fontName='Helvetica', 
        leading=11, leftIndent=10, spaceAfter=4
    )
    
    footer_title = ParagraphStyle(
        'FooterTitle', fontSize=10, fontName='Helvetica-Bold', 
        textColor=colors.HexColor("#2D6A4F")
    )

    # --- Header ---
    elements.append(Paragraph("Diverticulitis Food Chart", title_style))

    # --- Data for "FOODS TO EAT" (Left) ---
    left_data = [
        [Paragraph("FOODS TO EAT", eat_header)],
        [Paragraph("HIGH-FIBER FOODS", sub_category)],
        [Paragraph("<b>WHOLE GRAINS:</b><br/>• Whole wheat bread • Quinoa<br/>• Brown rice • Oats", list_item)],
        [Paragraph("<b>FRUITS AND VEGETABLES:</b><br/>• Apples • Spinach<br/>• Berries (strawberries, blueberries)<br/>• Bananas • Broccoli", list_item)],
        [Paragraph("<b>LEGUMES (BEANS, LENTILS):</b><br/>• Black beans • Chickpeas • Lentils", list_item)],
        [Paragraph("<b>NUTS AND SEEDS:</b><br/>• Almonds • Chia seeds", list_item)],
        [Paragraph("LEAN PROTEINS", sub_category)],
        [Paragraph("<b>SKINLESS POULTRY:</b><br/>• Chicken breast • Turkey<br/><b>FISH:</b><br/>• Salmon • Tuna<br/><b>EGGS</b>", list_item)],

        [Paragraph("HEALTHY FATS", sub_category)],
        [Paragraph("• Avocados • Flaxseeds<br/>• Olive oil • Walnuts", list_item)],
        [Paragraph("LOW-FAT DAIRY PRODUCTS", sub_category)],
        [Paragraph("• Yogurt (Greek yogurt) • Cottage cheese<br/>• Skim milk • Low-fat cheese", list_item)],
        [Paragraph("FLUIDS", sub_category)],
        [Paragraph("• Water • Broth", list_item)],
        [Paragraph("FLUIDS TO CONSUME", sub_category)],
        [Paragraph(
            """
            <b>1. WATER:</b>  Adequate hydration is crucial for digestive health and overall well-being.<br/>
            <b>2. HERBAL TEAS:</b>  Non-caffeinated herbal teas can be soothing and hydrating.<br/>
            <b>3. CLEAR BROTHS:</b>  Broths made from vegetables or lean meats can provide hydration and some nutrients.<br/>
            <b>4. COCONUT WATER:</b>  Provides hydration and electrolytes.<br/>
            <b>5. VEGETABLE JUICES:</b>  Low-sodium vegetable juices can contribute to fluid intake and provide some vitamins and minerals.<br/>
            """,
            list_item
        )],
    ]
    
    left_table = Table(left_data, colWidths=[3.2*inch])
    left_table.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (0,0), colors.HexColor("#4CAF50")), # Dark Green Header
        ('BOTTOMPADDING', (0,0), (0,0), 10),
        ('TOPPADDING', (0,0), (0,0), 10),
        ('VALIGN', (0,0), (-1,-1), 'TOP'),
    ]))

    # --- Data for "FOODS TO AVOID" (Right) ---
    right_data = [
        [Paragraph("FOODS TO AVOID", avoid_header)],
        [Paragraph("LOW-FIBER FOODS", sub_category)],
        [Paragraph("• Processed grains (white bread, white rice)<br/>• Cereals high in sugar and low in fiber", list_item)],
        [Paragraph("CANNED FRUITS AND VEGETABLES", sub_category)],
        [Paragraph("FRUIT JUICES", sub_category)],
        [Paragraph("• Fruit juices without pulp", list_item)],
        [Paragraph("REFINED CARBOHYDRATES", sub_category)],
        [Paragraph("• Pastries • White pasta • Instant noodles", list_item)],
        [Paragraph("NUTS AND SEEDS", sub_category)],
        [Paragraph("• Peanuts • Cashews • Pistachios", list_item)],
        [Paragraph("FATTY MEATS", sub_category)],
        [Paragraph("• Bacon • Sausages<br/>• Processed meats (hot dogs, deli meats)", list_item)],
        [Paragraph("HIGH-FAT FOODS", sub_category)],
        [Paragraph("• Fried foods • Potato chips<br/>• Fast food • Butter", list_item)],
        [Paragraph("SPICY OR HIGHLY SEASONED FOODS", sub_category)],
        [Paragraph("• Chili peppers • Spicy sauces", list_item)],
        [Paragraph("LIQUIDS TO AVOID", sub_category)],
        [Paragraph("<b>1. CARBONATED BEVERAGES:</b> High sugar content and gas-producing.<br/><b>2. ALCOHOL:</b> Can irritate the digestive system.", list_item)],
    ]
    
    right_table = Table(right_data, colWidths=[3.2*inch])
    right_table.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (0,0), colors.HexColor("#E91E63")), # Dark Pink/Red Header
        ('BOTTOMPADDING', (0,0), (0,0), 10),
        ('TOPPADDING', (0,0), (0,0), 10),
        ('VALIGN', (0,0), (-1,-1), 'TOP'),
    ]))

    # --- Combine into Main Layout Table ---
    main_layout = Table([[left_table, right_table]], colWidths=[3.5*inch, 3.5*inch])
    main_layout.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (0,0), colors.HexColor("#F1F8E9")), # Light green bg
        ('BACKGROUND', (1,0), (1,0), colors.HexColor("#FCE4EC")), # Light pink bg
        ('VALIGN', (0,0), (-1,-1), 'TOP'),
        ('LEFTPADDING', (0,0), (-1,-1), 10),
        ('RIGHTPADDING', (0,0), (-1,-1), 10),
        ('BOTTOMPADDING', (0,0), (-1,-1), 20),
    ]))

    elements.append(main_layout)
    elements.append(Spacer(1, 15))

    doc.build(elements)

create_food_chart("foodchart_diverticulitis.pdf")
