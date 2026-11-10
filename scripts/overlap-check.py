#!/usr/bin/env python3
"""overlap-check.py - fail when two pieces of text are printed on top of each other.

Why: render-check measured page fill and margins, but a footer pinned with
flex could stay at the page bottom while enlarged content slid underneath it.
Page fit still "passed" because the footer's margin was fine, and the only
signal was text visibly colliding on the exported page.

This reads the exported PDF and compares word-level glyph boxes. Word boxes
come from real glyph positions, not CSS line boxes, so neighbouring lines,
tight leading and multi-line headings do not look like collisions. Only text
printed over other text is reported.

Usage:
  scripts/overlap-check.py output/<doc>/<name>.pdf
  scripts/overlap-check.py --doc <slug>        # resolve the current export
  scripts/overlap-check.py --json <pdf>
  scripts/overlap-check.py --min-area 12 <pdf> # ignore tiny intersections

Exit code: 0 = no collisions, 1 = collisions found, 2 = could not check.
"""

import argparse
import json
import os
import sys

APP = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

# Minimum intersection for a real overprint: a sliver of kerning contact
# between neighbouring glyphs must not count.
MIN_OVERLAP_X = 1.0  # pt
MIN_OVERLAP_Y = 1.5  # pt
MIN_SHARE = 0.15     # share of the smaller word's area


def resolve_pdf(doc: str) -> str:
    config = os.path.join(APP, "documents", doc, "config.tl")
    try:
        text = open(config, encoding="utf-8").read()
    except OSError:
        return ""
    name = ""
    for line in text.splitlines():
        line = line.strip()
        if line.startswith("output_name") and "=" in line:
            name = line.split("=", 1)[1].strip().strip('",')
            break
    name = name or doc
    return os.path.join(APP, "output", doc, f"{name}.pdf")


def page_words(page, page_height: float):
    """Return words with tight glyph boxes, in top-left coordinates."""
    textpage = page.get_textpage()
    count = textpage.count_chars()
    words = []
    current = None

    for index in range(count):
        char = textpage.get_text_range(index, 1)
        if not char:
            continue
        left, bottom, right, top = textpage.get_charbox(index)
        if char.isspace() or not char.isprintable() or right <= left or top <= bottom:
            if current:
                words.append(current)
                current = None
            continue

        y0 = page_height - top
        y1 = page_height - bottom
        # A hyphenated line break can join two visual lines in text order;
        # a baseline change means a new word, not a longer one.
        if current and abs((y0 + y1) / 2 - (current["y0"] + current["y1"]) / 2) > 3.0:
            words.append(current)
            current = None
        if current is None:
            current = {"text": char, "x0": left, "x1": right, "y0": y0, "y1": y1}
        else:
            current["text"] += char
            current["x0"] = min(current["x0"], left)
            current["x1"] = max(current["x1"], right)
            current["y0"] = min(current["y0"], y0)
            current["y1"] = max(current["y1"], y1)

    if current:
        words.append(current)
    return words


def find_collisions(pdf_path: str, min_area: float):
    import pypdfium2 as pdfium

    document = pdfium.PdfDocument(pdf_path)
    collisions = []

    for page_index in range(len(document)):
        page = document[page_index]
        _, page_height = page.get_size()
        words = page_words(page, page_height)
        words = [w for w in words if w["text"].strip()]
        words.sort(key=lambda w: w["y0"])

        seen = set()
        for i, first in enumerate(words):
            for second in words[i + 1:]:
                if second["y0"] >= first["y1"]:
                    break
                overlap_x = min(first["x1"], second["x1"]) - max(first["x0"], second["x0"])
                overlap_y = min(first["y1"], second["y1"]) - max(first["y0"], second["y0"])
                if overlap_x < MIN_OVERLAP_X or overlap_y < MIN_OVERLAP_Y:
                    continue
                area = overlap_x * overlap_y
                first_area = (first["x1"] - first["x0"]) * (first["y1"] - first["y0"])
                second_area = (second["x1"] - second["x0"]) * (second["y1"] - second["y0"])
                if area < max(min_area, min(first_area, second_area) * MIN_SHARE):
                    continue
                a, b = sorted((first["text"], second["text"]))
                key = (page_index + 1, a, b)
                if key in seen:
                    continue
                seen.add(key)
                collisions.append(
                    {
                        "page": page_index + 1,
                        "text_a": a[:40],
                        "text_b": b[:40],
                        "x": round(max(first["x0"], second["x0"]), 1),
                        "y": round(max(first["y0"], second["y0"]), 1),
                        "area": round(area, 1),
                    }
                )

    return len(document), collisions


def main() -> int:
    parser = argparse.ArgumentParser(add_help=True)
    parser.add_argument("pdf", nargs="?")
    parser.add_argument("--doc")
    parser.add_argument("--json", action="store_true")
    parser.add_argument("--min-area", type=float, default=8.0)
    args = parser.parse_args()

    pdf_path = args.pdf
    if not pdf_path and args.doc:
        pdf_path = resolve_pdf(args.doc)
    if not pdf_path or not os.path.isfile(pdf_path):
        print(f"overlap-check: no pdf to inspect: {pdf_path or '(none)'}", file=sys.stderr)
        return 2

    try:
        pages, collisions = find_collisions(pdf_path, args.min_area)
    except Exception as exc:  # pypdfium2 missing or unreadable PDF
        print(f"overlap-check: could not inspect pdf: {exc}", file=sys.stderr)
        return 2

    if args.json:
        print(
            json.dumps(
                {
                    "pdf": pdf_path,
                    "pages": pages,
                    "collisions": collisions,
                    "pass": not collisions,
                }
            )
        )
        return 0 if not collisions else 1

    print(f"overlap-check: {os.path.relpath(pdf_path, APP)}")
    if not collisions:
        print(f"  collisions 0 across {pages} page{'s' if pages != 1 else ''}")
        print("  RESULT      PASS")
        return 0

    for collision in collisions[:8]:
        print(
            f"  page {collision['page']}  "
            f"'{collision['text_a']}' x '{collision['text_b']}'  "
            f"at {collision['x']},{collision['y']}pt"
        )
    if len(collisions) > 8:
        print(f"  ... {len(collisions) - 8} more")
    print(f"  collisions {len(collisions)}")
    print("  RESULT      FAIL")
    return 1


if __name__ == "__main__":
    sys.exit(main())
