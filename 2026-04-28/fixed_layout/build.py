import time
from weasyprint import HTML


is_a4 = 1

name = "table_fixed"

start = time.perf_counter()
template = "template.html" if is_a4 else "template_letter.html"
output = f"{name}_{'a4' if is_a4 else 'letter'}.pdf"

HTML(template).write_pdf(output)

end = time.perf_counter()
print(f"{output} created! Render time: {end - start:.3f} seconds")

