import time
from weasyprint import HTML

start = time.perf_counter()

#HTML('template.html').write_pdf('apartment_checklist_a4.pdf')
HTML('template_letter.html').write_pdf('apartment_checklist_letter.pdf')

end = time.perf_counter()
print(f"PDF created! Render time: {end - start:.3f} seconds")
