from PIL import Image, ImageDraw

# 1. Setup dimensions
width, height = 800, 800
background_color = "#add8e6"  # Light blue
lighter_blue = "#d7edf4"      # Lighter blue
outline_color = "black"
off_white = "#fff"
border_width = 2  
radius = 10

# 2. Create the canvas
img = Image.new("RGB", (width, height), background_color)
draw = ImageDraw.Draw(img)

# --- 3. Draw the Diagonal Split ---
# This draws a triangle covering the top-left half of the image
draw.polygon([(0, 0), (width, 0), (0, height)], fill=lighter_blue)

# 4. Define the square's bounding box
margin_x = 121
margin_y1 = 5
margin_y2 = 5
shape = [margin_x, margin_y1, width - margin_x, height - margin_y2]

# 5. Draw the rounded rectangle
# Using fill="white" so your product image has a clean base
draw.rounded_rectangle(
    shape, 
    radius=radius, 
    fill=off_white,
    outline=outline_color, 
    width=border_width
)

# 6. Save/Show
img.show()
img.save("zoom_2.png")
