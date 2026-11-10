#!/usr/bin/env bash
#
# render-check.sh - one-shot render + page-fit verification.
#
# Why: `tpr` is a blocking file watcher (Program.cs -> Watcher.Start()), so a
# quick check with it burns the whole timeout. This script runs the renderer
# once, kills it as soon as the document is written, and reports whether the
# output fits the page. Typical run: ~3-5 s.
#
# Usage:
#   scripts/render-check.sh                     # render runtime.tl doc + verify
#   scripts/render-check.sh --doc grocery_shopping_list
#   scripts/render-check.sh --no-render         # just re-measure existing output
#   scripts/render-check.sh --style-append 'li.item{line-height:1.16}'
#                                               # probe a CSS tweak (style.css is
#                                               # backed up and restored on exit)
#   scripts/render-check.sh --json              # machine-readable result
#   scripts/render-check.sh --images            # also wait for the export PNGs
#                                               # (slower, use for final checks)
#   scripts/render-check.sh --rebuild           # force dotnet publish first
#
# Exit code: 0 = fits, 1 = does not fit (or render failed).
#
# Thresholds (override with env vars):
#   MIN_FILL=85        minimum ink height as % of page height
#   MIN_BOTTOM=24      minimum bottom margin in pt
#   MAX_BOTTOM=64      maximum bottom margin in pt (larger = unused space)
#   MIN_SIDE=40        minimum left/right margin in pt
#   MAX_SPREAD=10      maximum spread of column bottoms in pt

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BIN_DIR="${TPR_BIN_DIR:-/root/tpr}"
BIN="$BIN_DIR/TemplatePrintable"

DOC=""
REBUILD=0
DO_RENDER=1
JSON=0
WAIT_IMAGES=0
STYLE_APPEND=""
TIMEOUT="${RENDER_TIMEOUT:-45}"

MIN_FILL="${MIN_FILL:-85}"
MIN_BOTTOM="${MIN_BOTTOM:-24}"
MAX_BOTTOM="${MAX_BOTTOM:-64}"
MIN_SIDE="${MIN_SIDE:-40}"
MAX_SPREAD="${MAX_SPREAD:-10}"

while (($#)); do
    case "$1" in
        --doc) DOC="${2:?--doc needs a value}"; shift 2 ;;
        --rebuild) REBUILD=1; shift ;;
        --no-render) DO_RENDER=0; shift ;;
        --images) WAIT_IMAGES=1; shift ;;
        --json) JSON=1; shift ;;
        --timeout) TIMEOUT="${2:?--timeout needs a value}"; shift 2 ;;
        --style-append) STYLE_APPEND="${2:?--style-append needs a value}"; shift 2 ;;
        -h|--help) sed -n '2,32p' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
        *) echo "render-check: unknown argument: $1" >&2; exit 2 ;;
    esac
done

cd "$APP" || exit 2

# The renderer loads Teal through NLua and needs the LuaRocks paths from env.sh
# (tpr sources the same file before launching the binary).
if [[ -f "$APP/env.sh" ]]; then
    # shellcheck disable=SC1090
    source "$APP/env.sh" >/dev/null 2>&1 || true
fi

fail() { echo "render-check: $*" >&2; exit 2; }

command -v magick >/dev/null 2>&1 || fail "ImageMagick (magick) not found"

runtime_field() {
    sed -n "s/^[[:space:]]*$1[[:space:]]*=[[:space:]]*\"\{0,1\}\([^\",]*\)\"\{0,1\}[[:space:]]*,\{0,1\}$/\1/p" \
        teal/runtime.tl | head -1
}

[[ -n "$DOC" ]] || DOC="$(runtime_field DOCUMENTID)"
[[ -n "$DOC" ]] || fail "could not read DOCUMENTID from teal/runtime.tl"
ENABLE_IMAGES="$(runtime_field ENABLEIMAGES)"
ENABLE_IMAGES="${ENABLE_IMAGES:-0}"

CONFIG="documents/$DOC/config.tl"
[[ -f "$CONFIG" ]] || fail "missing document config: $CONFIG"

TEMPLATE="$(sed -n 's/^[[:space:]]*template[[:space:]]*=[[:space:]]*"\([^"]*\)".*/\1/p' "$CONFIG" | head -1)"
OUT_NAME="$(sed -n 's/^[[:space:]]*output_name[[:space:]]*=[[:space:]]*"\([^"]*\)".*/\1/p' "$CONFIG" | head -1)"
OUT_NAME="${OUT_NAME:-$DOC}"

OUT_DIR="output/$DOC"
# Editable documents are exported with an _editable suffix; measure the file
# the renderer actually writes instead of waiting on a stale plain export.
IS_EDITABLE="$(sed -n 's/^[[:space:]]*is_editable[[:space:]]*=[[:space:]]*true.*/1/p' "$CONFIG" | head -1)"
SUFFIX=""
[[ "$IS_EDITABLE" == "1" ]] && SUFFIX="_editable"
PDF="$OUT_DIR/$OUT_NAME$SUFFIX.pdf"
PNG="$OUT_DIR/images/$OUT_NAME$SUFFIX.png"

TMP="$(mktemp -d "${TMPDIR:-/tmp}/render-check.XXXXXX")"
PID=""
CSS_PATH=""
CSS_BAK=""
RUNTIME="$APP/teal/runtime.tl"
RUNTIME_BAK="$TMP/runtime.tl.orig"
RUNTIME_TOUCHED=0

# --style-append temporarily edits a tracked stylesheet, so keep the pristine
# copy in a gitignored state dir: if a run is killed without restoring, the
# next run heals the file before doing anything else.
PROBE_DIR="$APP/.render-check"
PROBE_STATE="$PROBE_DIR/probe.state"
PROBE_BACKUP="$PROBE_DIR/style.orig"

heal_probe() {
    [[ -f "$PROBE_STATE" && -f "$PROBE_BACKUP" ]] || return 0
    local path
    path="$(cat "$PROBE_STATE")"
    if [[ -f "$path" ]]; then
        cp -p "$PROBE_BACKUP" "$path"
        echo "render-check: restored $path from an interrupted probe run" >&2
    fi
    rm -f "$PROBE_STATE"
}

restore_css() {
    if [[ -n "$CSS_PATH" && -n "$CSS_BAK" && -f "$CSS_BAK" ]]; then
        cp -p "$CSS_BAK" "$CSS_PATH"
        rm -f "$CSS_BAK" "$PROBE_STATE" "$PROBE_BACKUP"
        CSS_BAK=""
    fi
}

restore_runtime() {
    if [[ "$RUNTIME_TOUCHED" -eq 1 && -f "$RUNTIME_BAK" ]]; then
        cp -p "$RUNTIME_BAK" "$RUNTIME"
        RUNTIME_TOUCHED=0
    fi
}

cleanup() {
    restore_css
    restore_runtime
    if [[ -n "$PID" ]]; then
        kill -TERM -- "-$PID" 2>/dev/null
        sleep 0.1
        kill -KILL -- "-$PID" 2>/dev/null
    fi
    rm -rf "$TMP"
}
heal_probe
trap cleanup EXIT INT TERM HUP

# Wait until $1 exists, is newer than $stamp, and has stopped growing.
# Returns 0 = ready, 1 = timeout, 2 = renderer reported an error.
wait_for_output() {
    local f="$1" deadline=$((SECONDS + TIMEOUT))
    while ((SECONDS < deadline)); do
        if grep -qE "LUA ERROR|GENERAL ERROR|INIT/RENDER FAILED|tl check failed" \
            "$TMP/render.log" 2>/dev/null; then
            return 2
        fi
        if [[ -f "$f" && "$f" -nt "$stamp" ]]; then
            local s1 s2
            s1="$(stat -c%s "$f" 2>/dev/null || echo 0)"
            sleep 0.25
            s2="$(stat -c%s "$f" 2>/dev/null || echo 0)"
            if [[ "$s1" == "$s2" && "$s1" != "0" ]] && magick identify "$f" >/dev/null 2>&1; then
                return 0
            fi
        fi
        kill -0 "$PID" 2>/dev/null || return 1
        sleep 0.1
    done
    return 1
}

render_ms=0

if ((DO_RENDER)); then
    # ------------------------------------------------------------ build
    need_publish=$((REBUILD))
    [[ -x "$BIN" ]] || need_publish=1
    [[ "$APP/TemplatePrintable.csproj" -nt "$BIN" ]] && need_publish=1
    if [[ -n "$(find "$APP/src" -name '*.cs' -newer "$BIN" -print -quit 2>/dev/null)" ]]; then
        need_publish=1
    fi

    if ((need_publish)); then
        command -v dotnet >/dev/null 2>&1 || fail "dotnet not found, cannot publish renderer"
        echo "render-check: publishing renderer (sources changed)..." >&2
        if ! dotnet publish "$APP/TemplatePrintable.csproj" -c Release -o "$BIN_DIR" -v q --nologo \
            >"$TMP/publish.log" 2>&1; then
            tail -20 "$TMP/publish.log" >&2
            fail "dotnet publish failed"
        fi
    fi

    # ------------------------------------------------------------ render
    if [[ -n "$STYLE_APPEND" ]]; then
        CSS_PATH="templates/$TEMPLATE/style.css"
        [[ -f "$CSS_PATH" ]] || fail "cannot append style: $CSS_PATH not found"
        mkdir -p "$PROBE_DIR"
        CSS_BAK="$PROBE_BACKUP"
        cp -p "$CSS_PATH" "$CSS_BAK"
        printf '%s' "$CSS_PATH" >"$PROBE_STATE"
        printf '\n/* render-check probe (temporary) */\n%s\n' "$STYLE_APPEND" >>"$CSS_PATH"
    fi

    # The renderer takes its document from teal/runtime.tl, not from --doc.
    # Switch the runtime to the requested document and restore it afterwards,
    # so --doc works for any document instead of only the current one.
    if [[ "$(runtime_field DOCUMENTID)" != "$DOC" ]]; then
        cp -p "$RUNTIME" "$RUNTIME_BAK"
        RUNTIME_TOUCHED=1
        ENABLE_IMAGES=1
        cat >"$RUNTIME" <<EOF
return {
    DOCUMENTID = "$DOC",
    ENABLEIMAGES = 1,
    BASEPROJECTNAME = "$DOC"
}
EOF
    fi

    stamp="$TMP/stamp"
    touch "$stamp"
    target="$PDF"

    render_start=$(date +%s%N)
    setsid "$BIN" >"$TMP/render.log" 2>&1 &
    PID=$!

    ready=0
    wait_for_output "$target"
    case $? in
        0) ready=1 ;;
        2)
            sleep 0.2
            kill -TERM -- "-$PID" 2>/dev/null
            wait "$PID" 2>/dev/null
            PID=""
            restore_css
            echo "render-check: renderer error (fast fail)" >&2
            # tl check output / Lua errors come before the .NET stack trace
            awk '/^INIT\/RENDER FAILED:/{exit} /^GENERAL ERROR:/{exit} {print}' "$TMP/render.log" |
                head -25 >&2
            grep -E "^(INIT/RENDER FAILED|GENERAL ERROR|System\.Exception|.*tl check failed)" "$TMP/render.log" |
                head -4 >&2
            exit 1
            ;;
    esac

    # The image step runs after the PDF export, so wait for it separately.
    if ((ready && WAIT_IMAGES && ENABLE_IMAGES == 1)); then
        wait_for_output "$PNG" || true
    fi

    sleep 0.2
    kill -TERM -- "-$PID" 2>/dev/null
    wait "$PID" 2>/dev/null
    PID=""

    restore_css
    restore_runtime

    render_ms=$(( ($(date +%s%N) - render_start) / 1000000 ))

    if ((!ready)); then
        echo "render-check: render did not produce $target within ${TIMEOUT}s" >&2
        tail -15 "$TMP/render.log" >&2
        exit 1
    fi
fi

[[ -f "$PDF" ]] || fail "missing pdf: $PDF"

# ---------------------------------------------------------------- measure
# Prefer the renderer's own 300dpi PNG, but only when it matches this run;
# a multi-page PDF is emitted as name-0.png / name-1.png instead.
use_png=0
if [[ -f "$PNG" ]] && magick identify "$PNG" >/dev/null 2>&1; then
    if [[ -n "${stamp:-}" ]]; then
        [[ "$PNG" -nt "$stamp" ]] && use_png=1
    elif [[ ! -f "$PDF" || "$PNG" -nt "$PDF" ]]; then
        use_png=1
    fi
fi

measure_input=""
if ((use_png)); then
    measure_input="$PNG"
else
    magick -density 200 "$PDF[0]" "$TMP/page.png" 2>/dev/null
    [[ -f "$TMP/page.png" ]] || fail "could not rasterize $PDF"
    measure_input="$TMP/page.png"
fi

pages="$(magick identify -format "%n\n" "$PDF" 2>/dev/null | head -1)"
pages="${pages:-1}"

page_w_pt="$(magick identify -format "%w\n" "$PDF" 2>/dev/null | head -1)"
page_h_pt="$(magick identify -format "%h\n" "$PDF" 2>/dev/null | head -1)"
page_w_pt="${page_w_pt:-612}"
page_h_pt="${page_h_pt:-792}"

read -r img_w img_h < <(magick identify -format "%w %h\n" "$measure_input" | head -1)

trim_geom="$(magick "$measure_input" -fuzz 1% -trim -format "%w %h %X %Y" info: 2>/dev/null)"
read -r ink_w ink_h ink_x ink_y < <(
    printf '%s\n' "$trim_geom" |
        sed 's/^\([0-9]*\) \([0-9]*\) +\([0-9-]*\) +\([0-9-]*\)$/\1 \2 \3 \4/'
)
[[ -n "${ink_w:-}" ]] || fail "could not measure ink box (blank page?)"

scale="$(awk -v iw="$img_w" -v pw="$page_w_pt" 'BEGIN { if (pw > 0) printf "%.6f", iw / pw; else print 1 }')"

read -r top_pt bottom_pt left_pt right_pt fill_pct < <(
    awk -v s="$scale" -v iw="$img_w" -v ih="$img_h" \
        -v x="$ink_x" -v y="$ink_y" -v w="$ink_w" -v h="$ink_h" 'BEGIN {
            printf "%.1f %.1f %.1f %.1f %.1f\n",
                y / s,
                (ih - (y + h)) / s,
                x / s,
                (iw - (x + w)) / s,
                (h / ih) * 100
        }'
)

# ---------------------------------------------------------------- columns
pad_px="$(awk -v s="$scale" 'BEGIN { printf "%d", 50.4 * s }')"
band_px="$(awk -v s="$scale" -v iw="$img_w" 'BEGIN { printf "%d", (iw - 2 * 50.4 * s) / 6 }')"

bottoms=()
for i in 0 1 2 3 4 5; do
    x=$((pad_px + i * band_px))
    ((x + band_px > img_w)) && band_px=$((img_w - x))
    geo="$(magick "$measure_input" -crop "${band_px}x${img_h}+${x}+0" +repage \
        -fuzz 1% -trim -format "%h %Y" info: 2>/dev/null)"
    bh="$(printf '%s' "$geo" | awk '{print $1}')"
    by="$(printf '%s' "$geo" | awk '{print $2}' | tr -d '+')"
    [[ -z "$bh" ]] && bh=0
    [[ -z "$by" ]] && by=0
    bottoms+=("$(awk -v h="$bh" -v y="$by" -v s="$scale" -v ih="$img_h" \
        'BEGIN { printf "%.1f", (ih - (y + h)) / s }')")
done

col_list=""
for b in "${bottoms[@]}"; do
    col_list="${col_list} ${b}"
done
col_list="${col_list# }"
spread_pt="$(printf '%s\n' "${bottoms[@]}" | sort -n | awk 'NR==1{min=$1} {max=$1} END{printf "%.1f", max-min}')"

# ---------------------------------------------------------------- verdict
warnings=()
status="PASS"

check_range() { # value min max label
    local v="$1" lo="$2" hi="$3" label="$4"
    if ! awk -v v="$v" -v lo="$lo" -v hi="$hi" 'BEGIN { exit !(v >= lo && v <= hi) }'; then
        warnings+=("$label: ${v} (expected ${lo}..${hi})")
        status="FAIL"
    fi
}

if ((pages != 1)); then
    warnings+=("page count: ${pages} (expected 1)")
    status="FAIL"
fi
check_range "$fill_pct" "$MIN_FILL" 100 "fill"
check_range "$bottom_pt" "$MIN_BOTTOM" "$MAX_BOTTOM" "bottom margin"
check_range "$left_pt" "$MIN_SIDE" 200 "left margin"
check_range "$right_pt" "$MIN_SIDE" 200 "right margin"
check_range "$spread_pt" 0 "$MAX_SPREAD" "column bottom spread"

if [[ -n "$STYLE_APPEND" ]] && ! git diff --quiet -- "templates/$TEMPLATE/style.css" 2>/dev/null; then
    warnings+=("style.css was not restored cleanly")
    status="FAIL"
fi

if ((JSON)); then
    printf '{"doc":"%s","pages":%s,"render_ms":%s,"fill":%s,"margin_top":%s,"margin_bottom":%s,"margin_left":%s,"margin_right":%s,"column_bottoms":[%s],"column_spread":%s,"pass":%s,"warnings":[' \
        "$DOC" "$pages" "$render_ms" "$fill_pct" "$top_pt" "$bottom_pt" "$left_pt" "$right_pt" \
        "$(printf '%s' "$col_list" | sed 's/  */,/g')" "$spread_pt" \
        "$([[ $status == PASS ]] && echo true || echo false)"
    first=1
    for w in "${warnings[@]:-}"; do
        [[ -z "$w" ]] && continue
        ((first)) || printf ','
        printf '"%s"' "$(printf '%s' "$w" | sed 's/"/\\"/g')"
        first=0
    done
    printf ']}\n'
else
    printf 'render-check: %s\n' "$DOC"
    if ((render_ms > 0)); then
        printf '  render    %.1fs\n' "$(awk -v ms="$render_ms" 'BEGIN {print ms/1000}')"
    else
        printf '  render    (skipped, measured existing output)\n'
    fi
    printf '  pages     %s\n' "$pages"
    printf '  canvas    %sx%s px  (%sx%s pt)\n' "$img_w" "$img_h" "$page_w_pt" "$page_h_pt"
    printf '  margins   top %.1f  bottom %.1f  left %.1f  right %.1f pt\n' \
        "$top_pt" "$bottom_pt" "$left_pt" "$right_pt"
    printf '  fill      %.1f%% of page height\n' "$fill_pct"
    printf '  columns   bottoms:%s  spread %.1f pt\n' "$col_list" "$spread_pt"
    if ((${#warnings[@]})); then
        for w in "${warnings[@]}"; do printf '  WARN      %s\n' "$w"; done
    fi
    printf '  RESULT    %s\n' "$status"
fi

# A previous failing run may have produced name-0.png / name-1.png; clear them
# once the document is back to a single page so the output folder stays tidy.
if ((DO_RENDER && pages == 1)); then
    rm -f "$OUT_DIR/images/${OUT_NAME}-"[0-9]*.png 2>/dev/null
fi

[[ "$status" == PASS ]] || exit 1
exit 0
