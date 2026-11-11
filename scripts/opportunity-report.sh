#!/usr/bin/env bash
#
# opportunity-report.sh - sales + listing traffic + catalogue coverage report.
#
# Why: picking the next listing used to mean several ad-hoc queries against
# sales.csv and views.csv plus a manual cross-check of documents/. This prints
# the same numbers deterministically, capped at a fixed number of lines.
# views.csv adds the other half of the story: listing traffic per family, and
# the listings that get views but never convert into an order.
#
# Usage:
#   scripts/opportunity-report.sh                  # 30-day window, top 12 families
#   scripts/opportunity-report.sh --days 14
#   scripts/opportunity-report.sh --top 8 --titles 5
#   scripts/opportunity-report.sh --titles         # default 10 titles
#   scripts/opportunity-report.sh --min-rev 50     # hide low-revenue families
#   scripts/opportunity-report.sh --demand 5       # traffic leaders with no orders
#   scripts/opportunity-report.sh --no-views       # sales.csv only
#   scripts/opportunity-report.sh --json           # machine-readable
#
# Sources:
#   sales.csv  transaction log (dated) - units, revenue, window
#   views.csv  lifetime catalogue - listing traffic, favourites, orders, status
#
# Columns:
#   FAMILY     keyword family (rules live below, reviewed once)
#   UNITS/REV  lifetime units and revenue for that family
#   TITLES     distinct sales titles in the family
#   DOCS       documents/<slug> folders for that family
#   REV/DOC    revenue per existing document
#   WINDOW     units sold in the --days window
#   VIEWS      lifetime listing views (views.csv)
#   V/DOC      lifetime views per existing document
#   ORD/1K     orders per 1,000 views (lifetime conversion)
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

APP = sys.argv[1]
sys.dont_write_bytecode = True
sys.path.insert(0, os.path.join(APP, "scripts"))
from families import EXCLUDED_DOCS, family_of


def usage() -> None:
    print(
        "usage: opportunity-report.sh [--days N] [--top N] [--titles N] [--min-rev N]"
        " [--demand N] [--no-views] [--json]"
    )
    sys.exit(0)


def main() -> int:
    app = sys.argv[1]
    args = sys.argv[2:]

    days = 30
    top = 12
    title_limit = 10
    min_rev = 0.0
    demand_limit = 8
    with_views = True
    as_json = False

    def number(flag: str, raw: str, cast):
        try:
            return cast(raw)
        except (TypeError, ValueError):
            shown = raw if raw else "nothing"
            print(
                f"opportunity-report: {flag} needs a number, got {shown}",
                file=sys.stderr,
            )
            sys.exit(2)

    i = 0
    while i < len(args):
        arg = args[i]
        if arg in ("-h", "--help"):
            usage()
        elif arg == "--days":
            days = number("--days", args[i + 1] if i + 1 < len(args) else "", int)
            i += 2
        elif arg == "--top":
            top = number("--top", args[i + 1] if i + 1 < len(args) else "", int)
            i += 2
        elif arg == "--titles":
            # Bare --titles keeps the default count; --titles N sets it.
            if i + 1 < len(args) and re.fullmatch(r"\d+", args[i + 1]):
                title_limit = int(args[i + 1])
                i += 2
            else:
                i += 1
        elif arg == "--min-rev":
            min_rev = number("--min-rev", args[i + 1] if i + 1 < len(args) else "", float)
            i += 2
        elif arg == "--demand":
            demand_limit = number("--demand", args[i + 1] if i + 1 < len(args) else "", int)
            i += 2
        elif arg == "--no-views":
            with_views = False; i += 1
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

    def family_entry(name: str):
        return families.setdefault(
            name,
            {
                "units": 0,
                "revenue": 0.0,
                "titles": set(),
                "window_units": 0,
                "listings": 0,
                "views": 0,
                "favorites": 0,
                "orders": 0,
            },
        )

    for stamp, name, quantity, price in rows:
        family = family_of(name)
        entry = family_entry(family)
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

    # views.csv is the lifetime catalogue: one row per listing, whether or not
    # it ever sold. Same title may appear twice (renewed listing), so collapse
    # rows by title first and only then roll up per family.
    def plural(n: int, word: str) -> str:
        return f"{n:,} {word}" if n == 1 else f"{n:,} {word}s"

    def count(value: str) -> int:
        digits = re.sub(r"[^0-9]", "", value or "")
        return int(digits) if digits else 0

    views_path = os.path.join(app, "views.csv")
    listings = {}
    if with_views and os.path.exists(views_path):
        with open(views_path, newline="", encoding="utf-8") as handle:
            for row in csv.DictReader(handle):
                title = (row.get("Title") or "").strip()
                if not title:
                    continue
                family = family_of(title)
                entry = family_entry(family)
                views = count(row.get("Views", ""))
                favorites = count(row.get("Favorites", ""))
                orders = count(row.get("Orders", ""))
                entry["views"] += views
                entry["favorites"] += favorites
                entry["orders"] += orders

                listing = listings.setdefault(
                    title,
                    {
                        "title": title,
                        "family": family,
                        "views": 0,
                        "favorites": 0,
                        "orders": 0,
                        "active": False,
                    },
                )
                listing["views"] += views
                listing["favorites"] += favorites
                listing["orders"] += orders
                active = row.get("Status", "").strip().lower() == "active"
                listing["active"] = listing["active"] or active

        for listing in listings.values():
            family_entry(listing["family"])["listings"] += 1
    elif with_views:
        print(f"opportunity-report: {views_path} not found, sales-only report", file=sys.stderr)

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
        entry["views_per_doc"] = entry["views"] / entry["docs"] if entry["docs"] else 0.0
        entry["orders_per_1k"] = entry["orders"] * 1000 / entry["views"] if entry["views"] else 0.0

    ordered = sorted(families.items(), key=lambda item: (-item[1]["revenue"], item[0]))
    ordered = [(name, entry) for name, entry in ordered if entry["revenue"] >= min_rev]

    top_titles = sorted(titles.items(), key=lambda item: (-item[1]["revenue"], item[0]))
    unsold = sorted(
        (listing for listing in listings.values() if listing["orders"] == 0),
        key=lambda listing: (-listing["views"], listing["title"]),
    )
    unsold_views = sum(listing["views"] for listing in unsold)

    if as_json:
        payload = {"window_days": days}
        if listings:
            payload["views_summary"] = {
                "listings": len(listings),
                "active": sum(1 for listing in listings.values() if listing["active"]),
                "views": sum(listing["views"] for listing in listings.values()),
                "favorites": sum(listing["favorites"] for listing in listings.values()),
                "orders": sum(listing["orders"] for listing in listings.values()),
                "unsold_listings": len(unsold),
                "unsold_views": unsold_views,
            }
        payload["families"] = [
            {
                "family": name,
                "units": entry["units"],
                "revenue": round(entry["revenue"], 2),
                "titles": len(entry["titles"]),
                "docs": entry["docs"],
                "revenue_per_doc": round(entry["rev_per_doc"], 2),
                "window_units": entry["window_units"],
                "listings": entry["listings"],
                "views": entry["views"],
                "favorites": entry["favorites"],
                "orders": entry["orders"],
                "views_per_doc": round(entry["views_per_doc"], 1),
                "orders_per_1k_views": round(entry["orders_per_1k"], 2),
            }
            for name, entry in ordered[:top]
        ]
        payload["titles"] = [
            {
                "title": name,
                "family": entry["family"],
                "units": entry["units"],
                "revenue": round(entry["revenue"], 2),
                "window_units": entry["window_units"],
            }
            for name, entry in top_titles[:title_limit]
        ]
        payload["unsold_traffic"] = [
            {
                "title": listing["title"],
                "family": listing["family"],
                "views": listing["views"],
                "favorites": listing["favorites"],
                "active": listing["active"],
            }
            for listing in unsold[:demand_limit]
        ]
        print(json.dumps(payload, indent=2))
        return 0

    total_revenue = sum(entry["revenue"] for entry in families.values())
    total_units = sum(entry["units"] for entry in families.values())
    window_units = sum(entry["window_units"] for entry in families.values())

    print(f"sales.csv  {len(rows)} sales  {total_units} units  ${total_revenue:,.2f}  "
          f"window: last {days} days  {window_units} units")
    if listings:
        print(f"views.csv  {plural(len(listings), 'listing')}  "
              f"{sum(1 for listing in listings.values() if listing['active'])} active  "
              f"{sum(listing['views'] for listing in listings.values()):,} views  "
              f"{sum(listing['orders'] for listing in listings.values())} orders  "
              f"{sum(listing['favorites'] for listing in listings.values())} favs  (lifetime)")
    print()

    header = (
        f"{'FAMILY':24s} {'UNITS':>6s} {'REV':>9s} {'TITLES':>7s} {'DOCS':>5s}"
        f" {'REV/DOC':>8s} {'WINDOW':>7s}"
    )
    if listings:
        header += f" {'VIEWS':>9s} {'V/DOC':>7s} {'ORD/1K':>6s}"
    print(header)
    print("-" * len(header))
    for name, entry in ordered[:top]:
        line = (
            f"{name:24s} {entry['units']:6d} {entry['revenue']:9.2f} {len(entry['titles']):7d} "
            f"{entry['docs']:5d} {entry['rev_per_doc']:8.1f} {entry['window_units']:7d}"
        )
        if listings:
            per_doc = f"{entry['views_per_doc']:7,.0f}" if entry["docs"] else f"{'-':>7s}"
            line += f" {entry['views']:9,d} {per_doc} {entry['orders_per_1k']:6.1f}"
        print(line)

    print()
    print(f"TOP TITLES (by revenue, top {title_limit})")
    for name, entry in top_titles[:title_limit]:
        short = name if len(name) <= 72 else name[:69] + "..."
        print(f"  {entry['revenue']:7.2f} {entry['units']:3d}u {entry['window_units']:3d}w  {short}")

    if unsold and demand_limit > 0:
        print()
        print(f"TRAFFIC WITHOUT ORDERS ({plural(len(unsold), 'listing')}, {unsold_views:,} views, 0 orders)")
        for listing in unsold[:demand_limit]:
            title = listing["title"]
            short = title if len(title) <= 84 else title[:81] + "..."
            flag = "" if listing["active"] else "  (inactive)"
            print(f"  {listing['views']:7,d}v {listing['favorites']:3d}f  {short}{flag}")

    unmatched = [name for name, entry in titles.items() if entry["family"] == "other"]
    if unmatched:
        print()
        print(f"UNMATCHED ({len(unmatched)}) - add keywords to scripts/families.py:")
        for name in sorted(unmatched)[:8]:
            print(f"  {name[:96]}")

    return 0


sys.exit(main())
PY
