from weasyprint import HTML

HTML('template.html').write_pdf('budget_planner_letter.pdf')

print("PDF created!")
