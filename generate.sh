#!/bin/bash

# 1. Get the directory where the scripts live (absolute path)
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &> /dev/null && pwd)

if [ -z "$1" ]; then
    echo "Usage: ./generate.sh [subject_name]"
    exit 1
fi

SUBJECT=$1

# 2. Stay in the current folder, but call the scripts using their full paths
# This ensures the PDFs/PNGs are in the "current directory" for the sub-scripts
"$SCRIPT_DIR/cover_pdf.sh" "${SUBJECT}_a4.pdf"
"$SCRIPT_DIR/cover_pdf.sh" "${SUBJECT}_letter.pdf"

# 3. Run image edit
source "$SCRIPT_DIR/../venv/bin/activate"

if [ -f "${SUBJECT}_a4.png" ]; then
    python "$SCRIPT_DIR/image_edit.py" "${SUBJECT}_a4.png"
else
    echo "Error: ${SUBJECT}_a4.png was not generated. Check if cover_pdf.sh succeeded."
fi

echo "Done. Processed files in $(pwd)"
