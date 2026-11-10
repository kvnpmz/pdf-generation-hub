- Git & Scope: Commit after each self-contained task. Do not modify files outside the requested scope.
- Search: Always use `rg` instead of `grep`.

### Scripts & Workflows
- New Listing Prep: 
  1. `scripts/opportunity-report.sh` (sales by family; flags: `--days`, `--top`, `--titles [N]`, `--min-rev`, `--json`).
  2. `scripts/coverage-report.sh` (docs + templates by family; flags: `--docs`, `--family`, `--search`, `--json`). 
  (Family keywords live in `scripts/families.py`).
- Type & Spacing Fit: `scripts/scale-type.sh --doc <slug> --fit` (binary-searches the largest scale that passes render-check; `--prop height` scales row/spacing heights instead of font sizes; `--scale 1.1` one-shot, `--dry-run` preview; auto-restores stylesheet on overflow). Use it instead of hand-calculating CSS heights.
- Listing Copy: Draft `documents/<slug>/listing.txt`, then run `scripts/listing-check.sh documents/<slug>` and fix what it reports; never count title words, tags, or sentences by hand. `ship-check.sh` runs this check too.
- Ship Check: `scripts/ship-check.sh <slug>` (runs doc-audit, render-check, page-report, and listing-check in one block; `--images` for final export, `--json` for machine parse).

### Repo Intel Index (`lua scripts/repo-intel.lua <cmd>`)
Use the local index before `rg` or reading whole files:
- `overview`: Architecture summary (<500 tokens).
- `symbol <Name>`: Signature, docs, location, line range.
- `deps <Name-or-Module>`: Incoming/outgoing dependencies.
- `map --budget 1024`: Context-budgeted map.
- `update`: Hash-incremental update (`.repo-intel/`, gitignored, `--no-refresh` to use as-is).
- `setup`: Run if grammars/index are missing.
