from weasyprint import HTML

HTML('template.html').write_pdf('healthy_calories_letter.pdf')

print("PDF created!")
