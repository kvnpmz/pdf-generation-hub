from reportlab.platypus import SimpleDocTemplate, Image, Paragraph
from reportlab.lib.styles import getSampleStyleSheet

doc = SimpleDocTemplate("emoji.pdf")
styles = getSampleStyleSheet()

story = [
    Paragraph("Here is an emoji image:", styles['BodyText']),
    Image("smiley.png", width=20, height=20)
]

doc.build(story)

