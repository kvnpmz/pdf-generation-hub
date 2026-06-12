#!/usr/bin/env bash
set -euo pipefail

input="$1"

# Ensure the file exists
if [[ ! -f "$input" ]]; then
    echo "Error: '$input' is not a file" >&2
    exit 1
fi

# Resolve absolute path
base="$(basename "$input")"
name="$(basename "$input" .pdf)"

pdftoppm -png -r 300 -singlefile "$base" "${name}"

