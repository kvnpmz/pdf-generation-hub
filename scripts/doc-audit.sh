#!/usr/bin/env bash
#
# doc-audit.sh - structural audit of one document config before rendering.
#
# Why: data edits (moving rows between sections, cloning a variant, renaming
# fields) can duplicate, empty or orphan entries without failing the render.
# This compiles the config to Lua, follows the extends chain, merges it the
# way the renderer does, and checks it mechanically. No template-specific
# assumptions: it only knows tables, identifiers and the config contract.
#
# Usage:
#   scripts/doc-audit.sh gout_food_list
#   scripts/doc-audit.sh --doc first_apartment_pink
#   scripts/doc-audit.sh                # DOCUMENTID from teal/runtime.tl
#
# Checks:
#   id / template / output_name / completed_date contract
#   template render.tl, style.css and overrides css exist
#   duplicate identifier inside one list (FAIL)
#   identifier reused across lists (INFO)
#   repeated scalar inside one list (WARN)
#   empty strings and empty lists (WARN)
#   entry counts for every list of records with a name/title/label/key/id
#
# Exit code: 1 if any FAIL, 0 otherwise (WARN does not fail).

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

usage() {
    sed -n '2,28p' "$0" | sed 's/^# \{0,1\}//'
}

DOC=""
while (($#)); do
    case "$1" in
        --doc) DOC="${2:?--doc needs a value}"; shift 2 ;;
        -h|--help) usage; exit 0 ;;
        -*) echo "doc-audit: unknown argument: $1" >&2; exit 2 ;;
        *) DOC="$1"; shift ;;
    esac
done

if [[ -z "$DOC" ]]; then
    DOC="$(sed -n 's/^[[:space:]]*DOCUMENTID[[:space:]]*=[[:space:]]*"\{0,1\}\([^",]*\)"\{0,1\}[[:space:]],\{0,1\}$/\1/p' \
        "$APP/teal/runtime.tl" | head -1)"
fi

if [[ -z "$DOC" ]]; then
    echo "doc-audit: no document given and none in teal/runtime.tl" >&2
    exit 2
fi

CONFIG="$APP/documents/$DOC/config.tl"
if [[ ! -f "$CONFIG" ]]; then
    echo "doc-audit: missing document config: documents/$DOC/config.tl" >&2
    exit 2
fi

TMP="$(mktemp -d "${TMPDIR:-/tmp}/doc-audit.XXXXXX")"
trap 'rm -rf "$TMP"' EXIT INT TERM HUP

# Build the extends chain, base first, with a cycle guard.
chain=()
seen=" "
current="$DOC"
while [[ -n "$current" ]]; do
    case " $seen " in
        *" $current "*)
            echo "doc-audit: $DOC"
            echo "  FAIL    extends cycle at $current"
            echo "  result  FAIL"
            exit 1
            ;;
    esac
    seen="$seen$current "
    chain=("$current" "${chain[@]}")

    next="$(sed -n 's/.*[[:space:]]extends[[:space:]]*=[[:space:]]*"\([^"]*\)".*/\1/p' \
        "$APP/documents/$current/config.tl" | head -1)"

    if [[ -n "$next" && ! -f "$APP/documents/$next/config.tl" ]]; then
        echo "doc-audit: $DOC"
        echo "  FAIL    extends $next, but documents/$next/config.tl is missing"
        echo "  result  FAIL"
        exit 1
    fi
    current="$next"
done

# Compile every config in the chain; a Teal syntax error is a hard failure.
for name in "${chain[@]}"; do
    if ! tl gen -q --output "$TMP/$name.lua" "$APP/documents/$name/config.tl" \
        >"$TMP/$name.out" 2>"$TMP/$name.err"; then
        echo "doc-audit: $DOC"
        echo "  FAIL    cannot compile documents/$name/config.tl"
        sed -n '1,6p' "$TMP/$name.err" | sed 's/^/          /'
        echo "  result  FAIL"
        exit 1
    fi
done

exec lua "$APP/scripts/doc-audit.lua" "$TMP" "$DOC" "$APP" "${chain[@]}"
