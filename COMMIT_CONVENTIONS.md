# HannibalUI Commit Message Conventions

Credit: this document adapts the [Conventional Commits v1.0.0 specification](https://www.conventionalcommits.org/en/v1.0.0/)
(© the Conventional Commits contributors) to HannibalUI's existing commit history and folder
layout. Where this document is silent, the upstream spec is authoritative.

## Why

Conventional Commits give every commit a predictable structure, which makes `git log` scannable,
lets changelogs be generated automatically, and — because the spec maps types to
[SemVer](https://semver.org/) (`fix` → PATCH, `feat` → MINOR, `BREAKING CHANGE` → MAJOR) — gives
a mechanical way to decide the next version of the `com.voxelpixel.hannibal-ui` package in
`package.json`.

This repo's history already follows the shape of this spec (`feat(pool): Fix resize logic`,
`fix(animation): Fix animation issue`, etc.) — this document makes that existing practice
explicit and fills in the parts (breaking changes, footers, the full type list) that haven't come
up yet.

## Format

```
<type>(<scope>)<!>: <description>

<body>

<footer(s)>
```

- `type` — required.
- `(scope)` — required in this repo (see below); the spec itself treats it as optional.
- `!` — only when the commit is a breaking change.
- `description` — required, one line.
- `body` / `footer(s)` — optional, separated from the description and from each other by one
  blank line.

## Types

| Type | Use for | Status in this repo |
|---|---|---|
| `feat` | A new capability, class, or public API surface (e.g. adding `VP_HorizontalRectTransformContainer`) | Already used, most common type |
| `fix` | A bug fix (e.g. the `ObjectPool` resize logic fix) | Already used |
| `chore` | Maintenance with no runtime effect: renames, config, `.gitignore`, dependency bumps | Already used |
| `docs` | Documentation only (`README.md`, `CODING_STYLE.md`, XML doc comments) | Not yet used — adopt going forward instead of the bare `Update README.md` commits in the early history |
| `refactor` | Restructuring code with no behavior change (e.g. splitting a class for single-responsibility) | Not yet used |
| `test` | Adding or correcting tests, with no production code change | Not yet used as a *type* (existing `feat(test): Add object pool tests` used `test` as a scope instead — prefer `test(pool): Add object pool tests` going forward) |
| `perf` | A change whose primary purpose is performance | Not yet used |
| `style` | Formatting-only changes (whitespace, brace style) with no logic change — see [CODING_STYLE.md](CODING_STYLE.md) | Not yet used |
| `build` | Changes to the Unity project/package build setup (`package.json`, `.asmdef` files) | Not yet used — `feat(package): Remove unnecessary assembly definitions` would be `build(package)` under this convention |
| `ci` | Changes to CI/automation config, once this repo has any | Not applicable yet |

Only `feat` and `fix` carry SemVer meaning by default. Everything else is informational.

## Scope

Every commit should carry a scope: a lowercase, singular noun naming the part of the codebase
it touches. Base it on the `Runtime/` folder structure:

| Scope | Corresponds to |
|---|---|
| `base` | `Runtime/Base/` (`VP_Director`, `VP_Canvas`, `VP_UIObject`, events, ...) |
| `popup` | `Runtime/Base/PopupSystem/` |
| `animation` | `Runtime/Animation/` |
| `pool` | `Runtime/Helpers/ObjectPool/` |
| `observer` | `Runtime/Helpers/Observer/` |
| `elements` | `Runtime/UIElements/` |
| `utils` | `Runtime/Utilities/` |
| `test` | `Tests/Runtime/` |
| `package` | `package.json`, `.asmdef` files, package-level metadata |
| `sample` | Sample/demo scripts (`RuntimeSampleSciprt.cs` and similar) |

The existing history is inconsistent about pluralization (`helper` vs. `helpers`) and has scopes
that don't map to a folder (`config`, `core`, `git`, `setup`, `name`, `reference`, `typo`). Don't
add new ones of those — if a commit doesn't fit a folder-based scope, prefer `chore` with the
scope that best names the actual concern (e.g. `chore(package): bump DOTween dependency`), and
if it truly spans the whole repo, omit the scope rather than inventing one.

## Description

- Imperative mood, matching this repo's existing history: `Add`, `Fix`, `Remove`, not `Added` /
  `Fixes`.
- Capitalize the first letter (this repo's convention — the upstream spec doesn't mandate a
  case, but be consistent). Example: `feat(pool): Add resize guard`.
- No trailing period.
- Keep it to a single line, short enough to read in a `git log --oneline`.

## Body

Use the body to explain *why*, the same rule as code comments in
[CODING_STYLE.md](CODING_STYLE.md#comments) — not a restatement of the diff. Skip it for
small, self-explanatory commits (most `fix`/`chore` commits in this repo's history are a single
line and that's fine).

## Footers

One blank line after the body (or after the description, if there's no body). Each footer is
`Token: value` or `Token #value`; multi-word tokens use a hyphen (`Refs`, `Closes`,
`Reviewed-by`).

```
fix(pool): Guard against double release

Release() decremented _currentIndex without checking whether the slot
was already free, so calling it twice on the same object corrupted the
backing array.

Closes: #42
```

### Breaking changes

Mark a breaking API change with `!` before the colon, and/or a `BREAKING CHANGE:` footer with a
description of the break and the migration path. Use `!` alone for a short break; add the
footer when callers need more explanation.

```
feat(base)!: Change EnableCanvas to take a CanvasType instead of an index

BREAKING CHANGE: VP_Director.EnableCanvas(int) has been removed. Callers
must pass a CanvasType value instead.
```

Since HannibalUI is consumed as a UPM package by other projects, a `!` or `BREAKING CHANGE:`
footer is the signal that `package.json`'s `version` needs a MAJOR bump.

## Exceptions

- Merge commits (`Merge branch ...`, `Merge pull request ...`) are exempt — GitHub generates
  these automatically and they aren't hand-written.
- Everything else, including doc-only and config-only commits, should follow this format —
  including the bare `Update README.md` style used a few times in the early history, which
  should become `docs: Update README.md` going forward.

## Reference

- [Conventional Commits v1.0.0](https://www.conventionalcommits.org/en/v1.0.0/) — the base
  specification this document adapts.
- The extended type list (`docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`) comes from
  the [Angular commit convention](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#-commit-message-format),
  which Conventional Commits explicitly credits as its origin for those types.
- [Semantic Versioning 2.0.0](https://semver.org/) — the versioning scheme Conventional Commits
  types map to.
