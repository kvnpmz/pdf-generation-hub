#!/usr/bin/env lua

local ASSET_DIR = "/home/kevin/z_ob/etsy"
local OUTPUT_DIR = "images"
local BGS = {
    {path = ASSET_DIR .. "/ipad.png",   suffix = "ipad",   scale = "16.8%", pos = "69,112"},
    {path = ASSET_DIR .. "/zoom.png",   suffix = "zoom",   scale = "20.0%", pos = "146,58"},
--    {path = ASSET_DIR .. "/zoom_2.png", suffix = "zoom_2", scale = "22.2%", pos = "126,11"}
}

os.execute("mkdir -p " .. OUTPUT_DIR)

if #arg == 0 then
    print("Usage: ./generate.lua /path/to/your/file.pdf")
    os.exit(1)
end

local full_path = arg[1]

local function convert_pdf(path)
    local handle = io.popen("readlink -f " .. string.format("%q", path))
    local abs_path = handle:read("*a"):gsub("\n", "")
    handle:close()

    local dir = abs_path:match("(.*/)")
    local name = abs_path:match("([^/]+)%.pdf$")

    if not name then
        print("Error: Could not extract filename from " .. abs_path)
        return nil
    end
    
    local output_dir_full = string.format(
        "%s%s/%s",
        dir,
        OUTPUT_DIR,
        name
    )

    os.execute("mkdir -p " .. string.format("%q", output_dir_full))

    local out_png = output_dir_full .. "/" .. name .. ".png"

    local cmd = string.format(

    "pdftoppm -png -r 300 -singlefile %q %q",
    abs_path,
    output_dir_full .. "/" .. name
)
    print("--- [1/2] Converting PDF to PNG ---")
    print("Source: " .. abs_path)
    print("Output: " .. out_png)
    
print("DEBUG: Executing command -> " .. cmd)
    
    local handle_cmd = io.popen(cmd .. " 2>&1")
    local err_msg = handle_cmd:read("*a")
    local success = handle_cmd:close()
    
    if success then
        print("Success: Generated " .. out_png)
        return out_png -- Ensure this line exists
    else
        print("Error: pdftoppm failed.")
        return nil
    end
end

local function process_image(img_path)
    local dir = img_path:match("(.-)[^/]-%.png$")
    local base_name = img_path:match("([^/]-)%.png$")
    
    print("\n--- [2/2] Compositing Images ---")
    for _, bg in ipairs(BGS) do
        local out_path = string.format("%s%s_%s.png", dir, base_name, bg.suffix)
        
        local cmd = string.format(
            "convert %q \\( %q -resize %s -geometry +%s \\) -composite %q 2>&1",
            bg.path, img_path, bg.scale, bg.pos:gsub(",", "+"), out_path
        )
        
        print("Processing variant: " .. bg.suffix)
        
        local handle = io.popen(cmd)
        print("DEBUG CMD: " .. cmd)
        local err = handle:read("*a")
        local success = handle:close()
        
        if success then
            print("  [OK] Saved: " .. out_path)
        else
            print("  [FAIL] Could not process variant: " .. bg.suffix)
            print("  Error Details: " .. (err ~= "" and err or "Unknown ImageMagick error"))
        end
    end
end

local png = convert_pdf(full_path)
if png then
    process_image(png)
    print("\nProcessing Complete.")
else
    print("\nProcessing Aborted.")
end
