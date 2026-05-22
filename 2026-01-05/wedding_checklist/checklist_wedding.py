from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import letter
from reportlab.lib.colors import HexColor

def create_checklist(filename):
    c = canvas.Canvas(filename, pagesize=letter)
    width, height = letter

    # Colors
    pink_bg = HexColor("#FCE4EC")
    pink_header = HexColor("#F8BBD0")
    text_color = HexColor("#333333")

    # Title
    c.setFont("Helvetica-Bold", 28)
    c.drawString(80, height - 45, "WEDDING PLANNING CHECKLIST")

    # Names and Date Lines
    c.setFont("Helvetica", 10)
    c.drawString(50, height - 75, "BRIDE'S NAME: ___________________________")
    c.drawString(50, height - 95, "GROOM'S NAME: ___________________________")
    c.drawString(325, height - 95, "WEDDING DATE: ___________________________")

    def draw_section(x, y, title, items):
        section_width = 250
        line_height = 18
        header_height = 20
        total_height = header_height + (len(items) * line_height) + 1

        # Draw Header Background
        c.setFillColor(pink_header)
        c.rect(x, y - header_height, section_width, header_height, fill=1, stroke=0)
        
        # Draw Section Title
        c.setFillColor(text_color)
        c.setFont("Helvetica-Bold", 10)
        c.drawString(x + 10, y - 14, title.upper())

        # Draw Items
        curr_y = y - header_height - 15
        c.setFont("Helvetica", 9)
        for item in items:
            # Checkbox circle
            c.setLineWidth(0.5)
            c.circle(x + 15, curr_y + 3, 4, stroke=1, fill=0)
            # Item text
            c.drawString(x + 28, curr_y, item)
            curr_y -= line_height

        return total_height

    # Data structure for the checklist (Split for two-column layout)
    sections_left = [
        ("12 Months Before", ["Set a budget", "Make a guest list", "Choose bridal party", "Hire a wedding planner", "Decide style and theme", "Choose a venue", "Sample & select a caterer"]),
        ("11 Months Before", ["Create your wedding website", "Hire photographer & videographer", "Hire band or DJ", "Take engagement photos"]),
        ("10 Months Before", ["Wedding dress shopping", "Send out save the date cards"]),
        ("9 Months Before", ["Buy dress"]),
        ("8 Months Before", ["Choose bridesmaids dress", "Choose flowers", "Register for gifts"]),
        ("7 Months Before", ["Book rehearsal dinner venue", "Choose music for the ceremony", "Order decorations", "Hire officiant"]),
        ("6 Months Before", ["Send out wedding invitations", "Book hotel & transport for guests"]),
    ]

    sections_right = [
        ("5 Months Before", ["Book honeymoon", "Book or rent men's tuxedos"]),
        ("4 Months Before", ["Choose cake", "Buy wedding bands", "Hair & makeup trial", "Final tasting with the caterer"]),
        ("3 Months Before", ["Choose guest favors", "Write vows", "Select readings"]),
        ("2 Months Before", ["Dress fitting", "Pick up the marriage license", "Break-in wedding shoes", "Give song selection to band or DJ"]),
        ("1 Month Before", ["Assemble gift bags", "Pay vendors in full", "Create seating chart", "Venue walk-through", "Hair color refresh", "Mani/Pedi", "Final dress fitting", "Practice vows out loud"]),
        ("Night Before", ["Pray", "Drink water", "Eat a healthy meal", "Get a good night's rest"])
    ]

    # Draw Left Column
    current_y = height - 110
    for title, items in sections_left:
        h = draw_section(50, current_y, title, items)
        current_y -= (h + 15)

    # Draw Right Column
    current_y = height - 110
    for title, items in sections_right:
        h = draw_section(310, current_y, title, items)
        current_y -= (h + 15)

    c.save()

if __name__ == "__main__":
    create_checklist("wedding_checklist.pdf")
