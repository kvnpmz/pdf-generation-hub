#!/usr/bin/env bash
#
# opportunity-report.sh - sales + catalogue coverage report.
#
# Why: picking the next listing used to mean several ad-hoc queries against
# sales.csv plus a manual cross-check of documents/. This prints the same
# numbers deterministically, capped at a fixed number of lines.
#
# Usage:
#   scripts/opportunity-report.sh                  # 30-day window, top 12 families
#   scripts/opportunity-report.sh --days 14
#   scripts/opportunity-report.sh --top 8 --titles 5
#   scripts/opportunity-report.sh --min-rev 50     # hide low-revenue families
#   scripts/opportunity-report.sh --json           # machine-readable
#
# Columns:
#   FAMILY     keyword family (rules live below, reviewed once)
#   UNITS/REV  lifetime units and revenue for that family
#   TITLES     distinct sales titles in the family
#   DOCS       documents/<slug> folders for that family
#   REV/DOC    revenue per existing document
#   WINDOW     units sold in the --days window
#
# Exit code: 0 always (report, not a check).

set -euo pipefail

APP="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

exec python3 - "$APP" "$@" <<'PY'
import csv
import glob
import json
import os
import re
import sys
from datetime import date, timedelta

# Keyword families, first match wins. Order matters: a title that mentions
# several families belongs to the most specific one (glp-1 before heart,
# med diet before heart, cleaning before grocery/budget, etc).
RULES = [
    ("kidney / renal", ["kidney", "renal", "ckd", "potassium"]),
    ("diverticulitis", ["diverticul"]),
    ("fatty liver", ["fatty liver", "fattyliver", "nafld", "nash", "steatosis", "liver"]),
    ("gallbladder", ["gallbladder", "gallstone", "cholecystitis"]),
    ("low carb / keto", ["low carb", "lowcarb", "keto", "atkins", "carnivore"]),
    ("folic acid", ["folic"]),
    ("blood sugar", ["blood sugar", "bloodsugar", "glucose", "diabet"]),
    ("mediterranean", ["mediterranean"]),
    ("glp-1", ["glp-1", "glp1", "glp 1"]),
    ("kids / adhd", ["adhd", "brain food"]),
    ("baby", ["baby", "newborn", "infant", "nursery", "first foods"]),
    ("wedding", ["wedding", "bridal", "bride"]),
    ("heart health", ["heart"]),
    ("high protein", ["protein"]),
    ("emergency / survival", ["emergency", "survival", "72-hour", "72 hour", "72hour"]),
    ("cleaning / upkeep", ["clean", "maintenance", "repair"]),
    ("grocery / meal", ["grocery", "grocer", "meal plan", "pantry", "food list", "foodlist", "stocktake"]),
    ("budget / finance", ["budget", "finance", "expense", "paycheck", "bill", "saving", "investment", "debt", "monthly"]),
    ("home / move", ["apartment", "dorm", "college", "moving", "move-in", "move in", "movein", "new home", "first home", "household", "house"]),
    ("travel", ["travel", "packing", "vacation"]),
    ("planner / productivity", ["planner", "schedule", "time block", "productivity", "weekly plan"]),
]

# Dev fixtures that are not products.
EXCLUDED_DOCS = {"example_checklist", "grid_demo", "layout_probe"}


def family_of(text: str) -> str:
    haystack = text.lower().replace("_", " ").replace("-", "-")
    haystack = re.sub(r"\s+", " ", haystack)
    for family, keywords in RULES:
        for keyword in keywords:
            if keyword in haystack:
                return family
    return "other"


def usage() -> None:
    print("usage: opportunity-report.sh [--days N] [--top N] [--titles N] [--min-rev N] [--json]")
    sys.exit(0)


def main() -> int:
    app = sys.argv[1]
    args = sys.argv[2:]

    days = 30
    top = 12
    title_limit = 10
    min_rev = 0.0
    as_json = False

    i = 0
    while i < len(args):
        arg = args[i]
        if arg in ("-h", "--help"):
            usage()
        elif arg == "--days":
            days = int(args[i + 1]); i += 2
        elif arg == "--top":
            top = int(args[i + 1]); i += 2
        elif arg == "--titles":
            title_limit = int(args[i + 1]); i += 2
        elif arg == "--min-rev":
            min_rev = float(args[i + 1]); i += 2
        elif arg == "--json":
            as_json = True; i += 1
        else:
            print(f"opportunity-report: unknown argument: {arg}", file=sys.stderr)
            return 2

    sales_path = os.path.join(app, "sales.csv")
    if not os.path.exists(sales_path):
        print(f"opportunity-report: missing {sales_path}", file=sys.stderr)
        return 2

    def parse_date(value: str):
        month, day, year = value.split("/")
        return date(2000 + int(year), int(month), int(day))

    rows = []
    with open(sales_path, newline="", encoding="utf-8") as handle:
        for row in csv.DictReader(handle):
            try:
                quantity = int(row["Quantity"])
                price = float(row["Price"])
                stamp = parse_date(row["Sale Date"])
            except (KeyError, TypeError, ValueError):
                continue
            rows.append((stamp, row["Item Name"].strip(), quantity, price))

    if not rows:
        print("opportunity-report: no usable rows in sales.csv", file=sys.stderr)
        return 2

    cutoff = max(stamp for stamp, *_ in rows) - timedelta(days=days)
    families = {}
    titles = {}

    for stamp, name, quantity, price in rows:
        family = family_of(name)
        entry = families.setdefault(
            family, {"units": 0, "revenue": 0.0, "titles": set(), "window_units": 0}
        )
        entry["units"] += quantity
        entry["revenue"] += quantity * price
        entry["titles"].add(name)
        if stamp >= cutoff:
            entry["window_units"] += quantity

        title = titles.setdefault(
            name, {"family": family, "units": 0, "revenue": 0.0, "window_units": 0}
        )
        title["units"] += quantity
        title["revenue"] += quantity * price
        if stamp >= cutoff:
            title["window_units"] += quantity

    docs = {}
    for config_path in sorted(glob.glob(os.path.join(app, "documents", "*", "config.tl"))):
        document = os.path.basename(os.path.dirname(config_path))
        if document in EXCLUDED_DOCS:
            continue
        try:
            text = open(config_path, encoding="utf-8").read()
        except OSError:
            continue
        header = re.search(r'\bheader\s*=\s*"([^"]*)"', text)
        output = re.search(r'\boutput_name\s*=\s*"([^"]*)"', text)
        haystack = " ".join(
            part for part in (document, header.group(1) if header else "", output.group(1) if output else "") if part
        )
        family = family_of(haystack)
        docs[family] = docs.get(family, 0) + 1

    for family, entry in families.items():
        entry["docs"] = docs.get(family, 0)
        entry["rev_per_doc"] = entry["revenue"] / entry["docs"] if entry["docs"] else 0.0

    ordered = sorted(families.items(), key=lambda item: (-item[1]["revenue"], item[0]))
    ordered = [(name, entry) for name, entry in ordered if entry["revenue"] >= min_rev]

    top_titles = sorted(titles.items(), key=lambda item: (-item[1]["revenue"], item[0]))

    if as_json:
        payload = {
            "window_days": days,
            "families": [
                {
                    "family": name,
                    "units": entry["units"],
                    "revenue": round(entry["revenue"], 2),
                    "titles": len(entry["titles"]),
                    "docs": entry["docs"],
                    "revenue_per_doc": round(entry["rev_per_doc"], 2),
                    "window_units": entry["window_units"],
                }
                for name, entry in ordered[:top]
            ],
            "titles": [
                {
                    "title": name,
                    "family": entry["family"],
                    "units": entry["units"],
                    "revenue": round(entry["revenue"], 2),
                    "window_units": entry["window_units"],
                }
                for name, entry in top_titles[:title_limit]
            ],
        }
        print(json.dumps(payload, indent=2))
        return 0

    total_revenue = sum(entry["revenue"] for entry in families.values())
    total_units = sum(entry["units"] for entry in families.values())
    window_units = sum(entry["window_units"] for entry in families.values())

    print(f"sales.csv  {len(rows)} sales  {total_units} units  ${total_revenue:,.2f}  "
          f"window: last {days} days  {window_units} units")
    print()

    header = f"{'FAMILY':24s} {'UNITS':>6s} {'REV':>9s} {'TITLES':>7s} {'DOCS':>5s} {'REV/DOC':>8s} {'WINDOW':>7s}"
    print(header)
    print("-" * len(header))
    for name, entry in ordered[:top]:
        print(
            f"{name:24s} {entry['units']:6d} {entry['revenue']:9.2f} {len(entry['titles']):7d} "
            f"{entry['docs']:5d} {entry['rev_per_doc']:8.1f} {entry['window_units']:7d}"
        )

    print()
    print(f"TOP TITLES (by revenue, top {title_limit})")
    for name, entry in top_titles[:title_limit]:
        short = name if len(name) <= 72 else name[:69] + "..."
        print(f"  {entry['revenue']:7.2f} {entry['units']:3d}u {entry['window_units']:3d}w  {short}")

    unmatched = [name for name, entry in titles.items() if entry["family"] == "other"]
    if unmatched:
        print()
        print(f"UNMATCHED ({len(unmatched)}) - add keywords to RULES above:")
        for name in sorted(unmatched)[:8]:
            print(f"  {name[:96]}")

    return 0


sys.exit(main())
PY
