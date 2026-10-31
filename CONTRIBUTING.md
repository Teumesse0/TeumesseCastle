# Contributing to TeumesseCastle

*Version française : [CONTRIBUTING.fr.md](CONTRIBUTING.fr.md)*

Thank you for helping keep a free Active Directory audit tool alive. This document explains how we work.

## Ground rules

1. **TeumesseCastle stays open source, forever.** Any contribution is accepted under the project license
   (NPOSL-3.0, see `LICENSE.md`). We never add license checks, paid editions, telemetry or undocumented
   outbound network calls. Pull requests that introduce any of these are closed.
2. **No real Active Directory data.** Never commit, paste in an issue or attach a report, log, XML or
   screenshot containing data from a real domain. Anonymise or synthesise it.
3. **The security rule set is a reference.** Changing a rule's score, threshold, maturity level or wording
   is a functional change that needs a dedicated issue and a documented rationale (link to ANSSI, Microsoft,
   MITRE or CIS guidance).
4. Be kind. See `CODE_OF_CONDUCT.md`.

## Workflow

1. Open an issue first for anything bigger than a typo (bug, feature, new rule) using the templates.
2. Fork, then create a branch from `main`: `feat/<topic>`, `fix/<topic>`, `docs/<topic>`, `rule/<RULE-ID>`.
3. Commit with **Conventional Commits** (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`, `ci:`;
   `feat!:` or a `BREAKING CHANGE:` footer for incompatible changes). One logical change per commit.
4. Make sure the build is green and the tests pass (see below) **before every commit**.
5. Open a pull request against `main` and fill in the checklist. A maintainer reviews it; CI must be green.

## Build and test

Prerequisites: Windows, .NET SDK 8.x (the project targets .NET Framework 4.5.2; reference assemblies are
restored from NuGet, no targeting pack installation is required).

```powershell
dotnet build TeumesseCastle.sln -c Release
dotnet test  TeumesseCastle.Tests -c Release
.\bin\Release\TeumesseCastle.exe --help
```

Visual Studio 2022 (any edition) also opens the solution directly.

## Code conventions

- C# code comments and XML documentation are written **in English**.
- Every public or internal type and member has an XML documentation comment (`<summary>`, `<param>`,
  `<returns>`, `<remarks>` when useful). `CS1591` is an error in the Release configuration, so the build
  enforces this.
- Non-obvious algorithms (LDAP parsing, scoring, graph traversal, native structure decoding, P/Invoke) get
  inline `//` comments explaining *why*, not just *what*.
- Do not comment a method you have not read. If something is unclear, write `// TODO(review): ...` rather
  than guessing.
- Keep the original upstream copyright header in every file. Files you modify carry the line
  `// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.` under it.
- Preserve behaviour. A refactoring must not change the generated reports (HTML, XML, JSON) for the same
  input. When in doubt, compare against the reference output of PingCastle 3.0.0.0.

## Adding a security rule

1. Open a "New security rule" issue describing the detection, the risk and the remediation, with sources.
2. Implement it in `Healthcheck/Rules/` following an existing rule (attributes `[RuleModel]`,
   `[RuleComputation]`, `[RuleMaturityLevel]`, references `[RuleANSSI]`, `[RuleSTIG]`, `[RuleMitreAttack]`).
3. Add the texts to `Healthcheck/Rules/RuleDescription.resx` and regenerate `docs/RULES.md` with
   `tools/Generate-RulesDoc.ps1`.
4. Add a unit test with a synthetic fixture.
5. Update `CHANGELOG.md` under `[Unreleased]`.

## Documentation

User documentation exists in English (`README.md`) and French (`README.fr.md`). When you change one,
change the other, or say in the PR that a translation is needed.
