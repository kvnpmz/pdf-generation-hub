#!/usr/bin/env bash
#
# doc-colors.sh - what colour is this document, actually?
#
# Why: palette questions were answered by eyeballing render crops, which is
# slow and ambiguous ("is #2C6B45 green?"). This resolves the same cascade
# the renderer uses (style.css -> overrides), reports the accent as hex, RGB,
# hue and a plain colour name, and - when a PDF exists - confirms the colour
# that actually printed. With --template it lists every document on that
# template and flags two of them sharing an accent.
#
# Usage:
#   scripts/doc-colors.sh --doc split_budget_balanced_serif
#   scripts/doc-colors.sh --template split_budget_balanced   # all docs, dupes flagged
#   scripts/doc-colors.sh --hex '#2C6B45'                    # identify any colour
#   scripts/doc-colors.sh --doc x --json
#
# Notes:
#   --doc defaults to DOCUMENTID in teal/runtime.tl.
#   Palette names come from the :root block of the template stylesheet, so any
#   template works; the report leads with --accent and prints the rest below.
#   --pdf can point at an exported file when output/ is not where it lives.
#
# Exit code: 0 = report produced, 1 = document/stylesheet missing, 2 = usage.

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

DOC=""
TEMPLATE=""
HEX=""
PDF=""
JSON=0

usage() {
    sed -n '4,22p' "$0" | sed 's/^# \{0,1\}//'
}

while (($#)); do
    case "$1" in
        --doc) DOC="${2:?--doc needs a value}"; shift 2 ;;
        --template) TEMPLATE="${2:?--template needs a value}"; shift 2 ;;
        --hex) HEX="${2:?--hex needs a value}"; shift 2 ;;
        --pdf) PDF="${2:?--pdf needs a value}"; shift 2 ;;
        --json) JSON=1; shift ;;
        -h|--help) usage; exit 0 ;;
        *) echo "doc-colors: unknown argument: $1" >&2; exit 2 ;;
    esac
done

[[ -n "$HEX" || -n "$DOC" || -n "$TEMPLATE" ]] || { usage >&2; exit 2; }

exec python3 - "$APP" "$DOC" "$TEMPLATE" "$HEX" "$PDF" "$JSON" <<'PY'
import json
import os
import re
import sys
import zlib

APP, DOC, TEMPLATE, HEX, PDF, JSON = sys.argv[1:7]
JSON = JSON == "1"

NAMED = [
    (15, "red"), (40, "orange"), (65, "yellow"), (95, "yellow-green"),
    (155, "green"), (190, "teal"), (250, "blue"), (290, "violet"),
    (330, "plum"), (345, "pink"), (361, "red"),
]


def to_hex(rgb):
    return "#%02X%02X%02X" % rgb


def parse_hex(value):
    value = value.strip().lstrip("#")
    if not re.fullmatch(r"[0-9a-fA-F]{6}", value):
        sys.exit(f"doc-colors: not a 6-digit hex colour: {value}")
    return tuple(int(value[i:i + 2], 16) for i in (0, 2, 4))


def hue_name(hue):
    for limit, name in NAMED:
        if hue < limit:
            return name
    return "red"


def hue_of(rgb):
    r, g, b = [c / 255 for c in rgb]
    high, low = max(r, g, b), min(r, g, b)
    delta = high - low
    if delta == 0:
        return 0.0
    if high == r:
        hue = 60 * (((g - b) / delta) % 6)
    elif high == g:
        hue = 60 * ((b - r) / delta + 2)
    else:
        hue = 60 * ((r - g) / delta + 4)
    return round(hue % 360, 1)


def luminance(rgb):
    def channel(c):
        c = c / 255
        return c / 12.92 if c <= 0.03928 else ((c + 0.055) / 1.055) ** 2.4

    r, g, b = [channel(c) for c in rgb]
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def contrast(rgb, other):
    a, b = luminance(rgb), luminance(other)
    high, low = max(a, b), min(a, b)
    return round((high + 0.05) / (low + 0.05), 1)


def describe(rgb):
    hue = hue_of(rgb)
    return {
        "hex": to_hex(rgb),
        "rgb": "rgb(%d,%d,%d)" % rgb,
        "hue": hue,
        "name": hue_name(hue),
        "on_white": contrast(rgb, (255, 255, 255)),
        "on_black": contrast(rgb, (0, 0, 0)),
    }


def read(path):
    try:
        with open(path) as handle:
            return handle.read()
    except OSError:
        return ""


def config_field(slug, key):
    text = read(os.path.join(APP, "documents", slug, "config.tl"))
    match = re.search(r'^[ \t]*%s[ \t]*=[ \t]*"([^"]*)"' % key, text, re.M)
    return match.group(1) if match else ""


def chain(slug):
    """Document chain, leaf first: [(slug, overrides, style), ...]."""
    hops = []
    seen = set()
    while slug and slug not in seen and len(hops) < 6:
        seen.add(slug)
        hops.append((slug, config_field(slug, "overrides"), config_field(slug, "style")))

        # stop following when the config itself is missing
        if not os.path.isfile(os.path.join(APP, "documents", slug, "config.tl")):
            break
        slug = config_field(slug, "extends")
    return hops


def field_chain(slug, key):
    """First value of `key` walking the extends chain (leaf first)."""
    for hop_slug, _overrides, _style in chain(slug):
        value = config_field(hop_slug, key)
        if value:
            return value
    return ""


def template_of(slug):
    return field_chain(slug, "template") or slug


def root_vars(path):
    text = read(path)
    palette = {}
    for block in re.findall(r":root\s*\{(.*?)\}", text, re.S):
        for name, value in re.findall(r"(--[\w-]+)\s*:\s*([^;]+);", block):
            palette[name] = value.strip()
    return palette


ACCENT_KEYS = ("--accent", "--deep-blue", "--primary", "--accent-deep", "--brand")


def resolve(slug):
    hops = chain(slug)
    template = template_of(hops[0][0])
    palette = {}

    # base stylesheet first (doc `style` may name a different base file)
    base_style = hops[-1][2] or "style"
    palette.update(root_vars(os.path.join(APP, "templates", template, base_style + ".css")))

    # then override files from base to leaf
    for hop_slug, overrides, _style in reversed(hops):
        if overrides:
            palette.update(root_vars(os.path.join(APP, "templates", template, overrides + ".css")))

    accent_key = next((key for key in ACCENT_KEYS if key in palette), "")
    accent = palette.get(accent_key) if accent_key else ""
    return template, hops, palette, accent, accent_key


def pdf_colors(path):
    data = read_bytes(path)
    if not data:
        return None
    found = set()
    for stream in re.finditer(rb"stream\r?\n(.*?)endstream", data, re.S):
        try:
            body = zlib.decompress(stream.group(1))
        except zlib.error:
            continue
        for triple in re.findall(rb"([\d.]+ [\d.]+ [\d.]+) (?:rg|RG)", body):
            parts = [float(value) for value in triple.split()]
            found.add(to_hex(tuple(round(value * 255) for value in parts)))
    return found


def read_bytes(path):
    try:
        with open(path, "rb") as handle:
            return handle.read()
    except OSError:
        return b""


def doc_report(slug):
    template, hops, palette, accent, accent_key = resolve(slug)
    if not palette:
        sys.exit(f"doc-colors: no palette found for {slug} (template {template})")

    accent_info = describe(parse_hex(accent)) if accent else None

    result = {
        "doc": slug,
        "template": template,
        "extends": [hop[0] for hop in hops[1:]],
        "overrides": [hop[1] for hop in hops if hop[1]],
        "accent": accent_info,
        "palette": palette,
    }

    output_name = field_chain(slug, "output_name") or slug
    pdf_path = PDF or os.path.join(APP, "output", slug, output_name + ".pdf")
    rendered = pdf_colors(pdf_path) if os.path.isfile(pdf_path) else None
    if rendered is not None:
        result["pdf_path"] = os.path.relpath(pdf_path, APP)
        result["pdf_accent"] = accent.upper() in {value.upper() for value in rendered} if accent else None
        result["pdf_colors"] = sorted(rendered)

    if JSON:
        print(json.dumps(result, indent=2))
        return

    print(f"{slug}   (template {template})")
    if len(hops) > 1:
        print(f"  inherits  {' <- '.join(hop[0] for hop in hops)}")
    label = "accent" if accent_key == "--accent" else f"primary ({accent_key})"
    if accent_info:
        print(
            f"  {label:<9} {accent_info['hex']}  {accent_info['rgb']}  "
            f"hue {accent_info['hue']:.0f} deg  {accent_info['name']}  "
            f"white {accent_info['on_white']}:1"
        )
    else:
        print("  accent    (no accent-style variable in :root)")
    others = "  ".join(
        f"{name[2:]} {value}" for name, value in palette.items() if name != "--accent"
    )
    if others:
        print(f"  palette   {others}")
    if rendered is None:
        print("  printed   no pdf yet (render it first)")
    elif accent_info is None:
        print(f"  printed   {', '.join(result['pdf_colors'])}")
    else:
        mark = "matches" if result["pdf_accent"] else "NOT FOUND"
        subject = "accent" if accent_key == "--accent" else accent_key[2:]
        print(f"  printed   {subject} {mark} in {result['pdf_path']}")
        print(f"  in file   {', '.join(result['pdf_colors'])}")


def template_report(template):
    docs = sorted(
        name for name in os.listdir(os.path.join(APP, "documents"))
        if os.path.isdir(os.path.join(APP, "documents", name))
        and template_of(name) == template
    )
    rows = []
    for slug in docs:
        _template, _hops, _palette, accent, _key = resolve(slug)
        rows.append((slug, accent))

    counts = {}
    for _slug, accent in rows:
        if accent:
            counts[accent] = counts.get(accent, 0) + 1

    if JSON:
        print(json.dumps([
            {"doc": slug, "accent": accent, "duplicate": bool(accent and counts[accent] > 1)}
            for slug, accent in rows
        ], indent=2))
        return

    width = max(len(slug) for slug, _accent in rows) if rows else 8
    noun = "document" if len(rows) == 1 else "documents"
    print(f"{template}  ({len(rows)} {noun})")
    for slug, accent in rows:
        if accent:
            info = describe(parse_hex(accent))
            duplicate = "  DUPLICATE" if counts[accent] > 1 else ""
            print(
                f"  {slug:<{width}}  {info['hex']}  hue {info['hue']:>5.0f}  "
                f"{info['name']:<12}{duplicate}"
            )
        else:
            print(f"  {slug:<{width}}  (no --accent)")

    dupes = {accent for accent, count in counts.items() if count > 1}
    if dupes:
        print(f"  {len(dupes)} accent(s) shared by more than one document")
    else:
        print("  every document has its own accent")


def hex_report(value):
    info = describe(parse_hex(value))
    if JSON:
        print(json.dumps(info, indent=2))
        return
    print(
        f"{info['hex']}  {info['rgb']}  hue {info['hue']:.1f} deg  {info['name']}"
        f"  white {info['on_white']}:1  black {info['on_black']}:1"
    )


if HEX:
    hex_report(HEX)
elif TEMPLATE:
    template_report(TEMPLATE)
else:
    if not DOC:
        runtime = read(os.path.join(APP, "teal", "runtime.tl"))
        match = re.search(r'DOCUMENTID\s*=\s*"([^"]+)"', runtime)
        DOC = match.group(1) if match else ""
    if not os.path.isdir(os.path.join(APP, "documents", DOC)):
        sys.exit(f"doc-colors: no such document: {DOC}")
    doc_report(DOC)
PY
