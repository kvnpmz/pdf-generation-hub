#!/usr/bin/env bash
#
# ship-check.sh - one finishing gate for a printable listing.
#
# Why: a finished document is three checks in sequence (page fit, type/spacing,
# listing copy). Running them separately means reading three formats and
# remembering what was skipped. This runs all three and prints one block.
# It assumes nothing about the template or the niche: every check is delegated
# to the existing scripts, so any Teal or C# document works.
#
# Usage:
#   scripts/ship-check.sh                      # DOCUMENTID from teal/runtime.tl
#   scripts/ship-check.sh --doc gout_food_list
#   scripts/ship-check.sh --doc first_apartment_pink --images
#   scripts/ship-check.sh --doc x --json
#
# Checks:
#   data   doc-audit.sh     duplicates, empty values, config contract (FAIL blocks)
#   fit    render-check.sh  pages, fill, bottom margin (FAIL blocks the ship)
#   type   page-report.sh   smallest font size and blank bands (WARN only)
#   copy   listing-check.sh 30-word title / 13 tags / 1 sentence (if present)
#
# Exit code: 1 if fit or copy fails, 0 otherwise (type warnings do not fail).

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

DOC=""
IMAGES=0
JSON=0

usage() {
    sed -n '2,25p' "$0" | sed 's/^# \{0,1\}//'
}

while (($#)); do
    case "$1" in
        --doc) DOC="${2:?--doc needs a value}"; shift 2 ;;
        --images) IMAGES=1; shift ;;
        --json) JSON=1; shift ;;
        -h|--help) usage; exit 0 ;;
        *) echo "ship-check: unknown argument: $1" >&2; exit 2 ;;
    esac
done

if [[ -z "$DOC" ]]; then
    DOC="$(sed -n 's/^[[:space:]]*DOCUMENTID[[:space:]]*=[[:space:]]*"\{0,1\}\([^",]*\)"\{0,1\}[[:space:]],\{0,1\}$/\1/p' \
        "$APP/teal/runtime.tl" | head -1)"
fi

if [[ -z "$DOC" ]]; then
    echo "ship-check: no document given and none in teal/runtime.tl" >&2
    exit 2
fi

if [[ ! -f "$APP/documents/$DOC/config.tl" ]]; then
    echo "ship-check: missing document config: documents/$DOC/config.tl" >&2
    exit 2
fi

TMP="$(mktemp -d "${TMPDIR:-/tmp}/ship-check.XXXXXX")"
trap 'rm -rf "$TMP"' EXIT INT TERM HUP

# ---------- data ----------

DATA_STATUS="PASS"
DATA_DETAIL=""

if "$APP/scripts/doc-audit.sh" --doc "$DOC" >"$TMP/audit.out" 2>&1; then
    AUDIT_RESULT="$(sed -n 's/^  result  *//p' "$TMP/audit.out" | head -1)"
    [[ "$AUDIT_RESULT" == "WARN" ]] && DATA_STATUS="WARN"
else
    DATA_STATUS="FAIL"
fi

AUDIT_LISTS="$(sed -n 's/^  lists   //p' "$TMP/audit.out" | head -1)"
if [[ "$DATA_STATUS" == "FAIL" ]]; then
    DATA_DETAIL="$(sed -n 's/^  FAIL    //p' "$TMP/audit.out" | head -1)"
    [[ -z "$DATA_DETAIL" ]] && DATA_DETAIL="doc-audit failed"
else
    DATA_DETAIL="${AUDIT_LISTS:-structure ok}"
fi

# ---------- fit ----------

render_args=(--doc "$DOC")
if [[ "$IMAGES" -eq 1 ]]; then
    render_args+=(--images)
fi

if "$APP/scripts/render-check.sh" "${render_args[@]}" >"$TMP/render.out" 2>&1; then
    FIT_STATUS="PASS"
else
    FIT_STATUS="FAIL"
fi

PAGES="$(sed -n 's/^  pages  *//p' "$TMP/render.out" | head -1)"
FILL="$(sed -n 's/^  fill  *\([0-9.]*\)%.*/\1/p' "$TMP/render.out" | head -1)"
BOTTOM="$(sed -n 's/.*bottom \([0-9.]*\).*/\1/p' "$TMP/render.out" | head -1)"

if [[ "$FIT_STATUS" == "PASS" ]]; then
    FIT_DETAIL="${PAGES:-?} page"
    [[ "$PAGES" != "1" ]] && FIT_DETAIL="${PAGES:-?} pages"
    FIT_DETAIL="$FIT_DETAIL · fill ${FILL:-?}% · bottom ${BOTTOM:-?}pt"
else
    FIT_DETAIL="render-check failed; see below"
fi

# ---------- type / spacing ----------

TYPE_STATUS="SKIP"
TYPE_DETAIL="render did not complete"

if [[ "$FIT_STATUS" == "PASS" ]]; then
    if "$APP/scripts/page-report.sh" --doc "$DOC" >"$TMP/page.out" 2>&1; then
        PAGE_RESULT="$(sed -n 's/^  result  *//p' "$TMP/page.out" | head -1)"
        if [[ -z "$PAGE_RESULT" || "$PAGE_RESULT" == "PASS" ]]; then
            TYPE_STATUS="PASS"
        else
            TYPE_STATUS="WARN"
        fi
    else
        rc=$?
        if (( rc >= 2 )); then TYPE_STATUS="SKIP"; else TYPE_STATUS="WARN"; fi
    fi

    MIN_FONT="$(sed -n 's/^  type  *\([0-9.]*\)pt.*/\1/p' "$TMP/page.out" | head -1)"
    GAPS_RAW="$(sed -n 's/^  gaps  *//p' "$TMP/page.out" | head -1)"
    if [[ -z "$GAPS_RAW" || "$GAPS_RAW" == none* ]]; then
        GAPS="none"
    else
        GAP_MAX="$(printf '%s\n' "$GAPS_RAW" | sed 's/,/\n/g' | sed -n 's/.* \([0-9.]*\)pt @.*/\1/p' | sort -n | tail -1)"
        GAP_COUNT="$(printf '%s\n' "$GAPS_RAW" | sed 's/,/\n/g' | sed -n 's/.* [0-9.]*pt @.*/x/p' | wc -l | tr -d ' ')"
        if [[ -n "$GAP_MAX" ]]; then
            GAPS="${GAP_MAX}pt max, ${GAP_COUNT} gaps"
        else
            GAPS="$GAPS_RAW"
        fi
    fi
    TYPE_DETAIL="min ${MIN_FONT:-?}pt · gaps ${GAPS:-?}"
fi

# ---------- listing copy ----------

COPY_STATUS="SKIP"
COPY_DETAIL="no listing.txt"

if [[ -f "$APP/documents/$DOC/listing.txt" ]]; then
    if "$APP/scripts/listing-check.sh" "documents/$DOC" >"$TMP/listing.out" 2>&1; then
        COPY_STATUS="PASS"
    else
        COPY_STATUS="FAIL"
    fi
    COVERAGE="$(sed -n 's/^  INFO   tags cover //p' "$TMP/listing.out" | head -1)"
    if [[ "$COPY_STATUS" == "PASS" ]]; then
        COPY_DETAIL="${COVERAGE:-all rules pass}"
    else
        COPY_DETAIL="$(sed -n 's/^  FAIL   //p' "$TMP/listing.out" | head -1)"
        [[ -z "$COPY_DETAIL" ]] && COPY_DETAIL="listing-check failed"
    fi
fi

# ---------- result ----------

RESULT="PASS"
if [[ "$DATA_STATUS" == "FAIL" || "$FIT_STATUS" == "FAIL" || "$COPY_STATUS" == "FAIL" ]]; then
    RESULT="FAIL"
elif [[ "$TYPE_STATUS" == "WARN" ]]; then
    RESULT="WARN"
fi

if [[ "$JSON" -eq 1 ]]; then
    DOC="$DOC" DATA_STATUS="$DATA_STATUS" DATA_DETAIL="$DATA_DETAIL" \
    FIT_STATUS="$FIT_STATUS" FIT_DETAIL="$FIT_DETAIL" \
    TYPE_STATUS="$TYPE_STATUS" TYPE_DETAIL="$TYPE_DETAIL" \
    COPY_STATUS="$COPY_STATUS" COPY_DETAIL="$COPY_DETAIL" RESULT="$RESULT" \
    python3 - <<'PY'
import json
import os

print(json.dumps({
    "doc": os.environ["DOC"],
    "data": {"status": os.environ["DATA_STATUS"], "detail": os.environ["DATA_DETAIL"]},
    "fit": {"status": os.environ["FIT_STATUS"], "detail": os.environ["FIT_DETAIL"]},
    "type": {"status": os.environ["TYPE_STATUS"], "detail": os.environ["TYPE_DETAIL"]},
    "copy": {"status": os.environ["COPY_STATUS"], "detail": os.environ["COPY_DETAIL"]},
    "result": os.environ["RESULT"],
}, ensure_ascii=False))
PY
else
    echo "ship-check: $DOC"
    printf '  %-8s %-6s %s\n' "data" "$DATA_STATUS" "$DATA_DETAIL"
    printf '  %-8s %-6s %s\n' "fit" "$FIT_STATUS" "$FIT_DETAIL"
    printf '  %-8s %-6s %s\n' "type" "$TYPE_STATUS" "$TYPE_DETAIL"
    printf '  %-8s %-6s %s\n' "copy" "$COPY_STATUS" "$COPY_DETAIL"
    printf '  %-8s %s\n' "result" "$RESULT"

    if [[ "$DATA_STATUS" == "FAIL" ]]; then
        echo "  doc-audit:"
        sed -n 's/^  \(FAIL\|WARN\)/    \1/p' "$TMP/audit.out"
    fi

    if [[ "$FIT_STATUS" == "FAIL" ]]; then
        echo "  render-check:"
        sed 's/^/    /' "$TMP/render.out"
    fi

    if [[ "$COPY_STATUS" == "FAIL" ]]; then
        echo "  listing-check:"
        sed -n 's/^  \(FAIL\|WARN\)/    \1/p' "$TMP/listing.out"
    fi

    if [[ "$TYPE_STATUS" == "WARN" ]]; then
        FIRST_WARN="$(sed -n 's/^  warn    //p' "$TMP/page.out" | head -1)"
        WARN_COUNT="$(sed -n '/^  warn    /p' "$TMP/page.out" | wc -l | tr -d ' ')"
        if (( WARN_COUNT > 1 )); then
            echo "  type warning: ${FIRST_WARN} (+$((WARN_COUNT - 1)) more)"
        else
            echo "  type warning: ${FIRST_WARN}"
        fi
    fi
fi

[[ "$RESULT" == "FAIL" ]] && exit 1
exit 0
