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
    "Fruits" : [
        "Apples", "Apricots", "Avocados", "Bananas", "Berries", "Cherries",
        "Grapefruit", "Grapes", "Kiwi", "Lemons", "Limes", "Melons",
        "Nectarines", "Oranges", "Papaya", "Peaches", "Pears", "Plums",
        "Pomegranate", "Watermelon", "___________", "___________", "___________"
    ],

    "Vegetables" : [
        "Artichokes", "Asparagus", "Broccoli", "Beets", "Basil", "Cabbage",
        "Cauliflower", "Carrots", "Celery", "Chiles", "Chives", "Cilantro",
        "Corn", "Cucumbers", "Eggplant", "Garlic cloves", "Green beans",
        "Lettuce", "Onions", "Peppers", "Potatoes", "Salad greens", "Spinach",
        "Sprouts", "Squash", "Tomatoes", "Zucchini", "___________", "___________", "___________"
    ],

    "Breakfast" : [
        "Cereal", "Grits", "Instant breakfast drink", "Oatmeal", "Pancake mix", "___________", "___________", "___________"
    ],

    "Meat" : [
        "Bacon", "Chicken", "Hot dogs", "Beef roast", "Ground beef", "Ground turkey",
        "Ham", "Hot dogs", "Pork", "Sausage", "Steak", "Turkey", "___________"
    ],

    "Seafood" : [
        "Catfish", "Cod", "Crab", "Halibut", "Lobster", "Oysters", "Salmon",
        "Shrimp", "Tilapia", "Tuna", "___________"
    ],

    "Frozen" : [
        "Chicken bites", "Desserts", "Fish sticks", "Fruit", "Ice",
        "Ice cream", "Ice pops", "Juice", "Meat", "Pie shells", "Pizza",
        "Pot pies", "Pudding", "TV dinners", "Vegetables",
        "Veggie burger", "Waffles", "___________"
    ],

    "Baby" : [
        "Baby cereal", "Baby food", "Diapers", "Formula cream", "Formula",
        "Wipes", "___________", "___________"
    ],

    "Pets" : [
        "Cat food", "Cat sand", "Dog food", "Shampoo", "Treats",
        "Flea treatment", "___________", "___________"
    ],

    "Baking" : [
        "Baking powder", "Baking soda", "Bread crumbs", "Cake decor", "Cake mix",
        "Canned milk", "Chocolate", "Cocoa", "Cornstarch", "Flour",
        "Food coloring", "Frosting", "Fudge mix", "Pie crust", "Shortening",
        "Sugar (brown)", "Sugar (powdered)", "Sugar", "Yeast", "___________", "___________", "___________"
    ],

    "Snacks" : [
        "Candy", "Cookies", "Crackers", "Dried fruit", "Fruit snacks",
        "Gelatin", "Graham crackers", "Granola", "Granola bars", "Gum", "Nuts",
        "Popcorn", "Potato chips", "Pretzels", "Pudding", "Seeds",
        "Tortilla chips", "___________", "___________", "___________"
    ],

    "Bakery" : [
        "Bagels", "Bread", "Donuts", "Egg", "Cookies", "Croissants",
        "Dinner rolls", "Hamburger", "Hot dog buns", "Muffins", "Pastries",
        "Pie", "Pita bread", "Rye bread", "Tortillas (corn)", "Tortillas (flour)",
        "___________", "___________", "___________", "___________"
    ],

    "Pasta & Rice" : [
        "Brown rice", "Burger helper", "Couscous", "Egg noodles", "Lasagna",
        "Mac & cheese", "Noodle mix", "Rice mix", "Spaghetti", "White rice", "___________", "___________", "___________"
    ],

    "Cans & Jars" : [
        "Applesauce", "Baked beans", "Black beans", "Broth", "Canned cubes",
        "Canned fruit", "Canned vegetables", "Canned meats", "Chili", "Corn",
        "Creamed corn", "Jam / jelly", "Mushrooms", "Olives (green)", "Olives (black)",
        "Pasta", "Pasta sauce", "Peanut butter", "Pickles", "Pie filling",
        "Soup", "___________", "___________", "___________", 
    ],

    "Refrigerated" : [
        "Biscuits", "Butter", "Cheddar", "Cheese", "Cream", "Cream cheese",
        "Dip", "Eggs", "Egg substitute", "Feta cheese", "Half & half",
        "Milk", "Milk substitute", "Mozzarella", "Processed cheese", "Provolone", "Salsa",
        "Sour cream", "Swiss cheese", "Whipped cream", "Yogurt", "___________", "___________", 
        "___________", "___________"
    ],

    "Seasoning" : [
        "Basil", "Bay leaves", "BBQ", "Cinnamon", "Cumin", "Curry",
        "Garlic powder", "Hot sauce", "Gravy mix", "Italian seasoning",
        "Marinado", "Meat tenderizer", "Oregano", "Paprika", "Pepper",
        "Poppy seed", "Red pepper", "Sage", "Salt", "Seasoned salt",
        "Soup mix", "Vanilla extract", "___________", "___________", "___________"
    ],

    "Sauces & Condiments" : [
        "BBQ sauce", "Catsup", "Cocktail sauce", "Cooking spray", "Honey",
        "Horseradish", "Hot sauce", "Lemon juice", "Mayonnaise", "Mustard",
        "Olive oil", "Relish", "Salad dressing", "Salsa", "Soy sauce",
        "Steak sauce", "Sweet & sour", "Teriyaki", "Vegetable oil",
        "Vinegar", "___________", "___________", "___________"
    ],

    "Drinks" : [
        "Beer", "Champagne", "Club soda", "Coffee", "Diet soft drinks",
        "Energy drinks", "Juice", "Liquor", "Soft drinks", "Tea", "Wine", "___________", "___________"
    ],

    "Paper Products" : [
        "Aluminum foil", "Coffee filters", "Cups", "Garbage bags", "Napkins",
        "Paper plates", "Paper towels", "Plastic bags", "Plastic cutlery",
        "Plastic wrap", "Tissues", "Waxed paper", "___________", "___________"
    ],

    "Cleaning" : [
        "Air freshener", "Bleach", "Car soap", "Dishwasher", "Dust spray",
        "Fabric softener", "Floor cleaner", "Glass spray", "Laundry soap",
        "Polish", "Sponges", "Vacuum bags", "___________", "___________", "___________"
    ],

    "Personal Care" : [
        "Bath soap", "Body coolant", "Conditioner", "Cotton swabs", "Dental floss",
        "Deodorant", "Facial tissue", "Family planning", "Feminine products",
        "Hair spray", "Hand soap", "Lip care", "Lotion", "Makeup",
        "Mouthwash", "Razors/blades", "Shampoo", "Shaving cream", "Sunscreen",
        "Toilet tissue", "Toothbrush", "Toothpaste", "___________", "___________"
    ],

    "Misc. Items" : [
        "Batteries", "Charcoal", "Greeting cards", "Light bulbs", "___________", "___________", "___________"
    ],
}

# ------------------------------------------------------------
# Build PDF
# ------------------------------------------------------------
doc = SimpleDocTemplate(
    "checklist_groceries.pdf",
    pagesize=letter,
    leftMargin=0.2 * inch,
    rightMargin=0.05 * inch,
    topMargin=0.2 * inch,
    bottomMargin=0.2 * inch,
)

styles = getSampleStyleSheet()
title_style = ParagraphStyle(
    "Title",
    parent=styles["Heading1"],
    fontSize=24,
    fontName=FONT_BOLD,
    textColor=colors.HexColor("#228B22"),
    alignment=1
)

category_style = ParagraphStyle(
    "Category",
    parent=styles["Heading3"],
    fontSize=10,
    leading=10,
    fontName=FONT_BOLD,
    spaceBefore=0.15 * inch,
    spaceAfter=0.04 * inch,
)

item_style = ParagraphStyle(
    "Items",
    parent=styles["BodyText"],
    fontSize=8,
    leading=8,
    fontName=FONT_REGULAR,
    spaceBefore=0.04 * inch,
    spaceAfter=0.04 * inch,
)

story = []

# Document title
story.append(Paragraph("GROCERY CHECKLIST", title_style))

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
    doc.leftMargin,
    page_height - doc.topMargin - 0.25 * inch,   # title band height
    usable_width,
    0.5*inch,
    showBoundary=0
)

# Six equal content frames below it
column_width = usable_width / 6
content_frames = []

content_frame_height = usable_height - 0.25*inch 
for i in range(6):
    content_frames.append(
        Frame(
            doc.leftMargin + i * column_width,
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

print("PDF created: groceries_checklist.pdf")

