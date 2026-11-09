- Commit changes after completing each self-contained task or file implementation.
- Do not modify files or configurations outside the requested task scope.
- For layout work, verify with `scripts/render-check.sh`: one render plus a page-fit report (~3 s) and exit code 1 when it does not fit. `tpr` is a blocking watcher - do not use it for one-shot checks.
- Always try `search_code` before reading files or grepping; only fall back to `rg` for exact strings and read files only after a search points to them.

Use the local deterministic index before grep or reading whole files:
- Session start: `lua scripts/repo-intel.lua overview` (architecture summary, <500 tokens).
- Symbol lookup: `lua scripts/repo-intel.lua symbol <Name>` (signature, docs, location, line range).
- Dependency lookup: `lua scripts/repo-intel.lua deps <Name-or-Module>` (incoming/outgoing).
- Budgeted map: `lua scripts/repo-intel.lua map --budget 1024`.
- After edits: `lua scripts/repo-intel.lua update` (hash-incremental; only changed files are re-parsed).

Cache lives in `.repo-intel/` and is gitignored. `--no-refresh` uses the cache as-is.
If grammars or the index are missing, run `lua scripts/repo-intel.lua setup`.
