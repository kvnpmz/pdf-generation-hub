from weasyprint import HTML

HTML('template.html').write_pdf('low_inflammatory.pdf')

print("PDF created!")
