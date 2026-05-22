'''
import time
from weasyprint import HTML


is_a4 = 1

name = "grocery_list"

start = time.perf_counter()
template = "template.html" if is_a4 else "template_letter.html"
output = f"{name}_{'a4' if is_a4 else 'letter'}.pdf"

HTML(template).write_pdf(output)

end = time.perf_counter()
print(f"{output} created! Render time: {end - start:.3f} seconds")
'''

import time
from weasyprint import HTML
from jinja2 import Template
from utils import get_column_gap

start = time.perf_counter()

# 1. New Logic: Calculate the gaps (hardcoded counts for speed)
# In the future, you can automate these counts from your data source

gaps = {
    "g1": get_column_gap(56, 3),  # Fruits(20), Ref(19), Frozen(17) | 3 Cats
    "g2": get_column_gap(72, 4),  # Veg(28), Break(11), Baby(8), Pets(5) | 4 Cats
    "g3": get_column_gap(54, 3),  # Cans(21), Snacks(18), Bakery(15) | 3 Cats
    "g4": get_column_gap(50, 4),  # Meat(11), Seafood(10), Baking(20), Pasta(9) | 4 Cats
    "g5": get_column_gap(55, 3),  # Season(24), Sauces(20), Drinks(11) | 3 Cats
    "g6": get_column_gap(49, 4)   # Paper(12), Clean(11), Personal(22), Misc(4) | 4 Cats
}

is_a4 = 1

name = "grocery_list"
start = time.perf_counter()
template = "template.html" if is_a4 else "template_letter.html"

with open("template.html") as f:
    html_str = Template(f.read()).render(gaps=gaps)

output = f"{name}_{'a4' if is_a4 else 'letter'}.pdf"

HTML(string=html_str).write_pdf(output)

end = time.perf_counter()
print(f"{output} created! Render time: {end - start:.3f} seconds")
