#!/usr/bin/env python3
import os
import sys
from PIL import Image
import shutil

# ==== CHECK CLI ARGUMENT ====
if len(sys.argv) < 2:
    print("Usage: python3 prints_create.py <input_image>")
    sys.exit(1)

input_image = sys.argv[1]

if not os.path.isfile(input_image):
    print(f"Error: {input_image} not found")
    sys.exit(1)

# ==== CONFIGURATION ====
output_folder = "output_png"

os.makedirs(output_folder, exist_ok=True)

# Target sizes in inches
common_sizes = [(4,6), (8,12), (12,18), (16,20)]
DPI = 300  # print-quality resolution

# ==== FUNCTION TO CREATE PNG ====
def create_png(img_path, size_inch, output_path):
    width_px = int(size_inch[0] * DPI)
    height_px = int(size_inch[1] * DPI)

    img = Image.open(img_path)
    orig_width, orig_height = img.size
    img_ratio = orig_width / orig_height
    target_ratio = width_px / height_px

    print(f"\nProcessing {output_path}:")
    print(f"Original resolution: {orig_width}px x {orig_height}px")
    print(f"Target size: {size_inch[0]}in x {size_inch[1]}in at {DPI} DPI")
    print(f"Pixel calculation: {size_inch[0]}in*{DPI}dpi = {width_px}px, {size_inch[1]}in*{DPI}dpi = {height_px}px")

    # Crop to match ratio if needed
    if abs(img_ratio - target_ratio) > 0.01:
        if img_ratio > target_ratio:
            # Crop width
            new_width = int(orig_height * target_ratio)
            left = (orig_width - new_width)//2
            img = img.crop((0,0,new_width,orig_height))
            print(f"Cropped width from {orig_width}px to {new_width}px")
        else:
            # Crop height
            new_height = int(orig_width / target_ratio)
            top = (orig_height - new_height)//2
            img = img.crop((0,top,orig_width,top+new_height))
            print(f"Cropped height from {orig_height}px to {new_height}px")

    # Resize
    img = img.resize((width_px,height_px), Image.LANCZOS)
    img.save(output_path, "PNG")
    print(f"Created {output_path} ({width_px}px x {height_px}px)")

# ==== MAIN LOOP ====
# Save original
shutil.copy(input_image, os.path.join(output_folder, "original.png"))

# Generate resized PNGs
for w,h in common_sizes:
    filename = f"{w}x{h}_inch.png"
    path = os.path.join(output_folder, filename)
    create_png(input_image,(w,h),path)

print(f"\nOriginal + {len(common_sizes)} PNGs generated")
