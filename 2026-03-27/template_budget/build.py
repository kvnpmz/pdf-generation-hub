from weasyprint import HTML

HTML('template.html').write_pdf('budget_template_letter.pdf')

print("PDF created!")
