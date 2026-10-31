# Journal des décisions (ADR courts)

Format : contexte → décision → conséquences. Une entrée par décision non bloquante prise sans validation explicite, ou par arbitrage validé.

## ADR-0001 — Tag upstream retenu : `3.0.0.0`

- **Date** : 2026-10-31 (Phase 0)
- **Contexte** : les binaires de référence sont en version 3.0.0.0 (FileVersion, changelog identique). Les tags `3.0.0.3` et `3.0.0.4` existent aussi (correctifs, 33 fichiers de diff). Le tag `3.0.0.0_.net_4.5.2` pointe sur un commit 3.3.0.0 de 2024 et est trompeur.
- **Décision** : importer exactement le tag `3.0.0.0` (commit `d8f21e920278b82f243bdfe84d168d8040a0b96c`) pour garantir la parité stricte avec les binaires de référence utilisés pour la non-régression.
- **Conséquences** : les correctifs 3.0.0.3/3.0.0.4 (toujours NPOSL-3.0) sont candidats à une v1.1 après revue ; ils ne sont pas inclus en v1.0.0.

## ADR-0002 — Chaîne de build (à arbitrer en Phase 1)

- **Date** : 2026-10-31 (Phase 0)
- **Contexte** : aucun MSBuild/VS Build Tools ni targeting pack .NET Framework sur le poste ; `dotnet build` (SDK 8) échoue (MSB3644), et même avec `Microsoft.NETFramework.ReferenceAssemblies` il bute sur les ressources non-chaînes des `.resx` (MSB3822/3823).
- **Options** : (A) installer VS Build Tools 2022 + Developer Packs 4.5.2/4.8 et bâtir avec `msbuild` (fidèle à l'original, même chaîne que `windows-latest` en CI) ; (B) rester sur `dotnet build` avec `System.Resources.Extensions` (ajoute une DLL d'exécution, s'écarte de l'original).
- **Recommandation initiale** : A.
- **Résolution (Phase 1, 2026-10-31, feu vert global de Johnny)** : le poste n'a pas de droits administrateur, l'installation de VS Build Tools est donc impossible sans intervention manuelle. Option retenue : **conversion de `PingCastle.csproj` au format SDK (`Microsoft.NET.Sdk`) en conservant la cible `net452`**. Le SDK restaure implicitement `Microsoft.NETFramework.ReferenceAssemblies`, résout `Newtonsoft.Json` 13.0.1 en `PackageReference`, et compile les `.resx` une fois les entrées non-chaînes inutilisées retirées (`Color1`, `Bitmap1`, `Icon1` ; l'icône du formulaire Azure AD est désormais prise sur l'exécutable). Le projet `PingCastleAutoUpdater` reste au format legacy (il disparaît en Phase 4) et reçoit le package via `Directory.Build.props`.
- **Vérification de parité** : l'exécutable produit expose exactement les mêmes 26 ressources manifest et les mêmes 947 types utilisateur que `PingCastle.exe` 3.0.0.0 de référence (seuls les types générés par le compilateur — closures, tableaux statiques — diffèrent de nom). Les 4 fichiers `.cs` non compilés par l'upstream (`ADWS/BlockingQueue.cs`, `Healthcheck/Healthcheck.cs`, `Healthcheck/Rules/HeatlcheckRuleStaledSIDHistoryComputer.cs`, `Rules/RiskModelObjective.cs`) sont exclus explicitement ; les 4 `.gz` non embarqués par l'upstream aussi.
- **Conséquences** : `dotnet build PingCastle.sln -c Release` fonctionne sur tout poste Windows avec le SDK .NET 8 ; même commande en CI. Le post-build `signtool` lié au poste de l'auteur upstream est supprimé.

## ADR-0003 — Portée de la NPOSL-3.0 pour le fork

- **Date** : 2026-10-31 (Phase 0)
- **Contexte** : la NPOSL-3.0 (section 17) impose que le licencié qui redistribue soit une organisation sans but lucratif ou déclare ne pas tirer de bénéfice commercial de la distribution ; sinon la distribution se fait sous OSL-3.0. Teumesse est une société commerciale.
- **Décision provisoire** : conserver le fichier `LICENSE.md` d'origine tel quel (obligation), ne pas ajouter de double licence, et **signaler à Johnny** que la question « distribution sous NPOSL-3.0 vs OSL-3.0 (section 17-d) » doit être tranchée avec un conseil juridique avant la release publique. Ce n'est pas bloquant pour les phases 1 à 6.
- **Conséquences** : `NOTICE.md` mentionnera la NPOSL-3.0 ; une note sera ajoutée dans `docs/PROGRESS.md` « décisions à prendre ».

## ADR-0004 — Dossier de travail

- **Date** : 2026-10-31 (Phase 0)
- **Contexte** : la session Claude Code a été lancée dans `…\Teumesse\Site` (site web), alors que les binaires PingCastle et le prompt sont dans `…\Teumesse\Projets\TeumesseCastle`.
- **Décision** : tout le travail TeumesseCastle est réalisé dans `C:\Users\johnny\OneDrive\Bureau\Teumesse\Projets\TeumesseCastle`. Le clone upstream temporaire est dans `…\Projets\_pingcastle-upstream`.
- **Conséquences** : relancer les sessions suivantes directement dans `Projets\TeumesseCastle` (le `CLAUDE.md` de Phase 1 y sera créé).

## ADR-0005 — Préfixes des fichiers de sortie : `ad_hc_*` conservés

- **Date** : 2026-10-31 (Phase 1, décision anticipée pour la Phase 2)
- **Contexte** : les rapports s'appellent `ad_hc_<domaine>.html/.xml`, `ad_hc_summary.html`, `ad_carto_*.html`, `ad_scanner_*`, `ad_export_*`. De nombreux scripts et outils tiers (consolidation, SIEM, PowerShell) consomment ces noms. Aucun préfixe ne contient la marque PingCastle.
- **Décision** : conserver les préfixes historiques en v1.0.0 (option la plus conservatrice, recommandée par Johnny dans ses notes). Une option `--prefix` est inscrite à la roadmap v1.1.
- **Conséquences** : compatibilité totale avec l'outillage existant ; la consolidation (`fi.Name.StartsWith("ad_hc_")`) reste inchangée.
- **Exception** : le préfixe du rapport Azure AD `pingcastlecloud_<tenant>.html/.json.gz` contient la marque ; il devient `teumessecastlecloud_<tenant>` (même structure). De même, les valeurs d'exemple utilisées par `--generate-key` Azure AD (`tenant = "pingcastle.com"`, nom `vletoux`) deviennent `teumessecastle.local` / `teumessecastle` : elles ne servent qu'au sujet du certificat généré.

## ADR-0006 — Outils : GitHub CLI et génération des images

- **Date** : 2026-10-31 (Phase 1)
- **Contexte** : `gh` et ImageMagick étaient absents ; pas de droits administrateur.
- **Décision** : `gh` installé en portée utilisateur via `winget install GitHub.cli --scope user` ; authentification par le jeton GitHub déjà stocké dans le Git Credential Manager du poste (compte `Teumesse0`, scopes `repo`, `workflow`, `gist`) exposé via `GH_TOKEN` le temps de la session. ImageMagick n'est pas installé : les PNG/ICO de la Phase 2 seront produits avec un script PowerShell (`System.Drawing`) versionné dans `assets/branding/`, documenté dans `assets/branding/README.md`.
- **Conséquences** : aucune installation système ; les commandes de régénération des logos restent reproductibles sur tout poste Windows.

## ADR-0007 — Référence de non-régression : le binaire 3.0.0.0 ne s'exécute plus

- **Date** : 2026-10-31 (Phase 1)
- **Contexte** : `reference/pingcastle-3.0.0.0-binaries/PingCastle.exe --help` (et le build de la source intacte) s'arrête immédiatement sur « The program is unsupported since: 2024-07-31 » : la licence « Basic » embarquée a expiré. Impossible de générer des rapports de référence avec l'original aujourd'hui.
- **Décision** : la référence fonctionnelle devient le **commit de baseline** (`chore: import PingCastle 3.0.0.0 source as baseline`) ; pour produire des rapports de comparaison, on compilera ponctuellement ce commit avec la seule neutralisation du test d'expiration (hors dépôt, dossier `reference/`). La parité est de plus vérifiée statiquement : mêmes ressources manifest et mêmes types utilisateur que le binaire d'origine (voir ADR-0002).
- **Conséquences** : la Phase 3 (suppression de la licence) est indispensable pour que l'outil fonctionne ; la Phase 6 inclut une fixture XML anonymisée pour comparer les rapports HTML régénérés (`--regen-report`) avant/après.

## ADR-0008 — Identité Git et cohabitation OneDrive

- **Date** : 2026-10-31 (Phase 1)
- **Contexte** : aucune identité Git globale sur le poste ; le dépôt est dans un dossier synchronisé OneDrive. Un verrou OneDrive a déjà fait échouer une écriture de fichier pendant la Phase 1 (contourné par une relance).
- **Décision** : identité configurée **localement** dans le dépôt (`user.name = Johnny Cervantes`, `user.email = johnny.l.s.cervantes@gmail.com`, `core.autocrlf = false` pour préserver les CRLF de l'upstream). Le dépôt reste pour l'instant dans OneDrive ; la recommandation détaillée (déplacer le clone hors OneDrive, ne synchroniser que `releases/`) est rédigée dans `docs/RELEASING.md` en Phase 6 pour arbitrage.
- **Conséquences** : les scripts de modification en masse intègrent une logique de relance sur verrou de fichier.
