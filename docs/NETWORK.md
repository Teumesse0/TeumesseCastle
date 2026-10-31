# Network behaviour of TeumesseCastle

*Résumé en français en fin de page.*

TeumesseCastle is run on domain controllers, jump hosts and auditor laptops. This document lists **every**
network connection the program can open, when, and to whom. It is a maintained reference: any change to the
network behaviour must update this file (see `CLAUDE.md`, rule 4).

## Guarantees

- **No telemetry, ever.** Nothing is sent to Teumesse, to the former vendor or to any third party.
- **No automatic update.** The only non-infrastructure HTTP call is the opt-in `--check-update` command.
- **No license server.** There is no license, no activation, no expiry check.
- Everything else targets the infrastructure **you** point the tool at (your Active Directory, your
  Azure AD tenant, your SMTP relay, your WebDAV share).

## 1. Connections to the audited infrastructure

| Feature | Protocol / port | Target | Notes |
|---|---|---|---|
| Domain controller discovery | DsGetDcName (DC locator), DNS (53/UDP+TCP) | your DNS / DCs | `ADWS/DomainLocator.cs`, `misc/DnsQuery.cs` |
| Directory queries (default) | ADWS (SOAP over TCP **9389**) | the DC named by `--server` | `ADWS/ADWSConnection.cs`; falls back to LDAP unless `--protocol ADWSOnly` |
| Directory queries (fallback or `--protocol LDAPOnly`) | LDAP **389**, LDAPS **636**, Global Catalog 3268/3269 | DC | `ADWS/LDAPConnection.cs` (System.DirectoryServices.Protocols) |
| LDAP signing / channel-binding checks | LDAP 389, LDAPS 636 | each DC | `misc/ConnectionTesterLdap.cs`; deliberately tries weak settings to test the DC |
| SYSVOL / GPO analysis, LAPS/BitLocker, share scanner | SMB **445** (and 139) | DCs and scanned computers | `ADWS/WindowsFileConnection.cs`, `shares/` |
| SMB protocol and signing probes, MS17-010, SMBv1 | SMB 445 (raw protocol implementation) | scanned computers / DCs | `Scanners/Smb1Protocol.cs`, `Smb2Protocol.cs`, `ms17_010scanner.cs` |
| Null session, SAMR, LSA, NetLogon (ZeroLogon), spooler, OXID | MS-RPC over SMB named pipes (445) and TCP 135 (endpoint mapper, dynamic ports) | DCs / scanned computers | `RPC/*.cs`, `Scanners/ZeroLogonScanner.cs`, `SpoolerScanner.cs`, `OxidBindingScanner.cs` |
| WebDAV client detection on DCs | SMB named pipe `DAV RPC SERVICE` | each DC | `misc/NamedPipeTester.cs` |
| Certificate enrollment web services, WSUS checks | HTTP **80** / HTTPS **443** to the servers declared in your AD | your CA / WSUS servers | `misc/ConnectionTesterHttp.cs`; only when the AD advertises such endpoints |
| RDP (BlueKeep) scanner | TCP **3389** | scanned computers | `Scanners/bluekeepscanner.cs` |
| Generic port checks | TCP connect to the port under test | DCs | `Healthcheck/HealthcheckAnalyzer.cs` (`TcpClient`) |
| Local admin / antivirus / startup scanners | SMB, RPC, remote registry (445, 135) | scanned computers | `Scanners/` |
| Samba / Linux experiments | SMB via `libPingCastlesmb` (P/Invoke, library not shipped) | n/a on Windows | `ADWS/LinuxConnection.cs`, only compiled, never used on Windows |

All of these use the credentials of the current user or those given with `--user` / `--password`.

## 2. Azure AD / Entra ID audit (`--azuread` only)

Only when you run the cloud health check. Targets are Microsoft endpoints, with the credential you provide
(PRT, certificate or user password):

- `login.microsoftonline.com`, `login.windows.net` (OAuth 2.0 / OpenID Connect tokens)
- `graph.microsoft.com`, `graph.windows.net` (Microsoft Graph, Azure AD Graph)
- `management.azure.com`, `management.core.windows.net`, `main.iam.ad.ext.azure.com` (Azure management)
- `provisioningapi.microsoftonline.com`, `provisioning.microsoftonline.com` (provisioning API)
- `outlook.office365.com` (Exchange Online)
- tenant discovery endpoints (`/common/userrealm`, `GetUserRealm.srf`, `.well-known/openid-configuration`)

Implementation: `Cloud/` (`HttpClient`). A Fiddler-compatible `.saz` trace of these calls can be enabled
with `--log-samba`/`--log` for debugging.

## 3. Outputs you explicitly configure

| Feature | Switch | Protocol | Target |
|---|---|---|---|
| Send reports by e-mail | `--sendXmlTo`, `--sendHtmlTo`, `--sendAllTo` (+ `--smtplogin`, `--smtppass`, `--smtptls`) | SMTP (25/465/587) | the relay declared in `TeumesseCastle.exe.config` (`<mailSettings>`) |
| Upload XML reports to a WebDAV share | `--webdirectory` (+ `--webuser`, `--webpassword`) | HTTP(S) PUT | the URL you give |

Nothing is sent unless one of these switches is present.

## 4. The only non-infrastructure call: `--check-update` (opt-in)

| Item | Value |
|---|---|
| Trigger | explicit `--check-update` switch, or "checkupdate" in the interactive advanced menu. **Never automatic.** |
| Request | one `GET https://api.github.com/repos/Teumesse0/TeumesseCastle/releases/latest` |
| Headers sent | `User-Agent: TeumesseCastle/<version>`, `Accept: application/vnd.github+json` |
| Data sent | none (no domain name, no identifier, no statistics) |
| Timeout | 5 seconds |
| Result | one line on the console: up to date / newer release available with its URL. **Nothing is downloaded.** |
| Errors | silent on the command line (written to the trace only), shown in the interactive menu |
| Proxy | the system (WinHTTP / Internet Options) proxy, as for any `HttpWebRequest` |
| Code | `misc/UpdateChecker.cs` |

## 5. Removed from PingCastle 3.0.0.0

| Upstream behaviour | Status in TeumesseCastle |
|---|---|
| `PingCastleAutoUpdater.exe` downloading releases from the vendor's GitHub and replacing binaries | removed (project deleted) |
| "Compare with statistics" button posting domain hash, scores and license key to `stat.pingcastle.com` | removed |
| Enterprise console upload (`--api-endpoint`, `--api-key`, `--upload-all-reports`, settings and licenses pushed by the server) | removed (see `docs/ROADMAP.md`) |
| Content-Security-Policy of the HTML report allowing `www.pingcastle.com` as image source | replaced by `data:` only |

## 6. Links inside the reports

The HTML reports contain documentation links (Microsoft Learn, ANSSI, MITRE ATT&CK, CIS, blogs) next to each
rule. They are plain hyperlinks: nothing is fetched when the report is generated or opened, except if you click
them. Report assets (Bootstrap, jQuery, vis.js, fonts, images) are embedded in the file.

## 7. How to verify

```powershell
# no vendor domain left in the code
git grep -n -i -E "pingcastle\.com|netwrix" -- "*.cs" "*.config" "*.js" "*.html" | grep -v "^// "   # headers only
# every HTTP client in the code base
git grep -n -E "HttpWebRequest|WebClient\b|HttpClient\b" -- "*.cs"
```

A network capture (Wireshark, `pktmon`) during `--healthcheck` must show only the protocols of section 1.

## Résumé en français

- Aucune télémétrie, aucune mise à jour automatique, aucun serveur de licence.
- Connexions vers l'infrastructure auditée uniquement : DNS, ADWS 9389, LDAP/LDAPS 389/636, SMB 445, RPC 135 et
  ports dynamiques, HTTP(S) vers les serveurs de certificats/WSUS déclarés dans l'AD, RDP 3389 pour le scanner
  BlueKeep.
- `--azuread` : endpoints Microsoft (login, Graph, management, provisioning, Exchange Online) avec vos identifiants.
- Sorties explicites : SMTP (`--sendXmlTo`…) et WebDAV (`--webdirectory`).
- `--check-update` : un seul GET vers l'API GitHub Releases, sur demande, aucun téléchargement, erreurs
  silencieuses hors mode interactif.
- Supprimés par rapport à PingCastle : auto-updater, statistiques `stat.pingcastle.com`, console Enterprise.
