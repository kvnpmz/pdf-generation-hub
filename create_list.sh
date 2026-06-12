#!/bin/bash

# Create the output directory
mkdir -p ./processed_output

# Read each line from results.txt
while IFS= read -r file_path || [[ -n "$file_path" ]]; do
    [ -z "$file_path" ] && continue
    
    # 1. Define output filename
    date_dir=$(echo "$file_path" | rev | cut -d'/' -f3 | rev)
    parent_dir=$(echo "$file_path" | rev | cut -d'/' -f2 | rev)
    base_name=$(basename "$file_path" .html)
    output_filename="${date_dir}_${parent_dir}_${base_name}.txt"
    
    echo "Processing: $file_path -> ./processed_output/$output_filename"
    
    # 2. Write the header to the file
    echo "==> $file_path <==" > "./processed_output/$output_filename"
    
    # 3. Append the file content using tail
    tail -n +1 "$file_path" >> "./processed_output/$output_filename"
    
done < all_html_files.txt
