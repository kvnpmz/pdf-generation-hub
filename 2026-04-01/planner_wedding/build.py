from weasyprint import HTML

HTML('template.html').write_pdf('wedding_planner_letter.pdf')

print("PDF created!")
