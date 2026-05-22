from weasyprint import HTML

HTML('template.html').write_pdf('carnivore_list.pdf')

print("PDF created!")
