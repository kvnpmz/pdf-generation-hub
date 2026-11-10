- Commit changes after completing each self-contained task or file implementation.
- Do not modify files or configurations outside the requested task scope.
- Before proposing a new listing: `scripts/opportunity-report.sh` (sales by family; `--days`, `--top`, `--titles`, `--min-rev`, `--json`), then `scripts/coverage-report.sh` (docs + templates by family; `--docs`, `--family`, `--search`, `--json`). Family keywords live in `scripts/families.py` for both.
- For layout work, verify with `scripts/render-check.sh`: one render plus a page-fit report (~3 s) and exit code 1 when it does not fit; add `--images` when the final 300dpi PNG matters (`-h` lists all flags).
- For whitespace and text-size diagnostics on any rendered page, run `scripts/page-report.sh` (`--doc`, `--pdf`, `--png`, `--json`, `--min-gap`, `--min-font`, `--strict`).
- Always use rg for reading or searching files instead of grep.

Use the local deterministic index before grep or reading whole files:
- Session start: `lua scripts/repo-intel.lua overview` (architecture summary, <500 tokens).
- Symbol lookup: `lua scripts/repo-intel.lua symbol <Name>` (signature, docs, location, line range).
- Dependency lookup: `lua scripts/repo-intel.lua deps <Name-or-Module>` (incoming/outgoing).
- Budgeted map: `lua scripts/repo-intel.lua map --budget 1024`.
- After edits: `lua scripts/repo-intel.lua update` (hash-incremental; only changed files are re-parsed).

Cache lives in `.repo-intel/` and is gitignored. `--no-refresh` uses the cache as-is.
If grammars or the index are missing, run `lua scripts/repo-intel.lua setup`.
