from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Frame, PageTemplate, Table, TableStyle
from reportlab.lib import colors

# PDF setup
doc = SimpleDocTemplate("python_cheat_sheet.pdf", pagesize=letter)
styles = getSampleStyleSheet()
normal = styles['Normal']
heading = styles['Heading2']

# Two-column frames
frame_left = Frame(40, 40, 260, 700, id='left')
frame_right = Frame(320, 40, 260, 700, id='right')
template = PageTemplate(id='TwoCol', frames=[frame_left, frame_right])
doc.addPageTemplates([template])

# Data for categories
categories_left = [
    ("1. Variables & Data Types",
     [["Type", "Example", "Notes"],
      ["Integer", "x = 10", "Whole numbers"],
      ["Float", "y = 3.14", "Decimal numbers"],
      ["String", "s = 'Hello'", "Text"],
      ["Boolean", "b = True", "True or False"],
      ["List", "lst = [1,2,3]", "Ordered, mutable"],
      ["Tuple", "tup = (1,2,3)", "Ordered, immutable"],
      ["Dictionary", "d = {'a':1}", "Key-value pairs"],
      ["Set", "st = {1,2,3}", "Unordered, unique elements"],
      ["NoneType", "n = None", "Represents no value"]]),
    ("2. Strings",
     "s = 'Python'\n# Access & slicing\ns[0], s[1:4]\n# Methods\ns.upper(), s.replace('P','J')"),
    ("3. Lists & Tuples",
     "lst = [1,2,3]\nlst.append(4), lst.remove(2)\ntup = (1,2,3)"),
    ("4. Dictionaries & Sets",
     "d = {'a':1, 'b':2}, d['c'] = 3\ns = {1,2,3}, s.add(4), s.remove(2)")
]

categories_right = [
    ("5. Conditionals & Loops",
     "x = 10\nif x>5: ... elif x==5: ... else: ...\nfor i in range(5): ...\nwhile x>0: x-=1"),
    ("6. Functions",
     "def add(a,b): return a+b\nsquare = lambda x: x**2"),
    ("7. Classes & OOP",
     "class Person:\n  def __init__(self,name,age):...\n  def greet(self): ..."),
    ("8. Modules & File I/O",
     "import math\nmath.sqrt(16)\nwith open('file.txt','w') as f: f.write('Hello')\nwith open('file.txt','r') as f: f.read()")
]

# Build content
story = []

# Left column
for title, content in categories_left:
    story.append(Paragraph(title, heading))
    if isinstance(content, list):
        t = Table(content, hAlign='LEFT', colWidths=[70,100,90])
        t.setStyle(TableStyle([
            ('GRID', (0,0), (-1,-1), 0.5, colors.black),
            ('BACKGROUND', (0,0), (-1,0), colors.lightgrey),
            ('ALIGN',(0,0),(-1,-1),'LEFT')
        ]))
        story.append(t)
    else:
        story.append(Paragraph(content.replace("\n","<br/>"), normal))
    story.append(Spacer(1,12))

# Right column
for title, content in categories_right:
    story.append(Paragraph(title, heading))
    story.append(Paragraph(content.replace("\n","<br/>"), normal))
    story.append(Spacer(1,12))

# Build PDF
doc.build(story)

