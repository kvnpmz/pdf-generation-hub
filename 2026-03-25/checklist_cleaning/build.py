from weasyprint import HTML

HTML('template.html').write_pdf('cleaning_checklist.pdf')

print("PDF created!")
