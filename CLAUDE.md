# CLAUDE.md — TeumesseCastle

Guidance for Claude Code sessions working in this repository. Read it fully before touching code.

## What this project is

TeumesseCastle is the free, community fork of **PingCastle 3.0.0.0** (Active Directory security audit tool),
published by Teumesse (Johnny Cervantes, alias adios25) at https://github.com/Teumesse0/TeumesseCastle.
Version 1.0.0 = functional parity with PingCastle 3.0.0.0, fully rebranded, fully commented, with every
license/edition mechanism and every vendor network call removed. It must remain open source forever.

- Upstream baseline: https://github.com/netwrix/pingcastle tag `3.0.0.0`, commit
  `d8f21e920278b82f243bdfe84d168d8040a0b96c`, license NPOSL-3.0 (`LICENSE.md`, unchanged).
- Never import code from any later upstream commit without an explicit decision recorded in
  `docs/DECISIONS.md` and `CHANGELOG.md`.
- Working folder on Johnny's machine: `C:\Users\johnny\OneDrive\Bureau\Teumesse\Projets\TeumesseCastle`.
- The original mission brief is `docs/PROMPT-claude-code-v1.md`; current status is `docs/PROGRESS.md`;
  decisions are `docs/DECISIONS.md`; the source audit is `docs/audit/00-inventaire-source.md`.

## Absolute rules

1. **Zero functional regression.** Same reports (healthcheck HTML/XML, scanners, graph, exports) as
   PingCastle 3.0.0.0 on the same AD. Never invent, remove or "simplify" a rule, a score, a threshold or a
   scanner. Reference binaries for comparison live in `reference/pingcastle-3.0.0.0-binaries/` (git-ignored).
2. **Read before you write.** Never comment or modify a method whose body you have not read. If a block is
   unclear, write `// TODO(review): ...` instead of guessing.
3. **The build is green at every commit.** Run the build (see below) before committing. Never stack a second
   change on a broken build.
4. **No telemetry, no undocumented outbound network call.** Never reintroduce a license check, an edition
   check, an expiry date, an auto-updater, or any call to pingcastle.com / netwrix.com. The only allowed
   outbound HTTP call is the opt-in `--check-update` against the GitHub Releases API (never automatic).
   Every network behaviour is documented in `docs/NETWORK.md`.
5. **Respect the law.** Keep every original copyright header and `LICENSE.md` untouched. Do not add a second
   license. Do not use the PingCastle name as the product name (trademark); keep it only in attribution.
6. **Incremental and traceable.** Conventional Commits, atomic commits, one phase = one branch merged into
   `main`. Destructive operations (mass deletion, force push, history rewrite, branch deletion) need
   Johnny's explicit confirmation.
7. **Never commit real Active Directory data** (reports, logs, fixtures). Synthesise or anonymise.

## Build, run, test

Prerequisites: Windows, .NET SDK 8.x. The projects target .NET Framework 4.5.2 (main) and 4.8 (updater,
until removed); reference assemblies come from NuGet via `Directory.Build.props`, no targeting pack needed.

```powershell
dotnet build TeumesseCastle.sln -c Release
dotnet test  TeumesseCastle.Tests -c Release        # from Phase 6
.\bin\Release\TeumesseCastle.exe --help
.\bin\Release\TeumesseCastle.exe --version
```

`dotnet build` restores the NuGet packages automatically. The pre-build step runs
`template\ProcessTemplate.ps1` (gzips the HTML/CSS/JS templates); the `.gz` files are also tracked in git.

## Conventions

- Code comments and XML documentation: **English**. Documentation for users: English (`README.md`) and
  French (`README.fr.md`). Conversation with Johnny: French.
- XML doc comments (`<summary>`, `<param>`, `<returns>`, `<remarks>`, `<exception>`) on every class,
  interface, enum, public/internal method, non-trivial property and magic constant. `CS1591` is an error in
  Release once Phase 5 is complete.
- File header: original upstream copyright block, then
  `// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.`, then 2-4 lines on the file's
  role and links to the specification it implements ([MS-ADTS], [MS-DRSR], [MS-SAMR], ...).
- Commits: Conventional Commits (`feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `ci`, `build`;
  `feat!` / `BREAKING CHANGE:` footer for incompatible changes). Scope in parentheses when useful,
  e.g. `docs(code): document ADWS with XML comments`.
- Versioning: SemVer, single source of truth `<Version>` in `Directory.Build.props`, shown by `--version`.
- Output file names keep the historical `ad_hc_*` prefixes (compatibility with third-party tooling).
- Non-blocking decisions: take the most conservative option (the one that preserves the original behaviour)
  and record it as a short ADR in `docs/DECISIONS.md`.

## Repository map

| Path | Role |
|---|---|
| `Program.cs`, `Tasks.cs`, `RuntimeSettings.cs`, `ConsoleMenu.cs` | CLI entry point, argument parsing, interactive menu, task orchestration |
| `ADWS/` | AD connectors (ADWS/SOAP, LDAP, SMB file access, Samba) |
| `Healthcheck/` | Data collection (`HealthcheckAnalyzer.cs`) and the 170 rules in `Healthcheck/Rules/` |
| `Rules/` | Rule engine, attributes, maturity model, custom rule settings |
| `Report/` | HTML report generators (single, consolidation, maps, rules list, cloud) |
| `Data/` | Serialisable report model (`HealthcheckData`) |
| `Graph/` | Compromise graph (`--carto`) |
| `Scanners/`, `shares/` | Per-computer network scanners |
| `Exports/` | CSV exports |
| `RPC/`, `NativeMethods.cs`, `misc/` | P/Invoke, RPC, DNS, registry, utilities |
| `Cloud/` | Azure AD / Entra ID audit (`--azuread`) |
| `template/` | Embedded HTML/CSS/JS resources (gzipped) |
| `docs/` | Audit, decisions, progress, architecture, rules list, network, releasing |
| `reference/` | PingCastle 3.0.0.0 binaries for non-regression comparison (git-ignored except changelog/license) |

## Workflow for a session

1. Read `docs/PROGRESS.md` to know the current phase, then the relevant section of the mission brief.
2. Present the phase plan to Johnny as a task list; wait for validation when the brief requires it.
3. Work on a phase branch, commit atomically, keep the build green, update `CHANGELOG.md`.
4. At the end of the phase: update `docs/PROGRESS.md`, summarise (what was done, commands, files, build output).
