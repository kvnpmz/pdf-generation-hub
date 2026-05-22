from weasyprint import HTML

HTML('template.html').write_pdf('affirmations_list_letter.pdf')

print("PDF created!")
