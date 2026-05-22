import sys
import time
from weasyprint import HTML
from jinja2 import Template
from utils import calculate_dynamic_margins


try:
    is_letter = int(sys.argv[1])
except (IndexError, ValueError):
    is_letter = 0

name = "homeowners_checklist"
page_height = 792 if is_letter else 842
page_width = 612 if is_letter else 595
scale = 1.0 if is_letter else 1.08

start = time.perf_counter()

template_file = "template.html"
output_file = f"{name}_{'letter' if is_letter else 'a4'}.pdf"

with open(template_file, 'r') as f:
    raw_html = f.read()

context = calculate_dynamic_margins(raw_html, page_height=page_height, page_width=page_width, scale=scale)

rendered_html = Template(raw_html).render(**context)

HTML(string=rendered_html).write_pdf(output_file)

end = time.perf_counter()
print(f"{output_file} created! Render time: {end - start:.3f} seconds")

