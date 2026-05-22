from fontTools.ttLib import TTFont  # font inspection
from reportlab.pdfgen import canvas
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont as RLTTFont  # PDF rendering

FONT_NAME = "LiberationSans-Regular"

def test_unicode_symbols(output_file="unicode_test.pdf"):
    # 1️⃣ Register a font with ReportLab for PDF
    pdfmetrics.registerFont(RLTTFont(FONT_NAME, '/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf'))

    # 2️⃣ Create PDF canvas
    c = canvas.Canvas(output_file)
    c.setFont(FONT_NAME, 32)
    c.drawString(100, 700, "Check these symbols: ◇ ✓ ✗ ★ ♥ ☀ ☂ ☃ ☕ ✈")
    c.save()
    print(f"PDF created: {output_file}")

    # 3️⃣ Use fontTools to inspect the font (check for ◇)
    font = TTFont('/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf')
    found = any(0x25C7 in table.cmap for table in font['cmap'].tables)
    print(f"Found ◇ in {FONT_NAME}:", found)

# Run the test
test_unicode_symbols()

