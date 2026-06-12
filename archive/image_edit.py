from PIL import Image
import os
import sys

# ==== CHECK CLI ARGUMENT ====
if len(sys.argv) < 2:
    print("Usage: python3 prints_create.py <overlay_path>")
    sys.exit(1)

overlay_path = sys.argv[1]

if not os.path.isfile(overlay_path):
    print(f"Error: {overlay_path} not found")
    sys.exit(1)

base, _ = os.path.splitext(overlay_path)

# Paths
bg1_path = "/home/kevin/z_ob/etsy/ipad.png"
bg2_path = "/home/kevin/z_ob/etsy/zoom.png"
bg3_path = "/home/kevin/z_ob/etsy/zoom_2.png"

# Logic for processing
def process_overlay(bg_path, suffix, scale, pos):
    bg = Image.open(bg_path).convert("RGBA")
    overlay = Image.open(overlay_path).convert("RGBA")
    
    # Scale
    ow, oh = overlay.size
    new_size = (int(ow * scale), int(oh * scale))
    overlay = overlay.resize(new_size, Image.LANCZOS)
    
    # Paste & Save
    bg.paste(overlay, pos, overlay)
    out_path = f"{base}_{suffix}.png"
    bg.save(out_path)
    print(f"Created: {out_path}")

# Run both operations
# (Background Path, Filename Suffix, Scale Factor, Position)
#process_overlay(bg1_path, "ipad", 0.446, (70, 88))
#process_overlay(bg2_path, "zoom", 0.53, (147, 36))
#process_overlay(bg3_path, "zoom_2", 0.58, (123, 7))

process_overlay(bg1_path, "ipad", 0.172, (70, 88))
process_overlay(bg2_path, "zoom", 0.205, (147, 38))
process_overlay(bg3_path, "zoom_2", 0.222, (126, 11))
