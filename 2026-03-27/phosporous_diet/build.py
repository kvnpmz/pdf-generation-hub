from weasyprint import HTML

HTML('template.html').write_pdf('phosphorous_diet.pdf')

print("PDF created!")
