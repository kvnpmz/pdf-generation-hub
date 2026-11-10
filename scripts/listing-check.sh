#!/usr/bin/env bash
#
# listing-check.sh - validate documents/<slug>/listing.txt against
# listing_prompt.txt before it reaches Etsy.
#
# Why: the listing rules (30-word title, 13 tags, one sentence) were being
# counted by hand after every render, and a miscount costs a full round trip
# to fix. This checks the same rules deterministically in one pass.
#
# Usage:
#   scripts/listing-check.sh documents/carryon_packing
#   scripts/listing-check.sh documents/carryon_packing/listing.txt
#
# Rules (from listing_prompt.txt):
#   TITLE        exactly 30 words, pipe separators
#   TAGS         exactly 13, unique, 2-3 words, lowercase
#   DESCRIPTION  exactly one sentence, not starting with "This",
#                and without the word "clean"
#   Tags longer than Etsy's 20-character cap are reported as warnings.
#   Tag word coverage (unique/total) is printed; repeated words warn.
#
# Exit code: 0 = all checked listings pass, 1 = at least one failed.

set -euo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

exec python3 - "$APP" "$@" <<'PY'
import re
import sys
from pathlib import Path

SECTIONS = ("TITLE", "TAGS", "DESCRIPTION")


def parse_listing(path: Path) -> dict[str, str]:
    sections: dict[str, list[str]] = {}
    current = None
    for raw in path.read_text().splitlines():
        line = raw.strip()
        if not line:
            continue
        if line.upper() in SECTIONS:
            current = line.upper()
            sections.setdefault(current, [])
            continue
        if current:
            sections[current].append(line)
    return {key: " ".join(lines) for key, lines in sections.items()}


def check(path: Path) -> list[str]:
    errors: list[str] = []
    warnings: list[str] = []
    data = parse_listing(path)

    for section in SECTIONS:
        if not data.get(section):
            errors.append(f"{section} section is missing or empty")

    title = data.get("TITLE", "")
    words = [word for word in re.split(r"[|\s]+", title) if word]
    if len(words) != 30:
        errors.append(f"title has {len(words)} words, expected 30")
    if "|" not in title:
        errors.append("title has no pipe separators")

    tags = [tag.strip() for tag in data.get("TAGS", "").split(",") if tag.strip()]
    if len(tags) != 13:
        errors.append(f"{len(tags)} tags, expected 13")
    if len(set(tags)) != len(tags):
        errors.append("tags are not unique")
    for tag in tags:
        if len(tag.split()) not in (2, 3):
            errors.append(f"tag {tag!r} has {len(tag.split())} words, expected 2-3")
        if tag != tag.lower():
            errors.append(f"tag {tag!r} is not lowercase")
        if len(tag) > 20:
            warnings.append(f"tag {tag!r} is {len(tag)} chars (Etsy cap is 20)")

    tag_words = [word for tag in tags for word in tag.split()]
    unique_words = len(set(tag_words))
    print(f"  INFO   tags cover {unique_words}/{len(tag_words)} unique words")
    repeated = sorted({word for word in tag_words if tag_words.count(word) > 1})
    if repeated:
        warnings.append(f"tag words repeat: {', '.join(repeated)}")

    description = data.get("DESCRIPTION", "")
    endings = re.findall(r"[.!?](?=\s|$)", description)
    if len(endings) != 1 or not description.rstrip().endswith((".", "!", "?")):
        errors.append(f"description has {len(endings)} sentence endings, expected 1")
    if description.lower().startswith("this"):
        errors.append("description starts with 'This'")
    if re.search(r"\bclean\b", description, re.IGNORECASE):
        errors.append("description contains the word 'clean'")

    for warning in warnings:
        print(f"  WARN   {warning}")
    for error in errors:
        print(f"  FAIL   {error}")
    return errors


def resolve(app: Path, arg: str) -> Path:
    path = Path(arg)
    if not path.is_absolute():
        path = app / path
    if path.is_dir():
        path = path / "listing.txt"
    return path


def main() -> int:
    app = Path(sys.argv[1])
    args = sys.argv[2:]

    if not args or args[0] in ("-h", "--help"):
        print("usage: listing-check.sh documents/<slug>[.txt]")
        return 0

    paths = [resolve(app, args[0])]

    failed = 0
    for path in paths:
        if not path.exists():
            print(f"{path}: not found")
            failed += 1
            continue
        print(path.relative_to(app) if path.is_relative_to(app) else path)
        errors = check(path)
        if errors:
            failed += 1
        else:
            print("  OK     30-word title, 13 tags, 1-sentence description")

    print(f"\n{len(paths) - failed}/{len(paths)} listings pass")
    return 1 if failed else 0


sys.exit(main())
PY
