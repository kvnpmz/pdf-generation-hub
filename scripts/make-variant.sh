#!/usr/bin/env bash
#
# make-variant.sh - scaffold a color variant of an existing printable.
#
# Why: a colorway is a doc config that extends the base, a stylesheet that
# overrides the template's :root variables, and a copy of the base listing.
#
# Usage:
#   scripts/make-variant.sh <base-slug> <variant> \
#       --set ink=#1b2937 --set accent=#2c5f8a --set wash=#f1f6fa ...
#
# Palette variable names are read from the template's style.css, so any
# template works. Missing variables inherit the base palette and are reported.
# Finish with scripts/ship-check.sh --doc <base>_<variant>.

set -euo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

usage() {
    sed -n '2,16p' "$0" | sed 's/^# \{0,1\}//'
}

fail() {
    echo "make-variant: $*" >&2
    exit 1
}

# Read a config field, following the extends chain (base variant included).
doc_field() {
    local doc="$1" key="$2" value=""
    local hops=0
    while ((hops++ < 5)); do
        [[ -f "$APP/documents/$doc/config.tl" ]] || return 1
        value="$(sed -n "s/^[[:space:]]*${key}[[:space:]]*=[[:space:]]*\"\([^\"]*\)\".*/\1/p" \
            "$APP/documents/$doc/config.tl" | head -1)"
        if [[ -n "$value" ]]; then
            printf '%s\n' "$value"
            return 0
        fi
        doc="$(sed -n 's/.*extends[[:space:]]*=[[:space:]]*"\([^"]*\)".*/\1/p' \
            "$APP/documents/$doc/config.tl" | head -1)"
        [[ -n "$doc" ]] || return 1
    done
    return 1
}

BASE=""
VARIANT=""
declare -A PALETTE=()

while (($#)); do
    case "$1" in
        --set)
            entry="${2:?--set needs name=#hex}"
            name="${entry%%=*}"
            value="${entry#*=}"
            [[ "$entry" == *=* && -n "$name" && -n "$value" ]] \
                || fail "--set needs name=#hex: $entry"
            PALETTE["${name#--}"]="$value"
            shift 2
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        -*)
            fail "unknown option: $1"
            ;;
        *)
            if [[ -z "$BASE" ]]; then
                BASE="$1"
            elif [[ -z "$VARIANT" ]]; then
                VARIANT="$1"
            else
                fail "unexpected argument: $1"
            fi
            shift
            ;;
    esac
done

[[ -n "$BASE" && -n "$VARIANT" ]] || { usage >&2; exit 2; }
[[ -f "$APP/documents/$BASE/config.tl" ]] || fail "missing base config: documents/$BASE/config.tl"
((${#PALETTE[@]})) || fail "no colors given (use --set name=#hex)"

TEMPLATE="$(doc_field "$BASE" template)" || fail "cannot read template from $BASE"
OUT_NAME="$(doc_field "$BASE" output_name)" || fail "cannot read output_name from $BASE"
STYLE_PATH="$APP/templates/$TEMPLATE/style.css"
[[ -f "$STYLE_PATH" ]] || fail "missing base stylesheet: templates/$TEMPLATE/style.css"

# Palette variables come from the template's :root block, not a fixed list.
VARS=()
while IFS= read -r var; do
    VARS+=("$var")
done < <(sed -n '/^[[:space:]]*:root[[:space:]]*{/,/^[[:space:]]*}/p' "$STYLE_PATH" \
    | sed -n 's/^[[:space:]]*\(--[A-Za-z0-9_-]*\)[[:space:]]*:.*/\1/p' \
    | sed 's/^--//')
((${#VARS[@]})) || fail "no :root variables in templates/$TEMPLATE/style.css"

declare -A KNOWN=()
for var in "${VARS[@]}"; do KNOWN["$var"]=1; done

for name in "${!PALETTE[@]}"; do
    [[ -n "${KNOWN[$name]:-}" ]] || \
        fail "unknown palette variable: $name (available: ${VARS[*]})"
    value="${PALETTE[$name]}"
    [[ "$value" =~ ^#?[0-9a-fA-F]{6}$ ]] || fail "--set $name must be a 6-digit hex color"
    [[ "$value" == \#* ]] || value="#$value"
    PALETTE["$name"]="$value"
done

SLUG="$(printf '%s' "$VARIANT" | tr '[:upper:]' '[:lower:]' \
    | sed 's/[^a-z0-9]\+/_/g; s/^_//; s/_$//')"
[[ -n "$SLUG" ]] || fail "variant name has no usable characters: $VARIANT"
DOC="${BASE}_${SLUG}"
STYLE="${SLUG}_style"

[[ -e "$APP/documents/$DOC" ]] && fail "already exists: documents/$DOC"
[[ -e "$APP/templates/$TEMPLATE/$STYLE.css" ]] && fail "already exists: templates/$TEMPLATE/$STYLE.css"

mkdir -p "$APP/documents/$DOC"
cat > "$APP/documents/$DOC/config.tl" <<EOF
return {
    id = "$DOC",
    output_name = "$OUT_NAME",
    overrides = "$STYLE",
    extends = "$BASE",
    completed_date = "$(date +%F)",
}
EOF

if [[ -f "$APP/documents/$BASE/listing.txt" ]]; then
    cp "$APP/documents/$BASE/listing.txt" "$APP/documents/$DOC/listing.txt"
    echo "created documents/$DOC/listing.txt"
else
    echo "make-variant: note: $BASE has no listing.txt, skipped copy" >&2
fi

{
    echo ":root {"
    missing=()
    for var in "${VARS[@]}"; do
        if [[ -n "${PALETTE[$var]:-}" ]]; then
            printf '    --%s: %s;\n' "$var" "${PALETTE[$var]}"
        else
            missing+=("$var")
        fi
    done
    echo "}"
} > "$APP/templates/$TEMPLATE/$STYLE.css"

if ((${#missing[@]})); then
    echo "make-variant: note: not overridden, inheriting base: ${missing[*]}" >&2
fi

echo "created documents/$DOC/config.tl"
echo "created templates/$TEMPLATE/$STYLE.css"
echo "next: scripts/ship-check.sh --doc $DOC"
