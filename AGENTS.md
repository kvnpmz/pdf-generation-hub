# Project Manual

## Business Strategy & Product Expansion
- Assume the role of technical co-founder and business partner. Ship solid code and build a demand-led portfolio.
- Depth over diversification. High competition proves demand. Expand winning categories into long-tail searches and micro-niches. Do not abandon them over generic saturation fears.
- Digital fulfillment costs near-zero. Expand whenever incremental portfolio revenue exceeds creation/opportunity cost. Cannibalization and diminishing returns are metrics, not stop signs.
- Optimize portfolio profit, not averages. Measure total category revenue and return on design time. Remix templates to lower creation cost. Pivot only when incremental returns fall below the next-best opportunity.

## Architecture
- `teal/runtime.tl` selects the document.
- `documents/<slug>/config.tl` holds content and data only: labels, rows, columns, `output_name`, `completed_date`.
- `templates/<template>/` holds layout and `style.css`. Renderers stay layout-only.
- `Launcher.cs` compiles `render.cs` at build; the csproj does not.
- Use one `IRenderer`; config is a `LuaTable`.
- Build every element with `Dom.Tag(name, class, ...)`; never raw HTML. (Teal: `dom.tag`.)
- Teal templates use `render.tl` + `dom.tl`. Run `tl check` on load. Use one language per template. Declare named records (`local record Rule ... end`), not inline table shapes.
- CSS layers in order: `base.css` → template `style.css` → `config.style`/`config.overrides`. The last two live in the template folder and match by key name. A wrong name silently falls back to `base.css`.
- New document: `config.tl` needs `id`, `template`, `output_name`, `completed_date`. Add `listing.txt`.
- Variants share `output_name`, so files are `output/<doc>/<output_name>.pdf`.
- Commit `documents/` + `templates/` only. Rendering mutates `projects.db`; `output/` is generated. Commit after each self-contained task and keep scope to the request.

## Page & Type Rules
- Letter page: 612×792pt. `html` padding: 36pt top/bottom, 50.4pt sides → 511×720pt content.
- Targets: ink fill 85–91%, bottom margin 35–64pt, sides ≥40pt, column spread ≤10pt, zero collisions.
- Type: Inter default; "Source Serif 4" available. `base.css` body is 6pt, so templates set real pt sizes.
- Debug in this order: page count → fill/bottom margin → collisions/spread → CSS layer order. Use render-check numbers.
- If a layout feels unlike the repo, config is doing markup work or CSS is in the wrong layer. Fix the layer, not the values.

## New Listing Workflow
1. Analyze demand. Run `scripts/opportunity-report.sh` (sales + views.csv by family; `--days`, `--top`, `--titles [N]`, `--min-rev`, `--demand [N]`, `--no-views`, `--json`) and `scripts/coverage-report.sh` (docs + templates by family; `--docs`, `--family`, `--search`, `--json`; family keywords in `scripts/families.py`).
2. Present 2–3 strongest proven sellers worth varying with real content or use-case changes (alternative layout logic or structural worksheet views), plus a recommendation. Do not write files until the user picks one.
3. Implement in the session's chosen language (Teal or C#; see `.goosehints`).
4. Work under `templates/<slug>/` and `documents/<slug>/config.tl`. Mirror the closest sibling's palette, spacing, and structure. Read only the fingerprint described in `.goosehints`.
5. Use a unique 2–4 word slug folder. Point `teal/runtime.tl` at the new doc. Verify pages/fill/margins with `scripts/render-check.sh`.
6. Write `documents/<slug>/listing.txt` per §Listing Metadata.
7. Finish with `scripts/ship-check.sh --doc <slug>`. Add `--images` only for the final export.
8. Commit only the new `documents/<slug>/` and `templates/<slug>/` files.

## Listing Metadata
`listing.txt` is plain Etsy metadata, not a PDF.
- Title: exactly 30 words, pipe-separated (`|`).
- Description: exactly one sentence describing the PDF. Do not start with "This". Do not use "clean".
- Tags: exactly 13 unique tags, 2–3 words each, lowercase, comma-separated, maximizing unique word coverage.

Draft, then run `scripts/listing-check.sh documents/<slug>` and fix what it reports. Never count title words, tags, or sentences by hand. `ship-check.sh` runs this check too.

## Variants
- Color: run `scripts/make-variant.sh <base-slug> <color> --set name=#hex ...` for every palette variable. Keep the base `output_name` and `listing.txt` unchanged. Override hard-coded surface colors too, not only `:root`. Give each color a unique slug. Verify each with `scripts/ship-check.sh --doc <slug>`.
- Font/layout: use `extends` + `overrides`. Font-only variants are override stylesheets. A new structure needs a new template folder and document.

## Tooling
- `scripts/scale-type.sh --doc <slug> --fit` binary-searches the largest passing scale. `--prop height` scales row/spacing heights. `--scale 1.1` runs one-shot. `--dry-run` previews. The script restores the stylesheet on overflow. Use it instead of hand-calculating CSS heights.
- `scripts/ship-check.sh --doc <slug>` runs doc-audit, render-check, page-report, and listing-check in one block. `--json` for machine parse.
- Repo intel: `lua scripts/repo-intel.lua <cmd>`. Commands: `overview`, `symbol <Name>`, `deps <Name-or-Module>`, `map --budget 1024`, `update`, `setup`.

