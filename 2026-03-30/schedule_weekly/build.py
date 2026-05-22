from weasyprint import HTML

HTML('template.html').write_pdf('weekly_schedule_letter.pdf')

print("PDF created!")
