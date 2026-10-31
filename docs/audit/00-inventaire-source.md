# Audit Phase 0 — Inventaire de la source PingCastle 3.0.0.0

Date : 2026-10-31
Auteur : session Claude Code (ingénieur principal TeumesseCastle)
Statut : **aucune modification de code effectuée** (seuls `.gitignore` et `docs/` ont été touchés).

## 0. Provenance

| Élément | Valeur |
|---|---|
| Dépôt upstream | https://github.com/netwrix/pingcastle (ex `vletoux/pingcastle`) |
| Tag retenu | `3.0.0.0` |
| Commit | `d8f21e920278b82f243bdfe84d168d8040a0b96c` |
| Date du commit | 2023-02-21 19:24:59 +0100 |
| Auteur du commit | Vincent LE TOUX |
| Message | `PingCastle 3.0.0.0` |
| Licence dans le tag | `LICENSE.md` à la racine = **Non-Profit Open Software License 3.0 (NPOSL-3.0)** — confirmé |
| `license.rtf` dans le tag | NPOSL-3.0, « Copyright (C) 2016-2018 Ping Castle SAS » |
| Clone local temporaire | `..\_pingcastle-upstream` (hors projet, à supprimer en fin de Phase 1) |

Tags 3.0.x disponibles : `3.0.0.0`, `3.0.0.0_beta1`, `3.0.0.0_beta2`, `3.0.0.3`, `3.0.0.4`, et `3.0.0.0_.net_4.5.2` (attention : ce dernier pointe en réalité sur un commit de 2024-09-13 « PingCastle 3.3.0.0 (#256) », à ne **pas** utiliser).

Cohérence avec les binaires de référence (`reference/pingcastle-3.0.0.0-binaries/`) :

| Vérification | Résultat |
|---|---|
| `PingCastle.exe` FileVersion / ProductVersion | 3.0.0.0 / 3.0.0.0 |
| Company / Product / Copyright du binaire | Ping Castle / Ping Castle / « Copyright © 2015-2022 Ping Castle » = identique à `Properties/AssemblyInfo.cs` du tag |
| `changelog.txt` binaire vs tag | **identique** |
| `license.rtf` binaire vs tag | contenu NPOSL identique, encodage/longueur différents (10 705 vs 12 392 octets) — non bloquant |
| `PingCastle.exe.config` vs `app.config` du tag | diffère : le binaire ajoute la section `customRulesSettings` et commente les HoneyPots/Riverbeds (voir §7) |
| `Newtonsoft.Json.dll` | 13.0.1.25517 (non suivi dans Git upstream, `*.dll` ignoré ; requis par `HintPath=.\Newtonsoft.Json.dll`) |

Évolution de la licence upstream : `LICENSE.md` reste NPOSL-3.0 sur tous les tags testés jusqu'à `4.0.0.33`. Le passage au modèle payant Netwrix se traduit par des conditions d'usage et un code applicatif propriétaire côté serveur, pas par un changement de `LICENSE.md`. **Règle projet maintenue** : aucun code postérieur au tag `3.0.0.0` n'entre dans TeumesseCastle sans décision explicite (le diff `3.0.0.0 → 3.0.0.4` touche 33 fichiers, principalement des correctifs ; candidat éventuel pour v1.1, à arbitrer).

## 1. Arborescence et rôle des dossiers

| Dossier | .cs | LOC | Rôle |
|---|---|---|---|
| `/` (racine) | 9 | 4 523 | `Program.cs` (point d'entrée, parsing CLI, menu interactif), `Tasks.cs` (orchestration des tâches), `RuntimeSettings.cs` (état des options), `ConsoleMenu.cs`, `License.cs` (**mécanisme de licence**), `NativeMethods.cs`, `PingCastleFactory.cs` (découverte réflexive des scanners/exports), `PingCastleException.cs`, `Compatibility.cs` |
| `ADWS/` | 15 | 12 478 | Connecteurs annuaire : `ADWSConnection` (SOAP/WS-Enumeration, `wse.cs` = 7 953 lignes générées depuis les WSDL), `LDAPConnection` (System.DirectoryServices.Protocols), `ADItem` (modèle objet AD), `ADDomainInfo`, `DomainLocator` (DsGetDcName), `LinuxConnection`/`LinuxSidResolver` (Samba via P/Invoke `libPingCastlesmb`), `WindowsFileConnection` (SYSVOL/SMB) |
| `Healthcheck/` | 8 | 9 230 | Collecte : `HealthcheckAnalyzer.cs` (5 552 lignes, toute la collecte LDAP/SMB/RPC/DNS), `Healthcheck.cs` (ancien `ADSecurityHealthCheck`, modèle intermédiaire), `TrustAnalyzer`, `HealthCheckEncryption` (chiffrement RSA/AES des XML), `FakeHealthCheckDataGenerator` (rapports démo), `HoneyPotSettings`, `InfrastructureSettings` (sections app.config) |
| `Healthcheck/Rules/` | 171 | 7 283 | **170 règles** (`[RuleModel]`) : 68 Anomalies (A-*), 43 PrivilegedAccounts (P-*), 48 StaleObjects (S-*), 11 Trusts (T-*) + `RuleDescription.resx` (1 327 entrées : titres, descriptions, solutions, rationales) |
| `Rules/` | 9 | 1 472 | Moteur de règles : `RuleBase<T>`, `RuleSet<T>`, attributs (`RuleModel`, `RuleComputation`, `RuleMaturityLevel`, `RuleANSSI`, `RuleSTIG`, `RuleMitreAttack`…), catégories/modèle de risque, `CustomRulesSettings` (surcharge des scores via config) |
| `Report/` | 14 | 12 105 | Génération HTML : `ReportBase` (squelette, favicon/logo base64, pied de page), `ReportHealthCheckSingle` (4 691 lignes), `ReportHealthCheckConsolidation`, `ReportMapBuilder`/`ReportNetworkMap` (cartographie), `ReportHealthCheckRules`, `ReportRiskControls`, `ReportCloud`, `ReportBenchmark` (**POST vers stat.pingcastle.com**) |
| `Data/` | 12 | 3 301 | Modèle sérialisé XML : `HealthcheckData` (1 682 lignes, DataContract), `CompromiseGraphData`, collections de rapports, `SafeXmlWriter`, niveaux d'export |
| `Graph/` | 9 | 3 569 | Graphe de compromission (anciennement « graph » / `--carto`) : `Database/` (nœuds/relations en mémoire), `Export/ExportDataFromActiveDirectoryLive` + `RelationFactory` (extraction ACL/membres/GPO), `Reporting/ReportGenerator` |
| `Scanners/` | 24 | 4 764 | Scanners réseau par machine : antivirus, bluekeep, computerversion, localadmin, ms17-010, nullsession, oxidbindings, remote, smb, smb3querynetwork, spooler, startup, zerologon, + ACL, LAPS/BitLocker, consistency, foreign users, replication ; `Smb1Protocol`/`Smb2Protocol` (implémentation SMB bas niveau) |
| `shares/` | 2 | 259 | Scanner `share` (énumération de partages) |
| `Exports/` | 5 | 482 | `--export users|computers|changes` (CSV) |
| `RPC/` | 9 | 3 006 | P/Invoke RPC natif : `lsa`, `samr`, `nrpc` (ZeroLogon), `spool`, `dcom` (OXID), `nullsession`, `SSPI`, `rpcapi` |
| `Cloud/` | 51 | 7 739 | Audit Azure AD / Entra ID (`--azuread`) : credentials (PRT, cert, user), tokens JWT, REST (Graph, Provisioning, Management, O365), 6 règles cloud, UI WinForms `AuthenticationDialog`, logs SAZ |
| `Bot/` | 3 | 279 | Mode « bot » : exécution pilotée par named pipe local (XML in/out). Désactivé dans `Program.cs` (ligne `//{PossibleTasks.Bot, …}`) |
| `misc/` | 11 | 1 721 | Utilitaires : `ConnectionTester` (LDAP/HTTP vers le DC, test signing/channel binding), `DnsQuery`, `NamedPipeTester`, `RegistryPolReader` (GPO .pol), `ROCAVulnerabilityTester`, `RestrictedToken`, `Subnet`, `JSONParser` (TinyJson) |
| `template/` | 1 | 160 | `TemplateManager.cs` + ressources HTML/CSS/JS embarquées gzippées (bootstrap 5, bootstrap-table, jquery, popper, vis.js, DataTables, `responsivetemplate.html`, CSS/JS du rapport) ; `ProcessTemplate.ps1` regénère les `.gz` en pré-build |
| `PingCastleAutoUpdater/` | 2 | 309 | Exécutable séparé : télécharge la dernière release GitHub (`api.github.com/repos/vletoux/pingcastle/releases`) et remplace les fichiers |
| `Properties/` | 1 | — | `AssemblyInfo.cs` |

Fichiers non-C# : 3 `.resx`, 14 `.gz` (ressources embarquées, pourtant listées dans `template/.gitignore` mais présentes dans le tag), 7 `.css`, 5 `.js`, 1 `.html`, 2 `.ps1`, 2 `.ico`, 2 `.csproj`, 1 `.sln`, 2 `.config`, `LICENSE.md`, `README.md`, `changelog.txt`, `license.rtf`.

Dossier absent du tag mais référencé par le `.csproj` : `ADWS/wsdl/*` (items `<None>`, ignorés par `.gitignore` upstream) — sans impact sur la compilation.

## 2. Solution, projets, dépendances

### `PingCastle.sln`
Format VS 2017 (15.0). Deux projets : `PingCastle.csproj` (`{52BBA3C2-…}`) et `PingCastleAutoUpdater\PingCastleAutoUpdater.csproj` (`{0E5D043A-…}`). Configurations Debug/Release × AnyCPU/x64/x86 (x64 et x86 mappés sur AnyCPU).

### `PingCastle.csproj` (format legacy, `ToolsVersion=4.0`, non SDK-style)

| Propriété | Valeur |
|---|---|
| `OutputType` | Exe |
| `RootNamespace` / `AssemblyName` | `PingCastle` / `PingCastle` |
| `TargetFrameworkVersion` | **v4.5.2** |
| `PlatformTarget` / `Prefer32Bit` | AnyCPU / false |
| `ApplicationIcon` | `pingcastle.ico` |
| `SignAssembly` | false |
| `<Compile Include>` | 350 fichiers |
| Pré-build | `powershell -File template\ProcessTemplate.ps1` (gzip des templates) |
| Post-build | `signtool` conditionné à `COMPUTERNAME == DESKTOP-QCK4J75` (poste de l'auteur, certificat « Ping Castle SAS ») → **inopérant ailleurs, à supprimer en Phase 2** |
| ClickOnce | propriétés `Publish*`/`Bootstrapper` résiduelles (.NET 3.5 SP1), inutilisées |

Références (toutes GAC .NET Framework) : System, System.configuration, System.Data, System.DirectoryServices, System.DirectoryServices.Protocols, System.Drawing, System.Net.Http, System.Runtime.Serialization, System.Security, System.ServiceModel (WCF pour ADWS), System.Web, System.Windows.Forms, System.Xml, WindowsBase.
Dépendance externe unique : **Newtonsoft.Json 13.0.0.0** via `<HintPath>.\Newtonsoft.Json.dll</HintPath>` (pas de NuGet, DLL non versionnée dans Git).

Ressources embarquées : `Cloud\Rules\RuleDescription.resx`, `Cloud\UI\AuthenticationDialog.resx`, `Healthcheck\Rules\RuleDescription.resx`, et 23 fichiers `template\*.gz`. Les 3 `.resx` contiennent des entrées non-chaînes (`Color1`, `Bitmap1`, `Icon1` — gabarits inutilisés, aucune référence dans le code — et `$this.Icon` du formulaire WinForms, utilisé).

### `PingCastleAutoUpdater.csproj`
`TargetFrameworkVersion` **v4.8**, références System.Web.Extensions (JavaScriptSerializer), System.IO.Compression. Pas de dépendance externe.

### P/Invoke (COM : aucun `ComImport`)
134 `DllImport` dans 15 fichiers : `Rpcrt4.dll` (23), `advapi32` (12), `secur32` (13), `samlib.dll` (6), `crypt32` (7), `netapi32` (8), `kernel32` (5), `Dnsapi.dll` (2), `urlmon.dll`, `shell32.dll`, `IEFRAME.dll` (cookies WebBrowser, Cloud), `libPingCastlesmb` (Linux/Samba, non fourni). Fichiers : `RPC/nativemethods.cs` (25), `NativeMethods.cs` (22), `ADWS/LinuxConnection.cs` (21), `ADWS/LinuxFileConnection.cs` (15), `ADWS/LinuxSidResolver.cs` (14), `RPC/SSPI.cs` (13), etc.

## 3. Licence réelle

| Source | Constat |
|---|---|
| `LICENSE.md` (racine) | NPOSL-3.0, texte intégral (« Full License Text ») |
| `license.rtf` | NPOSL-3.0, « Copyright (C) 2016-2018 Ping Castle SAS » |
| En-têtes `.cs` | **296 / 356** fichiers portent `// Licensed under the Non-Profit OSL. See LICENSE file…` ; 246 avec `Copyright (c) Ping Castle. All rights reserved.`, 50 avec `Copyright (c) Vincent LE TOUX for Ping Castle. All rights reserved.` (tout `Cloud/`). **60 fichiers sans en-tête** (liste en annexe A) |
| `Properties/AssemblyInfo.cs` | Company/Product « Ping Castle », Copyright « © 2015-2022 Ping Castle », version 3.0.0.0, GUID `ce59f8ff-0ecf-41e9-a1fd-1776ca0b703d` |
| `README.md` | mentionne « To build services based on PingCastle AND earning money from that, you MUST purchase a license » (clause commerciale de l'éditeur ; la NPOSL interdit l'usage commercial par le *licencié* — point juridique à garder en tête : voir ADR-0003) |

**Confirmé** : NPOSL-3.0, titulaire Ping Castle SAS / Vincent Le Toux. Correction par rapport à l'attendu : le fichier s'appelle `LICENSE.md` et non `LICENSE`.

## 4. Inventaire des occurrences à rebrander

Comptage (hors `reference/` et `.gz`) :

| Motif | Lignes | Fichiers |
|---|---|---|
| `PingCastle` (insensible à la casse) | 1 547 | 361 |
| dont `PingCastle` / `pingcastle` / `Ping Castle` / `pingCastle` | 1 239 / 341 / 306 / 15 | |
| `pingcastle.com` | 318 | 298 |
| `Vincent Le Toux` | 53 | 52 |
| `@pingcastle` (emails) | 7 | 4 |
| `mysmartlogon` | 3 | 3 |
| `Netwrix` | 1 | 1 (`blog.netwrix.com`, lien de documentation d'une règle) |
| `ADSecurityHealthCheck` (ancien nom du produit, namespace résiduel) | 8 | 5 |

Classement :

**(a) Namespaces / identifiants de code** — `namespace PingCastle[.X]` : 353 déclarations dans 25 sous-namespaces ; `using PingCastle…` : 389 ; namespaces hérités `ADSecurityHealthCheck[.ADWS|.Healthcheck|.Healthcheck.Rules]` (5 fichiers) ; namespace `PingCastleAutoUpdater`. Types : `PingCastleException`, `PingCastleFactory`, `IPingCastleReport`, `IPingCastleReportGenerator`, `IPingCastleReportUser`, `PingCastleReportCollection`, `PingCastleReportHelper`, `PingCastleReportHistoryCollection`, `PingCastleReportDataExportLevel`, `PingCastleCloudException`, `IPingCastleLicenseInfo`, `PingCastleEnterpriseBaseUrl`. Fichiers : `PingCastle.csproj`, `PingCastle.sln`, `PingCastleException.cs`, `PingCastleFactory.cs`, `pingcastle.ico` (×2), dossier `PingCastleAutoUpdater/`. Config : `app.config` sections `type="PingCastle.…, PingCastle"` (4). Chargement de ressources par namespace : `TemplateManager` utilise `typeof(TemplateManager).Namespace + ".xxx.gz"` → le renommage du namespace renomme automatiquement les noms de ressources manifest (cohérent tant que le `RootNamespace` suit).

**(b) Chaînes affichées à l'utilisateur** — bannière `ConsoleMenu.Header` (`Program.cs:210-216` : « PingCastle (Version …) », « Vincent LE TOUX (contact@pingcastle.com) », « twitter: @mysmartlogon », URL) ; textes d'aide `DisplayHelp()` (« rules used by PingCastle ») ; menu interactif (« rules applied by PingCastle ») ; messages d'erreur de licence (`License.cs:89,94`, `Program.cs:313`) ; rapports : `Report/ReportHealthCheckSingle.cs:108-109` et `ReportCloud.cs:86-87` (« Basic Edition of PingCastle »), `:192` (« PingCastle has detected that the report has been modified »), textes de la modale « Privacy notice » (`ReportHealthCheckSingle.cs:143-176`), `ReportBase.cs:765` (« Generated by Ping Castle all rights reserved »), paragraphes « This feature is reserved for customers who have purchased a license » (7 occurrences, cf. §5) ; `README.md` ; traces `Trace.WriteLine("PingCastle version …")`.

**(c) Noms de fichiers de sortie** — `Data/HealthcheckData.cs:1154,1159` (`ad_hc_<domaine>[_date].html|.xml`) ; `Tasks.cs` : `ad_scanner_<nom>_<serveur>.txt`, `ad_export_<nom>_<serveur>.txt`, `ad_carto_full_node_map.html`, `ad_carto_simple_node_map.html`, `ad_hc_summary.html`, `ad_hc_summary_full_node_map.html`, `ad_hc_summary_simple_node_map.html`, `ad_hc_hilbert_map.html`, `ad_hc_rules_list.html` ; détection `fi.Name.StartsWith("ad_hc_")` (`Tasks.cs:661`, consolidation). Aucun préfixe ne contient « pingcastle » : **décision ad_hc_ vs tc_hc_ à arbitrer en Phase 2** (recommandation : conserver `ad_hc_` en v1).

**(d) URLs** — `https://www.pingcastle.com` : 296 en en-tête de fichier (ligne 3 du bloc copyright, à conserver tel quel car partie de la notice d'origine), `Program.cs:216` (bannière), `README.md`, `Report/ReportBase.cs:27` (`PingCastleEnterpriseBaseUrl = https://www.pingcastle.com/reports/`), `:147` (CSP `base-uri`/`img-src https://www.pingcastle.com`), `:765` (lien du pied de page), `ReportHealthCheckSingle.cs` ×5 et `ReportCloud.cs` ×2 (`/services/` « purchased a license ») ; `https://stat.pingcastle.com/Benchmark` (`ReportBenchmark.cs:16`) ; `https://api.github.com/repos/vletoux/pingcastle/releases` (`PingCastleAutoUpdater/Program.cs:44`) ; emails `contact@pingcastle.com`, `support@pingcastle.com`. **À conserver** (sources, pas branding) : ~700 URLs de documentation (docs/learn/support.microsoft.com, adsecurity.org, ssi.gouv.fr, cert.ssi.gouv.fr, attack.mitre.org, stigviewer.com, bsi.bund.de, specterops, harmj0y, etc.) et les namespaces XML `schemas.microsoft.com`, `schemas.xmlsoap.org`, `schemas.dmtf.org`, `w3.org`.

**(e) Mentions légales / copyright à conserver** — les 296 en-têtes (`Copyright (c) Ping Castle` / `Copyright (c) Vincent LE TOUX for Ping Castle` + URL + mention NPOSL), `LICENSE.md`, `license.rtf`, `changelog.txt` (historique des versions, mentionne PingCastle), `AssemblyCopyright` d'origine (à reproduire en « double copyright »), crédits des composants open source du pied de page (Bootstrap, jQuery, Popper, vis.js, DataTables, bootstrap-table).

## 5. Mécanisme de licence et éditions

Aucun fichier `.lic` : la licence est une chaîne base64 (`PC2…`) signée RSA-1024/SHA-1, lue depuis `--license <clé>` ou `app.config` `<LicenseSettings license="" />`, avec repli sur une **clé « Basic » embarquée** dans le code.

| # | Fichier:ligne | Point de contrôle | Comportement « Basic » (édition vide) | Branche à conserver (Phase 3) |
|---|---|---|---|---|
| 1 | `License.cs:16-21, 26-29, 31-51, 53-73, 75-305` | `ADHealthCheckingLicenseSettings` (section config), `IPingCastleLicenseInfo`, `ADHealthCheckingLicenseProvider`, `ADHealthCheckingLicense` (parsing V1/V2, `VerifySignature`, **clé publique RSA** `LoadRSAKey()` ligne 265) | — | Supprimer la classe ; remplacer par un objet « édition complète » neutre ou supprimer les paramètres `license` |
| 2 | `Program.cs:49-50` | `[LicenseProvider(typeof(ADHealthCheckingLicenseProvider))]`, `Program : IPingCastleLicenseInfo` | — | Retirer l'attribut et l'interface |
| 3 | `Program.cs:113-120` | options `--debug-license`, `--license <clé>` | — | Retirer (et ligne `case "--license"` 527 + aide) |
| 4 | `Program.cs:131-170` | `LicenseManager.Validate(...)` ; abandon si exception ; **arrêt du programme si `EndTime < DateTime.Now`** (« The program is unsupported since ») | la clé Basic embarquée porte une date de fin (« End of support » affiché dans la bannière) | Supprimer tout le bloc ; pas d'expiration |
| 5 | `Program.cs:213` | bannière « End of support: … » | affiché | Supprimer |
| 6 | `Program.cs:224-232` | `DomainLimitation` (restriction du `--server`), `CustomerNotice` | vides | Supprimer |
| 7 | `Program.cs:247-256` `LoadCustomRules` | `RuleSet.LoadCustomRules()` **uniquement si édition ≠ vide/Basic/Auditor** | désactivé | **Activer inconditionnellement** (lecture de `customRulesSettings` dans app.config) |
| 8 | `Program.cs:258-316` | `basicEditionLicense` (clé embarquée), `GetSerialNumber()` | — | Supprimer |
| 9 | `Tasks.cs:35` | propriété `License` passée à tous les générateurs de rapports | — | Supprimer le paramètre dans `IPingCastleReportUser.GenerateReportFile(…, license, …)` et implémentations (`Report/*.cs`) |
| 10 | `Tasks.cs:124,153,498,547` | `limitHoneyPot = string.IsNullOrEmpty(License.Edition)` → limite le nombre de honeypots en Basic | limité | `limitHoneyPot = false` (fonctionnalité activée) — **vérifier dans `HealthcheckAnalyzer` la sémantique exacte avant modification** |
| 11 | `Tasks.cs:940-957` | licence renvoyée par l'API Enterprise (`ProcessSettings`) | n/a | Supprimé avec la fonctionnalité API (voir §6 et ROADMAP) |
| 12 | `Tasks.cs:982` | `if (IsNullOrEmpty(Edition) || Edition == "Auditor")` dans `GenerateDemoReportTask` | — | Lire le corps avant décision (TODO Phase 3) |
| 13 | `Report/ReportBase.cs:634-651` `Brand()` | personnalisation `BrandLogo/BrandCss/BrandJs` depuis app.config **uniquement si édition payante** | désactivé | Activer inconditionnellement (utile pour la communauté) |
| 14 | `Report/ReportHealthCheckSingle.cs:79` | appel `Brand()` conditionnel | — | idem 13 |
| 15 | `Report/ReportHealthCheckSingle.cs:108-117` | affiche « Basic Edition of PingCastle » + `CustomerNotice` | affiché | Remplacer par mention TeumesseCastle |
| 16 | `Report/ReportHealthCheckSingle.cs:192-198` | alerte « report modified » si édition payante | masqué | Lire `IntegrityVerified` ; conserver l'alerte (information utile) |
| 17 | `Report/ReportHealthCheckSingle.cs:253-256, 462-465, 946-949, 1589-1592, 3082-3085` | 5 sections remplacées par « This feature is reserved for customers who have purchased a license » | masquées | **Afficher la branche complète** (contenu à vérifier section par section : lire chaque bloc `else`) |
| 18 | `Report/ReportHealthCheckConsolidation.cs:52, 166` | `Brand()` + une section conditionnelle | masquée | Afficher |
| 19 | `Report/ReportCloud.cs:86-96, 393-396, 602-605` | édition + 2 sections réservées | masquées | Afficher |
| 20 | `Report/ReportBenchmark.cs:20-29, 82` | la clé de licence est **envoyée** à stat.pingcastle.com avec le hash du domaine en Basic | envoyé au clic utilisateur | Supprimer la fonctionnalité (voir §6) |
| 21 | `Bot/Bot.cs:198` | `LicenseManager.Validate` dans le mode bot | — | Supprimer |
| 22 | `app.config:4, 41-42` | section `LicenseSettings` | — | Supprimer |
| 23 | `README.md:41,52` | mentions Basic Edition / achat de licence | — | Réécrit en Phase 6 |

Chaînes `IsBasic/IsPro/IsEnterprise/IsAuditor` : **absentes** ; les tests se font par comparaison de `license.Edition` avec `""`, `"Basic"`, `"Auditor"`, `"Pro"` (`License.cs:170`, limite `DomainNumberLimit`).

## 6. Appels réseau sortants

| # | Fichier:ligne | Mécanisme | Destination | Déclenchement | Décision proposée |
|---|---|---|---|---|---|
| 1 | `Report/ReportBenchmark.cs:11-18` + `ReportHealthCheckSingle.cs:177-186` | `<form method=POST>` injecté dans le rapport HTML (licence + JSON stats + HMAC) | `https://stat.pingcastle.com/Benchmark` | clic utilisateur « Compare with statistics » dans le rapport | **Supprimer** (service de l'éditeur) |
| 2 | `PingCastleAutoUpdater/Program.cs:44,164,195` | `HttpWebRequest` GET JSON releases + téléchargement zip + remplacement des fichiers | `https://api.github.com/repos/vletoux/pingcastle/releases` | exécution manuelle de l'exe | **Supprimer le projet** ; remplacer par `--check-update` opt-in sans téléchargement (Phase 4) |
| 3 | `Tasks.cs:773-930, 1065-1130` | `WebClient.UploadData/DownloadString` vers `api/Agent/Login`, `SendReport`, `GetSettings` | `--api-endpoint` fourni par l'utilisateur (console PingCastle Enterprise) | `--api-endpoint` + `--api-key`, `--upload-all-reports` | Fonction dépendant d'un serveur propriétaire inexistant dans la source → **retirer l'option, documenter dans ROADMAP** (à valider Phase 3/4) |
| 4 | `Tasks.cs:1214-1230` | `WebClient.UploadData(url, "PUT")` | `--webdirectory` (WebDAV utilisateur) | explicite | Conserver (destination choisie par l'utilisateur) |
| 5 | `Tasks.cs:1160-1212` | `SmtpClient` | `--sendXmlTo/--sendHtmlTo` + `<mailSettings>` | explicite | Conserver |
| 6 | `Report/ReportBase.cs:147` | CSP `base-uri`/`img-src https://www.pingcastle.com` (pas une requête, mais une autorisation) | — | — | Remplacer par `'self'`/`data:` |
| 7 | `Cloud/**` (`HttpClientHelper`, `TokenFactory`, `PublicService`, `RESTClientBase`…) | `HttpClient` | `login.microsoftonline.com`, `graph.microsoft.com`, `graph.windows.net`, `management.azure.com`, `provisioning*.microsoftonline.com`, `outlook.office365.com`, `main.iam.ad.ext.azure.com` | `--azuread` uniquement | Conserver (cible auditée) |
| 8 | `misc/ConnectionTesterHttp.cs`, `misc/ConnectionTesterLdap.cs`, `HealthcheckAnalyzer.cs:5085,5114` (`TcpClient`), `NamedPipeTester`, `RPC/*`, `Scanners/Smb*`, `ADWS/*`, `DnsQuery` | LDAP/LDAPS/ADWS(9389)/SMB/RPC/HTTP(S)/DNS/named pipes | DC et machines du domaine audité | healthcheck/scanners | Conserver (légitime) |

Aucune télémétrie automatique au démarrage n'a été trouvée : **pas de vérification de version en ligne dans `PingCastle.exe`** (elle est déléguée à l'AutoUpdater).

## 7. Points d'entrée CLI et configuration

- `Program.Main` → `Run(args)` : pré-parse `--debug-license`, `--license`, `--out <fichier>` (redirection stdout) → validation licence → table `actions` (14 tâches : GenerateKey, Scanner, Export, Carto, ADHealthCheck, ADConso, HCRules, Regen, Reload, DemoReport, UploadAllReports, FakeReport, CloudHealthCheck, ExportRulesXml ; Bot commenté) → `ParseCommandLine` (80 `case`) → `RunInteractiveMode()` si aucun argument.
- Switches (`Program.cs`) : `--help --interactive --log --log-console --log-samba --api-endpoint --api-key --server --port --user --password --protocol --pagesize --quota --carto --healthcheck --explore-trust --explore-forest-trust --explore-exception --datefile --encrypt --level --no-enum-limit --reachable --sendXmlTo --sendHtmlTo --sendAllTo --notifyMail --smtplogin --smtppass --smtptls --skip-null-session --webdirectory --webuser --webpassword --max-depth --max-nodes --node --nodes --center-on --rules --generate-key --hc-conso --xmls --filter-date --regen-report --reload-report --scanner --scmode-* --nslimit --foreigndomain --export --azuread --tenantid --use-prt --clientid --thumbprint --private-key --p12-file --p12-pass --upload-all-reports --export-rules --license --debug-license --out --bot --demo-reports --generate-fake-reports --slim-report --doNotTestSMBv1 --I-swear-I-paid-win7-support --I-swear-I-paid-win8-support`.
- Menu interactif : healthcheck, azuread, conso, carto, scanner, export, advanced (protocol, hcrules, generatekey, noenumlimit, decrypt, regenerate, log).
- `app.config` (→ `PingCastle.exe.config`) : sections `LicenseSettings`, `encryptionSettings` (clé RSA « default » **avec clé privée en clair** : à régénérer/retirer en Phase 2, elle est publique dans le dépôt upstream), `honeyPotSettings`, `infrastructureSettings`, `mailSettings`, `appSettings` BrandLogo/BrandCss/BrandJs (commentés), `enforceFIPSPolicy=false`, `AppContextSwitchOverrides` TLS. Le `.config` livré avec le binaire ajoute `customRulesSettings` (`PingCastle.Rules.CustomRulesSettings`) : la source `app.config` du tag est **en retard** sur le binaire → aligner en Phase 3 (activation des règles personnalisées).
- `PingCastleAutoUpdater/App.config` : `supportedRuntime v4.0 / .NETFramework 4.8`.

## 8. État du build (sans modification)

Environnement : Windows 11, `dotnet` SDK 8.0.425 présent ; **absents** : MSBuild/Visual Studio Build Tools, targeting packs .NET Framework (`C:\Program Files (x86)\Reference Assemblies\...` vide), `gh`, ImageMagick, `nuget`.

| Tentative | Commande | Résultat |
|---|---|---|
| 1 (source intacte) | `dotnet build PingCastle.sln -c Release` | **ÉCHEC** : `MSB3644` assemblys de référence .NETFramework v4.5.2 et v4.8 introuvables (2 erreurs) |
| 2 (copie scratch, hors projet : + `Directory.Build.props` avec `PackageReference Microsoft.NETFramework.ReferenceAssemblies 1.0.3` + `Newtonsoft.Json.dll` copié depuis les binaires) | idem | `PingCastleAutoUpdater.exe` **compile** ; `PingCastle.csproj` **ÉCHEC** : `MSB3822/MSB3823` — les `.resx` contiennent des ressources non-chaînes (`System.Drawing.Icon/Color/Bitmap`) que le resgen du SDK .NET 8 ne compile qu'avec `GenerateResourceUsePreserializedResources=true` + référence `System.Resources.Extensions` (dépendance d'exécution supplémentaire à livrer) |

> **Résolution (Phase 1)** : voir ADR-0002 dans `docs/DECISIONS.md` — projet converti au format SDK (cible `net452` inchangée), build vert avec `dotnet build`, parité ressources/types vérifiée. Note : le binaire de référence lui-même refuse de démarrer depuis le 2024-07-31 (licence Basic expirée, ADR-0007).

Pré-requis identifiés à l'époque de l'audit (décision Phase 1, voir ADR-0002) :
- **Option A (recommandée, fidèle à l'original)** : installer Visual Studio Build Tools 2022 (charge « .NET desktop build tools ») + Developer Pack .NET Framework 4.5.2 (paquet hors support mais encore distribué) et 4.8 ; `msbuild PingCastle.sln /p:Configuration=Release`. Même chaîne dans GitHub Actions `windows-latest` (qui embarque MSBuild et les targeting packs 4.6.2+ ; 4.5.2 à installer ou à retargeter en 4.6.2 — à arbitrer).
- **Option B (sans installation admin)** : `dotnet build` + `Microsoft.NETFramework.ReferenceAssemblies` + `System.Resources.Extensions` + retrait des entrées `.resx` inutilisées (`Color1/Bitmap1/Icon1`) ; reste bloquant : `$this.Icon` de `AuthenticationDialog.resx`.
- Dans les deux cas : fournir `Newtonsoft.Json.dll` 13.0.1 (passer en `PackageReference Newtonsoft.Json 13.0.1`, décision Phase 1), neutraliser le post-build `signtool`, vérifier que `ProcessTemplate.ps1` s'exécute (il l'a fait sans erreur en tentative 2).

## 9. Métriques

| Métrique | Valeur |
|---|---|
| Fichiers `.cs` | 356 (350 compilés dans `PingCastle.csproj` + 2 AutoUpdater + 4 non référencés à vérifier en Phase 5) |
| Lignes de code `.cs` | **72 712** |
| Règles healthcheck | 170 (+ 6 cloud) ; calculs : 137 TriggerOnPresence, 41 TriggerOnThreshold, 22 PerDiscover, 2 TriggerIfLessThan, 1 PerDiscoverWithAMinimumOf |
| Scanners | 14 (+ ACL, LAPS/BitLocker, consistency, foreign users, replication) |
| `DllImport` | 134 |

Les 15 plus gros fichiers (lignes) — ordre de priorité des lots de commentaires Phase 5 :

| # | Fichier | Lignes |
|---|---|---|
| 1 | `ADWS/wse.cs` (code généré WSDL — commenter l'en-tête seulement) | 7 953 |
| 2 | `Healthcheck/HealthcheckAnalyzer.cs` | 5 552 |
| 3 | `Report/ReportHealthCheckSingle.cs` | 4 691 |
| 4 | `Report/ReportCloud.cs` | 2 011 |
| 5 | `Healthcheck/Healthcheck.cs` | 1 748 |
| 6 | `Cloud/RESTServices/Azure/ProvisioningApi.cs` | 1 743 |
| 7 | `Data/HealthcheckData.cs` | 1 682 |
| 8 | `Report/ReportBase.cs` | 1 509 |
| 9 | `Tasks.cs` | 1 424 |
| 10 | `Program.cs` | 1 278 |
| 11 | `Graph/Reporting/ReportGenerator.cs` | 1 225 |
| 12 | `Report/ReportHealthCheckConsolidation.cs` | 1 221 |
| 13 | `ADWS/ADItem.cs` | 1 187 |
| 14 | `Cloud/Analyzer/Analyzer.cs` | 994 |
| 15 | `Graph/Export/RelationFactory.cs` | 889 |

## 10. Points d'attention relevés (hors périmètre strict de l'audit)

1. `app.config` embarque une **clé privée RSA** « default » (chiffrement des rapports) identique pour tous les utilisateurs upstream : à régénérer ou laisser vide avec instruction `--generate-key` (Phase 2/3).
2. Le dossier de travail réel est `C:\Users\johnny\OneDrive\Bureau\Teumesse\Projets\TeumesseCastle` (le dossier courant de la session, `…\Teumesse\Site`, est le site web). Toutes les opérations ont été faites dans `Projets\TeumesseCastle`.
3. `PROMPT-claude-code-v1.md` (prompt de mission) est à la racine du projet : à déplacer dans `docs/` en Phase 1 (ou à exclure du dépôt, à ta convenance).
4. Les `.gz` de `template/` sont à la fois versionnés dans le tag et listés dans `template/.gitignore` : décider en Phase 1 si on les garde versionnés (build reproductible sans PowerShell) ou si on les régénère (pré-build).
5. Le binaire `PingCastle.exe.config` est plus récent que `app.config` (section `customRulesSettings`) : le code `Rules/CustomRulesSettings.cs` existe bien dans le tag.

## Annexe A — fichiers `.cs` sans en-tête de licence (60)

`ADWS/BlockingQueue.cs`, `ADWS/IADConnection.cs`, `ADWS/LinuxConnection.cs`, `ADWS/LinuxFileConnection.cs`, `ADWS/LinuxSidResolver.cs`, `ADWS/WindowsFileConnection.cs`, `ADWS/wse.cs`, `Bot/Bot.cs`, `Bot/BotData.cs`, `Bot/BotStream.cs`, `Cloud/Credentials/CertificateBuilder.cs`, `Cloud/Data/AzureADKey.cs`, `Cloud/UI/AuthenticationDialog.Designer.cs`, `ConsoleMenu.cs`, `Data/IPingCastleReportGenerator.cs`, `Data/PingCastleReportDataExportLevel.cs`, `Exports/ExportBase.cs`, `Exports/ExportChanges.cs`, `Exports/ExportComputers.cs`, `Exports/ExportUsers.cs`, `Exports/IExport.cs`, `Graph/Reporting/GraphObjectReference.cs`, `Healthcheck/FakeHealthCheckDataGenerator.cs`, `Healthcheck/Healthcheck.cs`, `Healthcheck/HoneyPotSettings.cs`, `Healthcheck/InfrastructureSettings.cs`, `Healthcheck/Rules/HeatlcheckRuleStaledSIDHistoryComputer.cs`, `PingCastleAutoUpdater/Program.cs`, `PingCastleAutoUpdater/Properties/AssemblyInfo.cs`, `PingCastleException.cs`, `PingCastleFactory.cs`, `Properties/AssemblyInfo.cs`, `RPC/SSPI.cs`, `RPC/dcom.cs`, `RPC/spool.cs`, `Report/IActionPlan.cs`, `Report/IPingCastleReportUser.cs`, `Report/ReportBenchmark.cs`, `Report/ReportHealthCheckRules.cs`, `Report/ReportHealthCheckSingleCompared.cs`, `Report/ReportHelper.cs`, `Report/ReportNetworkMap.cs`, `Report/ReportRiskControls.cs`, `Rules/CustomRulesSettings.cs`, `Rules/RiskModelObjective.cs`, `RuntimeSettings.cs`, `Scanners/ACLScanner.cs`, `Scanners/AntivirusScanner.cs`, `Scanners/LAPSBitLocker.cs`, `Scanners/Smb1Protocol.cs`, `Scanners/Smb2Protocol.cs`, `Scanners/ZeroLogonScanner.cs`, `misc/ConnectionTester.cs`, `misc/ConnectionTesterHttp.cs`, `misc/ConnectionTesterLdap.cs`, `misc/JSONParser.cs` (TinyJson, auteur tiers — vérifier sa licence), `misc/NamedPipeTester.cs`, `misc/ROCAVulnerabilityTester.cs`, `misc/RestrictedToken.cs`, `misc/Subnet.cs`.

Pour ces fichiers, la Phase 1 ajoutera un en-tête complet (copyright upstream + NPOSL + ligne Teumesse) puisque le projet entier est couvert par `LICENSE.md` ; `misc/JSONParser.cs` et `ADWS/wse.cs` (généré) sont à traiter à part.
