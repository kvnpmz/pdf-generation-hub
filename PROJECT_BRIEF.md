# PROJECT BRIEF — read this before exploring

One-page Letter printables. `teal/runtime.tl` picks the document; `documents/<slug>/config.tl` is content only
(data, labels, `output_name`); `templates/<template>/` is layout (`render.cs` or `render.tl`) + `style.css`.
Mirror an existing doc/template instead of inventing structure.

## Pipeline
- C# `render.cs` is compiled at build by `Launcher.cs` (Roslyn); it is not in the csproj and holds one `IRenderer`
  class. Config arrives as `LuaTable`; `Dom.cs` has only basic tags — use raw HTML strings for `rowspan`/`colspan`.
- CSS order: `base.css` → `templates/<template>/style.css` → `config.style` / `config.overrides` (CSS files that must
  live in the template folder and share the key name). No `style` key = base.css only.
- Teal templates use `render.tl` + `dom.tl` and run `tl check` on load. A template uses one language only.
- Never run `tpr` (blocking watcher). `scripts/render-check.sh --doc <slug>` renders once and measures.

## Page rules (Letter, 612×792pt)
- `html` padding is 36pt top/bottom and 50.4pt sides → 511×720pt of content. Ink must fill 85–91% and end 35–64pt
  above the page bottom; side margins ≥40pt; column-bottom spread ≤10pt; zero text collisions.
- Fonts: Inter is default; "Source Serif 4" is installed for serif variants. `base.css` body is 6pt, so templates set
  real pt sizes. Use `scripts/scale-type.sh --doc <slug> --fit` (`--prop height` for rows) instead of hand-tuning.

## Variants & files
- Color: `scripts/make-variant.sh <base> pink --set var=#hex ...` → `<base>_pink` doc + `<pink>_style.css`. It copies the
  listing; override hard-coded surface colors too, not just `:root`.
- Font/layout tweak: doc config `extends = "<base>"` + `overrides = "<name>"`; new structure: new template folder + doc.
- Variants share `output_name`, so the PDF is `output/<doc>/<output_name>.pdf`, not `<doc>.pdf` (use `--doc` everywhere).
- Commit `documents/` + `templates/` (+ runtime pointer) only. Never commit `projects.db` (renderer mutates it) or `output/`.

## Copy & checks
- `listing_prompt.txt` rules → `scripts/listing-check.sh documents/<slug>`; `scripts/ship-check.sh <slug> --images` is
  the final gate (doc-audit, fit, type, copy). Never hand-count title words, tags, or sentences.
- Do not read `preview.html`, `output/**/*.html`, or 300dpi PNGs; read render-check numbers and small raster crops.
