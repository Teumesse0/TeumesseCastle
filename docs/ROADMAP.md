# TeumesseCastle roadmap

*Version française : voir la section « Résumé en français » en bas de page.*

## 1.0.0 — parity release (current work)

Functional parity with PingCastle 3.0.0.0, rebranded, every license/edition mechanism removed, every vendor
network call removed, fully commented code, CI and signed-off release process. See `CHANGELOG.md`.

### Not ported in 1.0.0 (upstream features that depend on a proprietary server)

| Upstream feature | Why it is not ported | Status |
|---|---|---|
| Enterprise console upload (`--api-endpoint`, `--api-key`, `--upload-all-reports`, agent settings, custom rules pushed by the server) | Requires the PingCastle Enterprise server, which is closed source and not part of the imported code. | Options removed in Phase 4; reports can still be sent by e-mail (`--sendXmlTo`) or to a WebDAV share (`--webdirectory`). |
| "Compare with statistics" benchmark button in the HTML report | Posted the domain hash, scores and the license key to `stat.pingcastle.com`. | Removed in Phase 3 (no replacement). |
| `PingCastleAutoUpdater.exe` | Downloaded releases from the vendor's GitHub and replaced the binaries in place. | Replaced by the opt-in `--check-update` command (GitHub Releases API, no download) in Phase 4. |
| Edition-specific "Customer notice", domain limitation and expiry date | License mechanism. | Removed in Phase 3. |
| Report integrity warning ("the report has been modified before being uploaded") | Tied to the Enterprise console workflow but harmless; kept as upstream computed it. | Kept, see `TODO(review)` in `Report/ReportHealthCheckSingle.cs`. |

## 1.1 — fixes and community rules

- Review and cherry-pick the upstream fixes between tags `3.0.0.0` and `3.0.0.4` (still NPOSL-3.0): LDAP
  signing/channel binding tests, SMB protocol probes, `A-CertEnrollChannelBinding`, report CSS.
- `--prefix <name>` option to rename the generated files (default stays `ad_hc_`), see ADR-0005.
- `--version` output with upstream baseline information; `--check-update` improvements (proxy settings).
- Community rules: template, contribution guide, first external rules (ANSSI "points de contrôle" not yet
  covered).
- French translation of the HTML report rule texts (resx satellite assembly).

## 2.0 — modernisation

- Migrate to .NET 8 (multi-target `net8.0-windows` + `net48`), replace WCF/ADWS client with a maintained
  stack, drop the Samba/Linux P/Invoke experiments or make them real.
- Modern HTML report: Bootstrap 5.3, no inline JavaScript blobs, accessible tables, dark mode, printable.
- Internationalisation FR/EN of reports and CLI.
- Rule set aligned with the ANSSI AD security guide (`CERTFR-2020-DUR-001` and later) and CIS benchmarks,
  with a documented mapping table.
- Normalised JSON export (stable schema, versioned) for SIEM/BI ingestion.

## Community ideas (not scheduled)

- SIEM integration samples (Splunk, Elastic, Sentinel) based on the JSON export.
- CI mode: non-zero exit code when the score crosses a threshold, SARIF output.
- Historical diff between two reports with a readable "what changed" summary.
- Container / PowerShell module packaging.
- Rule authoring kit with synthetic AD fixtures for unit tests.

## Résumé en français

- **1.0.0** : parité avec PingCastle 3.0.0.0, rebrandée, sans licence ni appel réseau éditeur, code commenté,
  CI et release. Non portés : console Enterprise (`--api-endpoint`), bouton « Compare with statistics »,
  auto-updater (remplacé par `--check-update` opt-in).
- **1.1** : correctifs upstream 3.0.0.3/3.0.0.4, option `--prefix`, règles communautaires, textes FR.
- **2.0** : migration .NET 8 multi-cible, rapport HTML modernisé, i18n FR/EN, alignement ANSSI/CIS, export JSON
  normalisé.
- **Idées** : intégration SIEM, mode CI (code retour, SARIF), diff entre rapports, module PowerShell.
