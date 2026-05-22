from weasyprint import HTML

html_content = """
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: sans-serif; padding: 50px; }
        input[type="text"] {
            border: 1px solid #333;
            width: 300px;
            height: 25px;
            background: #eee;
        }
    </style>
</head>
<body>
    <h2>Firefox-Editable PDF</h2>
    <label>Type your name here:</label><br>
    <input type="text" name="user_name">
</body>
</html>
"""

# The 'pdf_forms=True' part is the magic key!
HTML(string=html_content).write_pdf("editable_form.pdf", pdf_forms=True)

