#!/bin/bash

# Check if an argument was provided
if [ -z "$1" ]; then
    echo "Error: No PDF file specified."
    echo "Usage: $0 <path_to_pdf>"
    exit 1
fi

INPUT_PDF="$1"
OUTPUT_IMAGE="${INPUT_PDF%.*}.png" # Changes .pdf extension to .png

# 1. Check if the file actually exists
if [ ! -f "$INPUT_PDF" ]; then
    echo "Error: File '$INPUT_PDF' not found."
    exit 1
fi

# 2. Check if poppler-utils is installed
if ! command -v pdfinfo &> /dev/null || ! command -v pdftoppm &> /dev/null; then
    echo "Required tools missing. Installing poppler-utils..."
    sudo apt update && sudo apt install -y poppler-utils
fi

# 3. Get the page count
PAGE_COUNT=$(pdfinfo "$INPUT_PDF" | grep "Pages:" | awk '{print $2}')

# 4. Validate page count
if [ "$PAGE_COUNT" -ne 1 ]; then
    echo "Error: This PDF has $PAGE_COUNT pages. This script only processes single-page PDFs."
    exit 1
fi

# 5. Convert the single page to 300 DPI PNG
echo "Valid 1-page PDF detected. Converting to 300 DPI PNG..."

# -f 1 -l 1 ensures only page 1 is targeted
# -singlefile prevents pdftoppm from appending "-1" to the filename
pdftoppm -png -r 300 -f 1 -l 1 -singlefile "$INPUT_PDF" "${OUTPUT_IMAGE%.*}"

if [ $? -eq 0 ]; then
    echo "Success! Image saved as: $OUTPUT_IMAGE"
else
    echo "Error: Conversion failed."
    exit 1
fi
