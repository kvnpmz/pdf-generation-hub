from weasyprint import HTML

HTML('template.html').write_pdf('paleo_list_a4.pdf')

print("PDF created!")
