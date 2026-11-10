#!/usr/bin/env bash
#
# coverage-report.sh - sales demand against the existing catalogue, per family.
#
# Why: the opportunity report shows which families sell, but not what already
# exists in documents/ and templates/. Cross-checking that used to take
# several ls/rg calls per session. This prints both sides in one pass, and
# --search answers "has anyone made an X?" for a candidate gap.
#
# Usage:
#   scripts/coverage-report.sh                      # 30-day window
#   scripts/coverage-report.sh --days 14
#   scripts/coverage-report.sh --family travel      # substring match
#   scripts/coverage-report.sh --docs 4             # docs per family (default 6, 0 = all)
#   scripts/coverage-report.sh --search "restaurant,eating out,deposit"
#   scripts/coverage-report.sh --json               # machine-readable
#
# Columns:
#   FAMILY     keyword family (rules live in scripts/families.py)
#   UNITS/REV  lifetime units and revenue for that family
#   TITLES     distinct sales titles in the family
#   DOCS       documents/<slug> folders for that family
#   TMPL       templates/<slug> folders for that family
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
from datetime import datetime, timedelta

APP = sys.argv[1]
sys.dont_write_bytecode = True
sys.path.insert(0, os.path.join(APP, "scripts"))
from families import EXCLUDED_DOCS, family_of

# Infrastructure folders that are not sellable templates.
EXCLUDED_TEMPLATES = {"checklist", "components", "default", "layout_probe", "tabbed_checklist"}


def usage() -> None:
    print("usage: coverage-report.sh [--days N] [--family NAME] [--docs N] [--search 'a,b'] [--json]")
    sys.exit(0)


def load_sales(days: int):
    rows = []
    path = os.path.join(APP, "sales.csv")
    with open(path, newline="", encoding="utf-8") as handle:
        for record in csv.DictReader(handle):
            try:
                stamp = datetime.strptime(record["Sale Date"].strip(), "%m/%d/%y").date()
                quantity = int(record["Quantity"])
                price = float(record["Price"])
            except (KeyError, TypeError, ValueError):
                continue
            rows.append((stamp, record["Item Name"], quantity, price))

    if not rows:
        return rows, None

    cutoff = max(row[0] for row in rows) - timedelta(days=days)
    return rows, cutoff


def load_documents():
    documents = []
    pattern = os.path.join(APP, "documents", "*", "config.tl")
    for config_path in sorted(glob.glob(pattern)):
        slug = os.path.basename(os.path.dirname(config_path))
        if slug in EXCLUDED_DOCS:
            continue
        try:
            text = open(config_path, encoding="utf-8").read()
        except OSError:
            continue
        header = re.search(r'\bheader\s*=\s*"([^"]*)"', text)
        output = re.search(r'\boutput_name\s*=\s*"([^"]*)"', text)
        completed = re.search(r'\bcompleted_date\s*=\s*"([^"]*)"', text)
        documents.append(
            {
                "slug": slug,
                "header": header.group(1) if header else "",
                "output_name": output.group(1) if output else "",
                "completed_date": completed.group(1) if completed else "",
                "text": text,
            }
        )

    # Style variants only carry id/overrides/extends; inherit the fields that
    # describe the underlying product so every row is identifiable.
    by_slug = {doc["slug"]: doc for doc in documents}

    def inherited(doc, field):
        seen = set()
        current = doc
        while current and current["slug"] not in seen:
            seen.add(current["slug"])
            if current[field]:
                return current[field]
            match = re.search(r'\bextends\s*=\s*"([^"]+)"', current["text"])
            current = by_slug.get(match.group(1)) if match else None
        return ""

    for doc in documents:
        for field in ("header", "output_name", "completed_date"):
            doc[field] = inherited(doc, field)
        doc["family"] = family_of(
            " ".join(part for part in (doc["slug"], doc["header"], doc["output_name"]) if part)
        )
    return documents


def load_templates():
    templates = []
    for directory in sorted(glob.glob(os.path.join(APP, "templates", "*"))):
        if not os.path.isdir(directory):
            continue
        slug = os.path.basename(directory)
        if slug in EXCLUDED_TEMPLATES:
            continue
        chunks = []
        for name in ("render.tl", "render.cs"):
            path = os.path.join(directory, name)
            if os.path.exists(path):
                try:
                    chunks.append(open(path, encoding="utf-8").read())
                except OSError:
                    pass
        templates.append(
            {
                "slug": slug,
                "text": "\n".join(chunks),
                "family": family_of(slug),
            }
        )
    return templates


def main() -> int:
    args = sys.argv[2:]
    days = 30
    docs_cap = 6
    family_filter = ""
    search_terms = []
    as_json = False

    index = 0
    while index < len(args):
        arg = args[index]
        if arg in ("-h", "--help"):
            usage()
        elif arg == "--days" and index + 1 < len(args):
            days = int(args[index + 1])
            index += 2
        elif arg == "--docs" and index + 1 < len(args):
            docs_cap = int(args[index + 1])
            index += 2
        elif arg == "--family" and index + 1 < len(args):
            family_filter = args[index + 1].lower()
            index += 2
        elif arg == "--search" and index + 1 < len(args):
            search_terms = [term.strip() for term in args[index + 1].split(",") if term.strip()]
            index += 2
        elif arg == "--json":
            as_json = True
            index += 1
        else:
            print(f"unknown argument: {arg}", file=sys.stderr)
            return 2

    rows, cutoff = load_sales(days)
    documents = load_documents()
    templates = load_templates()

    if search_terms:
        results = []
        for term in search_terms:
            needle = term.lower()
            doc_hits = [doc for doc in documents if needle in doc["text"].lower()]
            template_hits = [template for template in templates if needle in template["text"].lower()]
            results.append({"term": term, "documents": doc_hits, "templates": template_hits})

        if as_json:
            print(
                json.dumps(
                    {
                        "searched": {"documents": len(documents), "templates": len(templates)},
                        "results": [
                            {
                                "term": result["term"],
                                "documents": [doc["slug"] for doc in result["documents"]],
                                "templates": [template["slug"] for template in result["templates"]],
                            }
                            for result in results
                        ],
                    },
                    indent=2,
                )
            )
            return 0

        print(f"coverage-report: searching {len(documents)} documents, {len(templates)} templates")
        print()
        for result in results:
            doc_hits = result["documents"]
            template_hits = result["templates"]
            print(f"  {result['term']:<24} {len(doc_hits):2d} documents  {len(template_hits):2d} templates")
            for doc in doc_hits:
                print(f"      documents/{doc['slug']}")
            for template in template_hits:
                print(f"      templates/{template['slug']}")
        print()
        return 0

    families = {}

    def entry_for(name: str):
        return families.setdefault(
            name,
            {"units": 0, "revenue": 0.0, "titles": set(), "window_units": 0, "docs": [], "templates": []},
        )

    for stamp, name, quantity, price in rows:
        entry = entry_for(family_of(name))
        entry["units"] += quantity
        entry["revenue"] += quantity * price
        entry["titles"].add(name)
        if cutoff is not None and stamp >= cutoff:
            entry["window_units"] += quantity

    for doc in documents:
        entry_for(doc["family"])["docs"].append(doc)

    for template in templates:
        entry_for(template["family"])["templates"].append(template["slug"])

    ordered = sorted(families.items(), key=lambda item: (-item[1]["revenue"], item[0]))
    if family_filter:
        ordered = [(name, entry) for name, entry in ordered if family_filter in name.lower()]

    if as_json:
        payload = {
            "window_days": days,
            "families": [
                {
                    "family": name,
                    "units": entry["units"],
                    "revenue": round(entry["revenue"], 2),
                    "titles": len(entry["titles"]),
                    "window_units": entry["window_units"],
                    "docs": [
                        {
                            "slug": doc["slug"],
                            "header": doc["header"],
                            "completed_date": doc["completed_date"],
                        }
                        for doc in sorted(entry["docs"], key=lambda doc: (doc["completed_date"], doc["slug"]), reverse=True)
                    ],
                    "templates": sorted(entry["templates"]),
                }
                for name, entry in ordered
            ],
        }
        print(json.dumps(payload, indent=2))
        return 0

    print(
        f"coverage-report: {len(rows)} sales  {sum(entry['units'] for entry in families.values())} units"
        f"  {len(documents)} documents  {len(templates)} templates"
        f"  window: last {days} days"
    )
    print()

    headers = ("FAMILY", "UNITS", "REV", "TITLES", "DOCS", "TMPL", "WINDOW")
    family_width = max([len(headers[0])] + [len(name) for name, _ in ordered])
    print(
        f"{headers[0]:<{family_width}} {headers[1]:>5} {headers[2]:>9}"
        f" {headers[3]:>6} {headers[4]:>4} {headers[5]:>4} {headers[6]:>6}"
    )
    print("-" * (family_width + 5 + 9 + 6 + 4 + 4 + 6 + 6))

    slug_width = max([len(doc["slug"]) for doc in documents] + [4])

    for name, entry in ordered:
        print(
            f"{name:<{family_width}} {entry['units']:>5} {entry['revenue']:>9.2f}"
            f" {len(entry['titles']):>6} {len(entry['docs']):>4} {len(entry['templates']):>4}"
            f" {entry['window_units']:>6}"
        )
        docs = sorted(entry["docs"], key=lambda doc: (doc["completed_date"], doc["slug"]), reverse=True)
        shown = docs if docs_cap <= 0 else docs[:docs_cap]
        for doc in shown:
            print(f"   {doc['completed_date']:<10}  {doc['slug']:<{slug_width}}  {doc['header']}")
        if len(shown) < len(docs):
            print(f"   ... {len(docs) - len(shown)} more (--docs {len(docs)})")

    return 0


sys.exit(main())
PY
