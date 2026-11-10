#!/usr/bin/env bash
#
# page-report.sh - design-agnostic whitespace and type-size report.
#
# Why: render-check answers "does this fit on one page?" but not "where is the
# dead space?" or "how small is the smallest text?". This reads the PDF/PNG a
# render already produced and reports both, without rendering again.
#
# It makes no assumptions about the template: no table, footer, column or
# checklist selectors. It measures blank horizontal bands and computed font
# sizes on any printable design, light or dark.
#
# Usage:
#   scripts/page-report.sh                       # runtime.tl document
#   scripts/page-report.sh --doc movein_budget
#   scripts/page-report.sh --pdf output/x/x.pdf
#   scripts/page-report.sh --png output/x/images/x.png
#   scripts/page-report.sh --doc x --json
#   scripts/page-report.sh --doc x --strict --min-gap 14 --min-font 8.5
#
# Exit code: 0 = report produced, 1 = --strict and a threshold was crossed.
# render-check.sh remains the authority on page count, fill and margins.
#
# Dependencies: ImageMagick, venv Python with Pillow (and WeasyPrint for the
# type pass). The type pass is skipped when no HTML file is available.

set -uo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ -x "$APP/venv/bin/python" ]]; then
    PYTHON="$APP/venv/bin/python"
else
    PYTHON="python3"
fi

exec "$PYTHON" - "$APP" "$@" <<'PY'
import json
import math
import os
import re
import subprocess
import sys
import tempfile
import warnings
from collections import Counter

warnings.filterwarnings("ignore")

APP = sys.argv[1]
ARGS = sys.argv[2:]

BORDER_SAMPLE_STEP = 7
INK_DELTA = 18


def usage() -> None:
    print(
        "usage: page-report.sh [--doc SLUG] [--pdf PATH] [--png PATH] "
        "[--html PATH] [--min-gap PT] [--min-font PT] [--top N] [--strict] [--json]"
    )
    sys.exit(0)


def parse_args(argv):
    opts = {
        "doc": "",
        "pdf": "",
        "png": "",
        "html": "",
        "min_gap": 14.0,
        "min_font": 8.0,
        "top": 5,
        "strict": False,
        "json": False,
    }
    i = 0
    while i < len(argv):
        arg = argv[i]
        if arg in ("-h", "--help"):
            usage()
        elif arg == "--doc":
            opts["doc"] = argv[i + 1]; i += 2
        elif arg == "--pdf":
            opts["pdf"] = argv[i + 1]; i += 2
        elif arg == "--png":
            opts["png"] = argv[i + 1]; i += 2
        elif arg == "--html":
            opts["html"] = argv[i + 1]; i += 2
        elif arg == "--min-gap":
            opts["min_gap"] = float(argv[i + 1]); i += 2
        elif arg == "--min-font":
            opts["min_font"] = float(argv[i + 1]); i += 2
        elif arg == "--top":
            opts["top"] = int(argv[i + 1]); i += 2
        elif arg == "--strict":
            opts["strict"] = True; i += 1
        elif arg == "--json":
            opts["json"] = True; i += 1
        else:
            print(f"page-report: unknown argument: {arg}", file=sys.stderr)
            sys.exit(2)
    return opts


def runtime_field(name: str) -> str:
    path = os.path.join(APP, "teal", "runtime.tl")
    try:
        text = open(path, encoding="utf-8").read()
    except OSError:
        return ""
    match = re.search(
        rf'^\s*{name}\s*=\s*"?([^",]+)"?\s*,?\s*$', text, re.MULTILINE
    )
    return match.group(1).strip() if match else ""


def config_field(doc: str, name: str) -> str:
    path = os.path.join(APP, "documents", doc, "config.tl")
    try:
        text = open(path, encoding="utf-8").read()
    except OSError:
        return ""
    match = re.search(rf'^\s*{name}\s*=\s*"([^"]*)"', text, re.MULTILINE)
    return match.group(1).strip() if match else ""


def output_paths(doc: str):
    output_name = config_field(doc, "output_name") or doc
    out_dir = os.path.join(APP, "output", doc)
    return (
        os.path.join(out_dir, f"{output_name}.pdf"),
        os.path.join(out_dir, "images", f"{output_name}.png"),
        os.path.join(out_dir, f"{output_name}.html"),
    )


def magick(*args: str) -> str:
    result = subprocess.run(
        ["magick", *args], capture_output=True, text=True, check=True
    )
    return result.stdout.strip()


def pdf_page_size(pdf: str):
    size = magick("identify", "-format", "%w %h", f"{pdf}[0]")
    width, height = (float(part) for part in size.split())
    pages = int(magick("identify", "-format", "%n", pdf).splitlines()[0] or 1)
    return width, height, pages


def png_page_size(png: str):
    from PIL import Image

    image = Image.open(png)
    dpi = image.info.get("dpi", (300, 300))
    try:
        dpi_x = float(dpi[0]) or 300.0
        dpi_y = float(dpi[1]) or 300.0
    except (TypeError, ValueError, IndexError):
        dpi_x = dpi_y = 300.0
    return (
        image.width * 72.0 / dpi_x,
        image.height * 72.0 / dpi_y,
        image.width,
        image.height,
    )


def background_colour(image):
    width, height = image.size
    samples = []
    for x in range(0, width, BORDER_SAMPLE_STEP):
        samples.append(image.getpixel((x, 0)))
        samples.append(image.getpixel((x, height - 1)))
    for y in range(0, height, BORDER_SAMPLE_STEP):
        samples.append(image.getpixel((0, y)))
        samples.append(image.getpixel((width - 1, y)))
    return Counter(samples).most_common(1)[0][0]


def row_ink_flags(png: str):
    from PIL import Image, ImageChops

    image = Image.open(png).convert("RGB")
    width, height = image.size
    background = background_colour(image)
    diff = ImageChops.difference(
        image, Image.new("RGB", image.size, background)
    ).convert("L")
    data = list(diff.getdata())
    flags = []
    for y in range(height):
        start = y * width
        flags.append(max(data[start:start + width]) > INK_DELTA)
    return flags


def gaps_for_page(png: str, page_w_pt: float, page_h_pt: float):
    flags = row_ink_flags(png)
    first = next((i for i, ink in enumerate(flags) if ink), None)
    last = next((i for i, ink in enumerate(reversed(flags)) if ink), None)
    if first is None or last is None:
        return []
    last = len(flags) - 1 - last

    scale = page_h_pt / len(flags)
    runs = []
    start = None
    for y in range(first, last + 1):
        if not flags[y] and start is None:
            start = y
        elif flags[y] and start is not None:
            runs.append((start, y - 1))
            start = None
    if start is not None:
        runs.append((start, last))

    return [
        {
            "y": round(a * scale, 1),
            "height": round((b - a + 1) * scale, 1),
        }
        for a, b in runs
    ]


def type_sizes(html: str):
    try:
        from weasyprint import HTML
        from weasyprint.formatting_structure import boxes
    except Exception as exc:
        return None, f"weasyprint unavailable: {exc}"

    try:
        document = HTML(filename=html).render()
    except Exception as exc:
        return None, f"html layout failed: {exc}"

    counts = Counter()
    char_total = 0

    def walk(box):
        nonlocal char_total
        if isinstance(box, boxes.TextBox):
            text = (box.text or "").strip()
            if text:
                size_pt = round(float(box.style["font_size"]) * 0.75, 1)
                counts[size_pt] += len(text)
                char_total += len(text)
        for child in getattr(box, "children", []) or []:
            walk(child)

    for page in document.pages:
        walk(page._page_box)

    sizes = [
        {"pt": size, "chars": chars}
        for size, chars in sorted(counts.items())
    ]
    return sizes, char_total


def fmt_gap_list(gaps, page_no, min_gap):
    parts = []
    for gap in gaps:
        mark = "!" if gap["height"] >= min_gap else ""
        parts.append(f'{gap["height"]:.1f}pt @ y={gap["y"]:.1f}{mark}')
    return f'page {page_no}: ' + ", ".join(parts) if parts else f"page {page_no}: none"


def main() -> int:
    opts = parse_args(ARGS)
    explicit_output = bool(opts["pdf"] or opts["png"] or opts["html"])
    doc = opts["doc"] or ("" if explicit_output else runtime_field("DOCUMENTID"))

    pdf = opts["pdf"]
    png = opts["png"]
    html = opts["html"]

    if doc:
        default_pdf, default_png, default_html = output_paths(doc)
        if not opts["pdf"] and not opts["png"]:
            pdf = default_pdf
            png = default_png
        html = html or default_html

    if not pdf and not png:
        print(
            "page-report: no PDF or PNG. Run render-check.sh first or pass --pdf/--png.",
            file=sys.stderr,
        )
        return 2

    if not os.path.exists(pdf):
        pdf = ""
    if not os.path.exists(png):
        png = ""
    if not os.path.exists(html):
        html = ""

    if not pdf and not png:
        print(
            "page-report: output not found. Run render-check.sh first.",
            file=sys.stderr,
        )
        return 2

    pages = 1
    page_w_pt = page_h_pt = 0.0
    temporary = None

    # Prefer the PDF: it is the printable artifact and gives page count/size.
    if pdf:
        page_w_pt, page_h_pt, pages = pdf_page_size(pdf)
        temporary = tempfile.TemporaryDirectory(prefix="page-report.")
        magick("-density", "200", pdf, os.path.join(temporary.name, "page-%d.png"))
        page_pngs = [
            os.path.join(temporary.name, f"page-{i}.png") for i in range(pages)
        ]
    else:
        page_w_pt, page_h_pt, _, _ = png_page_size(png)
        page_pngs = [png]

    all_gaps = []
    for index, page_png in enumerate(page_pngs, start=1):
        if not os.path.exists(page_png):
            continue
        gaps = gaps_for_page(page_png, page_w_pt, page_h_pt)
        gaps.sort(key=lambda item: item["height"], reverse=True)
        for gap in gaps:
            gap["page"] = index
        all_gaps.extend(gaps[: opts["top"]])

    sizes, type_note = (None, "")
    if html:
        sizes, type_note = type_sizes(html)
    else:
        type_note = "no html file; type pass skipped"

    warnings = []
    for gap in all_gaps:
        if gap["height"] >= opts["min_gap"]:
            warnings.append(
                f'gap {gap["height"]:.1f}pt on page {gap["page"]} '
                f'at y={gap["y"]:.1f} (min {opts["min_gap"]:.1f})'
            )

    if sizes:
        for item in sizes:
            if item["pt"] < opts["min_font"]:
                warnings.append(
                    f'text {item["pt"]:.1f}pt ({item["chars"]} chars) '
                    f'below {opts["min_font"]:.1f}'
                )

    result = {
        "doc": doc or os.path.basename(pdf or png),
        "pages": pages,
        "page_pt": [round(page_w_pt, 1), round(page_h_pt, 1)],
        "gaps": all_gaps,
        "fonts": sizes or [],
        "warnings": warnings,
        "pass": not warnings,
    }

    if opts["json"]:
        print(json.dumps(result, indent=2))
    else:
        name = result["doc"]
        print(
            f'page-report: {name}  ({pages} page'
            f'{"s" if pages != 1 else ""}, '
            f'{page_w_pt:.0f}x{page_h_pt:.0f} pt)'
        )
        if all_gaps:
            for page_no in sorted({gap["page"] for gap in all_gaps}):
                page_gaps = [gap for gap in all_gaps if gap["page"] == page_no]
                print(f'  gaps    {fmt_gap_list(page_gaps, page_no, opts["min_gap"])}')
        else:
            print("  gaps    none detected")
        if sizes:
            shown = ", ".join(
                f'{item["pt"]:.1f}pt ({item["chars"]} chars)' for item in sizes[:8]
            )
            print(f"  type    {shown}")
        else:
            print(f"  type    skipped ({type_note})")
        for warning in warnings:
            print(f"  warn    {warning}")
        print(f'  result  {"PASS" if not warnings else "WARN"}')

    if opts["strict"] and warnings:
        return 1
    return 0


try:
    sys.exit(main())
finally:
    pass
PY
