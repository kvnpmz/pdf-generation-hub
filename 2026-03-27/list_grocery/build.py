import time
from weasyprint import HTML

start = time.perf_counter()
output = "grocery_list"
HTML("template.html").write_pdf(f"{output}.pdf")

end = time.perf_counter()
print(f"{output} created! Render time: {end - start:.3f} seconds")
