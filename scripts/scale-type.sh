#!/usr/bin/env bash
#
# scale-type.sh - scale every font-size (or other numeric CSS property) in a
# template stylesheet, then verify the document still fits one page.
#
# Why: "make the text bigger" or "give the chart more room" used to mean
# hand-editing a dozen declarations, running render-check.sh, reading the same
# margins, and repeating. This applies a consistent multiplier to the whole
# stylesheet and restores it automatically when the page stops fitting.
#
# Usage:
#   scripts/scale-type.sh --doc gout_food_list --scale 1.1   # one-shot
#   scripts/scale-type.sh --doc gout_food_list --fit         # largest that fits
#   scripts/scale-type.sh --doc x --fit --prop height        # grow the table rows
#   scripts/scale-type.sh --doc x --fit --max 1.5 --images   # with export PNGs
#   scripts/scale-type.sh --doc x --scale 1.1 --dry-run      # preview only
#   scripts/scale-type.sh --template kidney_food_guide --scale 1.05 --dry-run
#
# Notes:
#   --doc defaults to DOCUMENTID in teal/runtime.tl.
#   The stylesheet is templates/<template>/style.css unless --css is given.
#   --prop sets the property to scale (default font-size). Pass a comma
#   list, e.g. --prop height,padding-top. Only pt, px, rem and em values
#   are scaled; inherit and % are left alone. Height rules are matched whole,
#   so min-height and line-height are not touched by --prop height.
#   --fit binary-searches scale 1.00..--max (default 1.60, 0.01 resolution).
#   The file is only changed permanently when the render passes.
#
# Exit code: 0 = scaled and fits, 1 = does not fit (file restored), 2 = usage.

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

DOC=""
TEMPLATE=""
CSS=""
SCALE=""
MODE=""
MAX="1.60"
PROPS="font-size"
DRY=0
WAIT_IMAGES=0

usage() {
    sed -n '2,25p' "$0" | sed 's/^# \{0,1\}//'
}

while (($#)); do
    case "$1" in
        --doc) DOC="${2:?--doc needs a value}"; shift 2 ;;
        --template) TEMPLATE="${2:?--template needs a value}"; shift 2 ;;
        --css) CSS="${2:?--css needs a value}"; shift 2 ;;
        --scale) MODE="scale"; SCALE="${2:?--scale needs a value}"; shift 2 ;;
        --prop) PROPS="${2:?--prop needs a value}"; shift 2 ;;
        --fit) MODE="fit"; shift ;;
        --max) MAX="${2:?--max needs a value}"; shift 2 ;;
        --dry-run) DRY=1; shift ;;
        --images) WAIT_IMAGES=1; shift ;;
        -h|--help) usage; exit 0 ;;
        *) echo "scale-type: unknown argument: $1" >&2; exit 2 ;;
    esac
done

if [[ -z "$MODE" ]]; then
    usage >&2
    exit 2
fi

runtime_field() {
    sed -n "s/^[[:space:]]*$1[[:space:]]*=[[:space:]]*\"\{0,1\}\([^\",]*\)\"\{0,1\}[[:space:]],\{0,1\}$/\1/p" \
        "$APP/teal/runtime.tl" | head -1
}

[[ -n "$DOC" ]] || DOC="$(runtime_field DOCUMENTID)"

if [[ -z "$TEMPLATE" ]]; then
    CONFIG="$APP/documents/$DOC/config.tl"
    [[ -f "$CONFIG" ]] || { echo "scale-type: missing document config: ${CONFIG#$APP/}" >&2; exit 2; }
    TEMPLATE="$(sed -n 's/^[[:space:]]*template[[:space:]]*=[[:space:]]*"\([^"]*\)".*/\1/p' "$CONFIG" | head -1)"
fi

[[ -n "$TEMPLATE" ]] || { echo "scale-type: could not determine template for $DOC" >&2; exit 2; }
[[ -n "$CSS" ]] || CSS="$APP/templates/$TEMPLATE/style.css"
[[ -f "$CSS" ]] || { echo "scale-type: missing stylesheet: $CSS" >&2; exit 2; }

# font sizes are rewritten from this pristine copy on every attempt
TMP="$(mktemp -d "${TMPDIR:-/tmp}/scale-type.XXXXXX")"
PRISTINE="$TMP/style.css"
cp -p "$CSS" "$PRISTINE"
KEEP=0

cleanup() {
    if [[ "$KEEP" -ne 1 ]]; then
        cp -p "$PRISTINE" "$CSS"
    fi
    rm -rf "$TMP"
}
trap cleanup EXIT INT TERM HUP

apply_css() {
    # $1 = multiplier; rewrites $CSS from $PRISTINE, prints replacement count
    python3 - "$PRISTINE" "$CSS" "$1" "$PROPS" <<'PY'
import re
import sys
from pathlib import Path

source, target, factor = Path(sys.argv[1]), Path(sys.argv[2]), float(sys.argv[3])
props = [part.strip() for part in sys.argv[4].split(",") if part.strip()]
text = source.read_text()
pattern = re.compile(
    r"((?<![\w-])(?:" + "|".join(re.escape(prop) for prop in props) + r"):\s*)"
    r"([0-9]*\.?[0-9]+)(pt|px|rem|em)\b"
)


def fmt(value: float) -> str:
    return f"{value:.2f}".rstrip("0").rstrip(".")


count = 0


def replace(match: re.Match) -> str:
    global count
    count += 1
    return f"{match.group(1)}{fmt(float(match.group(2)) * factor)}{match.group(3)}"


target.write_text(pattern.sub(replace, text))
print(count)
PY
}

preview_css() {
    # $1 = multiplier; prints unique before -> after pairs, changes nothing
    python3 - "$PRISTINE" "$1" "$PROPS" <<'PY'
import re
import sys
from collections import Counter

text = sys.argv[1]
factor = float(sys.argv[2])
props = [part.strip() for part in sys.argv[3].split(",") if part.strip()]
pattern = re.compile(
    r"((?<![\w-])(?:" + "|".join(re.escape(prop) for prop in props) + r"):\s*)"
    r"([0-9]*\.?[0-9]+)(pt|px|rem|em)\b"
)
pairs: Counter = Counter()
units: dict[str, str] = {}


def fmt(value: float) -> str:
    return f"{value:.2f}".rstrip("0").rstrip(".")


def collect(match: re.Match) -> str:
    before, unit = match.group(2), match.group(3)
    key = (before, fmt(float(before) * factor))
    pairs[key] += 1
    units[key] = unit
    return match.group(0)


pattern.sub(collect, open(text).read())
for (before, after), count in sorted(pairs.items(), key=lambda item: float(item[0][0])):
    unit = units[(before, after)]
    print(f"  {before}{unit} -> {after}{unit}   ({count}x)")
PY
}

run_check() {
    "$APP/scripts/render-check.sh" --doc "$DOC" "$@" >/dev/null 2>&1
}

report() {
    echo "scale-type: $DOC  template $TEMPLATE"
    echo "  file    ${CSS#$APP/}"
    echo "  scale   $1x"
}

if [[ "$MODE" == "scale" ]]; then
    if [[ "$DRY" -eq 1 ]]; then
        echo "scale-type: $DOC  preview ${SCALE}x  (${CSS#$APP/})"
        preview_css "$SCALE"
        exit 0
    fi

    count="$(apply_css "$SCALE")"
    if run_check; then
        KEEP=1
        report "$SCALE"
        echo "  changed $count value(s) in $PROPS"
        if [[ "$WAIT_IMAGES" -eq 1 ]]; then
            "$APP/scripts/render-check.sh" --doc "$DOC" --images 2>&1 | sed 's/^/  /'
        fi
        echo "  RESULT  PASS (file updated)"
        exit 0
    fi

    echo "scale-type: ${SCALE}x does not fit; stylesheet restored" >&2
    exit 1
fi

# --fit: find the largest multiplier that still passes.
if [[ "$DRY" -eq 1 ]]; then
    echo "scale-type: $DOC  fit preview up to ${MAX}x  (${CSS#$APP/})"
    preview_css "$MAX"
    exit 0
fi

if ! run_check; then
    echo "scale-type: current layout already fails render-check; fix it first" >&2
    exit 1
fi

if apply_css "$MAX" >/dev/null && run_check; then
    BEST="$MAX"
    LO=101
else
    LO=100
    HI="$(awk -v max="$MAX" 'BEGIN { printf "%d", max * 100 }')"
    STEPS=0

    while (( HI - LO > 1 )) && (( STEPS < 10 )); do
        STEPS=$((STEPS + 1))
        MID=$(( (LO + HI) / 2 ))
        MID_SCALE="$(awk -v mid="$MID" 'BEGIN { printf "%.2f", mid / 100 }')"
        apply_css "$MID_SCALE" >/dev/null
        if run_check; then
            echo "  probe ${MID_SCALE}x  pass" >&2
            LO="$MID"
        else
            echo "  probe ${MID_SCALE}x  fail" >&2
            HI="$MID"
        fi
    done
    BEST="$(awk -v lo="$LO" 'BEGIN { printf "%.2f", lo / 100 }')"
fi

if (( LO <= 100 )); then
    cp -p "$PRISTINE" "$CSS"
    echo "scale-type: no room to enlarge; stylesheet left unchanged"
    exit 0
fi

count="$(apply_css "$BEST")"
report "$BEST"
echo "  changed $count font-size values"
if [[ "$WAIT_IMAGES" -eq 1 ]]; then
    KEEP=1
    "$APP/scripts/render-check.sh" --doc "$DOC" --images 2>&1 | sed 's/^/  /'
else
    run_check || { echo "scale-type: final render failed; restored" >&2; exit 1; }
    KEEP=1
    echo "  RESULT  PASS (file updated)"
fi
