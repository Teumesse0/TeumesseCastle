# Changelog

All notable changes to TeumesseCastle are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Version 1.0.0 is defined as functional parity with PingCastle 3.0.0.0 (upstream commit
`d8f21e920278b82f243bdfe84d168d8040a0b96c`), rebranded and freed from every license/edition check.

## [Unreleased]

### Added
- Repository scaffolding: `NOTICE.md`, `CONTRIBUTING.md` / `CONTRIBUTING.fr.md`, `CODE_OF_CONDUCT.md`,
  `SECURITY.md`, `CLAUDE.md`, `docs/` (audit, decisions, progress).
- `Directory.Build.props` pulling the .NET Framework reference assemblies from NuGet so that the solution
  builds with the .NET SDK (`dotnet build`) without installing targeting packs.

### Changed
- `PingCastle.csproj` converted from the legacy project format to the SDK format. The target framework is
  unchanged (.NET Framework 4.5.2); the produced executable exposes the same embedded resources and the
  same types as PingCastle 3.0.0.0.
- `Newtonsoft.Json` is now a NuGet `PackageReference` (13.0.1) instead of a loose DLL next to the project.
- Every `.cs` file carries the original upstream copyright header plus the TeumesseCastle modification notice.
- Unused non-string sample entries (`Color1`, `Bitmap1`, `Icon1`) removed from the `.resx` files; the Azure AD
  sign-in dialog now takes its icon from the executable instead of an embedded resource.

### Removed
- Post-build code-signing step bound to the upstream author's workstation (`signtool`, certificate
  "Ping Castle SAS").

[Unreleased]: https://github.com/Teumesse0/TeumesseCastle/compare/main...HEAD
