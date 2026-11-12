#!/usr/bin/env bash
#
# make-variant.sh - scaffold a color variant of an existing printable.
#
# Why: a colorway is just a doc config that extends the base, a stylesheet
# that redefines the palette variables, and a copy of the base listing copy.
# Writing those by hand drifts on output_name, slug and override names.
#
# Usage:
#   scripts/make-variant.sh <base-slug> <variant> \
#       --accent '#2c5f8a' --ink '#1b2937' --muted '#6a7d8d' \
#       --line '#d5e1ea' --line-strong '#a6bccc' --wash '#f1f6fa'
#
# Writes documents/<base>_<variant>/config.tl, copies listing.txt when the
# base has one, and writes templates/<template>/<variant>_style.css.
# It never overwrites existing files and does not render or check the result;
# finish with scripts/ship-check.sh --doc <base>_<variant>.

set -euo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

usage() {
    sed -n '2,18p' "$0" | sed 's/^# \{0,1\}//'
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
        --accent|--ink|--muted|--line|--line-strong|--wash)
            PALETTE["${1#--}"]="${2:?$1 needs a value}"
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

for key in ink accent muted line line-strong wash; do
    value="${PALETTE[$key]:-}"
    [[ -n "$value" ]] || fail "missing --$key"
    [[ "$value" =~ ^#[0-9a-fA-F]{6}$ ]] || fail "--$key must be a 6-digit hex color: $value"
done

TEMPLATE="$(doc_field "$BASE" template)" || fail "cannot read template from $BASE"
OUT_NAME="$(doc_field "$BASE" output_name)" || fail "cannot read output_name from $BASE"
[[ -d "$APP/templates/$TEMPLATE" ]] || fail "missing template folder: templates/$TEMPLATE"

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

cat > "$APP/templates/$TEMPLATE/$STYLE.css" <<EOF
:root {
    --ink: ${PALETTE[ink]};
    --accent: ${PALETTE[accent]};
    --muted: ${PALETTE[muted]};
    --line: ${PALETTE[line]};
    --line-strong: ${PALETTE[line-strong]};
    --wash: ${PALETTE[wash]};
}
EOF

echo "created documents/$DOC/config.tl"
echo "created templates/$TEMPLATE/$STYLE.css"
echo "next: scripts/ship-check.sh --doc $DOC"
