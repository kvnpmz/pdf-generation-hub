from PIL import Image

img = Image.open("capybara.png")

# Get DPI
dpi = img.info.get('dpi', (72,72))  # default is usually 72 if not set
print(f"DPI: {dpi[0]} x {dpi[1]}")

