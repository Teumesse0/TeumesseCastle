# Prompt Claude Code — TeumesseCastle v1.0.0 (fork open source de PingCastle 3.0.0.0)

> **Mode d'emploi** : ouvrir un terminal dans `C:\Users\johnny\OneDrive\Bureau\Teumesse\Projets\TeumesseCastle`, lancer `claude`, puis coller intégralement le bloc ci-dessous (de `=== DÉBUT` à `=== FIN`).
> Prérequis sur le poste : Git, `gh` authentifié (`gh auth login`), Visual Studio 2022 Build Tools (ou VS) avec charge de travail ".NET desktop" + .NET Framework 4.7.2 Developer Pack, ImageMagick (`magick`) pour le logo.

---

=== DÉBUT DU PROMPT ===

# Mission

Tu es l'ingénieur principal du projet **TeumesseCastle**, fork open source et communautaire de **PingCastle 3.0.0.0** (dernière version distribuée sous licence libre en 2023, avant le passage en modèle payant). Le code source de PingCastle 3.0.0.0 se trouve dans le dossier courant. Ton objectif : produire la **version 1.0.0 de TeumesseCastle**, fonctionnellement identique à PingCastle 3.0.0.0, entièrement rebrandée, entièrement commentée, libérée de tout mécanisme de licence/édition payante, versionnée dans Git et publiée sur `https://github.com/Teumesse0/TeumesseCastle`.

TeumesseCastle est édité par **Teumesse** (société cyber offensive de Johnny Cervantes, alias adios25). Il doit rester **open source pour toujours** : c'est la raison d'être du projet.

# Paramètres (modifiables par moi, à respecter tels quels sinon)

- `PROJECT_NAME` = TeumesseCastle
- `ORG` = Teumesse
- `GITHUB_REPO` = https://github.com/Teumesse0/TeumesseCastle
- `VERSION_V1` = 1.0.0
- `LOGO_SOURCE` = `C:\Users\johnny\OneDrive\Bureau\Teumesse\Images\logoblanc.jpg`
- `LANGUE_COMMENTAIRES_CODE` = anglais (projet communautaire international)
- `LANGUE_DOCS` = français ET anglais (README.md en anglais, README.fr.md en français ; même chose pour CONTRIBUTING)
- `LANGUE_ECHANGES_AVEC_MOI` = français
- `CIBLE_BUILD_V1` = conserver la cible .NET Framework existante (ne PAS migrer vers .NET 8 en v1 ; la migration est une tâche v2 à documenter dans la roadmap)

# Règles absolues

1. **Zéro régression fonctionnelle** : la v1 doit produire exactement les mêmes rapports (healthcheck, scanners, graph, export) que PingCastle 3.0.0.0 sur un même AD. Tu n'inventes aucune règle, aucun score, aucun scanner. Tu ne « simplifies » pas de logique métier.
2. **Rien n'est supposé** : avant toute modification, tu lis le code. Tu ne commentes jamais une méthode dont tu n'as pas lu le corps. Si une portion est obscure, tu écris un commentaire `// TODO(review): ...` plutôt qu'une explication inventée.
3. **Le build passe à chaque commit**. Tu n'enchaînes jamais deux étapes si le build (`msbuild` ou `dotnet build`) est cassé.
4. **Pas de télémétrie, pas d'appel réseau sortant non documenté**. Tout appel vers `pingcastle.com`, `*.netwrix.com` ou équivalent est supprimé ou remplacé (voir Phase 4).
5. **Respect du droit** : voir Phase 1. On ne supprime jamais un copyright, on ne change jamais la licence d'origine.
6. **Travail incrémental et traçable** : une phase = une branche de travail = plusieurs commits atomiques (Conventional Commits) = une PR mergée dans `main` (ou un merge local propre si je te le demande). Tu me montres le plan de chaque phase avant de l'exécuter et tu attends ma validation pour les phases 1, 3 et 4.
7. **Toute opération destructive** (suppression de fichiers en masse, `git push --force`, réécriture d'historique, suppression de branche) requiert ma confirmation explicite.
8. Tu utilises la liste de tâches (TodoWrite) en permanence pour que je suive l'avancement, et tu mets à jour `docs/PROGRESS.md` à la fin de chaque phase.

# Phase 0 — Récupération du code source et audit (aucune modification de code)

**État réel du dossier courant** : il contient la **distribution binaire** de PingCastle 3.0.0.0 (`PingCastle.exe`, `PingCastle.pdb`, `PingCastleAutoUpdater.exe`, `Newtonsoft.Json.dll`, `license.rtf`, `changelog.txt`, deux PDF, `PingCastle_3.0.0.0.zip`), **pas le code source**. Tu ne décompiles jamais ces binaires : le code source officiel est public.

0-A. **Réorganisation** : déplace tous ces fichiers dans `reference/pingcastle-3.0.0.0-binaries/` (ils serviront de référence de non-régression : on comparera les rapports générés par l'original et par TeumesseCastle). Ajoute ce dossier au `.gitignore` sauf `changelog.txt` et `license.rtf` (utile pour la traçabilité de la licence d'origine).

0-B. **Récupération de la source** : clone le dépôt officiel dans un dossier temporaire hors du projet (`git clone https://github.com/netwrix/pingcastle.git ../_pingcastle-upstream`), liste les tags (`git tag -l '3.0.*'`), et extrais (`git archive`) le tag **`3.0.0.0`** s'il existe, sinon le tag 3.0.0.x le plus proche de la version binaire présente (le tag `3.0.0.4` est confirmé existant). Consigne le tag retenu, son hash de commit et la licence trouvée dans le tag (`LICENSE` à la racine — attendu NPOSL-3.0). **Vérifie impérativement que la licence du tag retenu est bien NPOSL-3.0** : les versions récentes du dépôt upstream sont passées sous licence propriétaire, et aucun code issu d'un commit postérieur au changement de licence ne doit entrer dans TeumesseCastle. Copie la source extraite à la racine du projet. Si aucun tag 3.0.x n'est accessible, arrête-toi et demande-moi.

0-C. **Audit** — produis `docs/audit/00-inventaire-source.md` contenant :

1. Arborescence complète de la source avec rôle de chaque dossier (`ADWS/`, `Healthcheck/`, `Rules/`, `Report/`, `Scanners/`, `Graph/`, `Exports/`, `Cloud/`, `misc/`, `Bot/`, `NativeMethods/`, etc. — adapte à ce que tu trouves réellement).
2. Fichier `.sln`, liste des `.csproj`, **TargetFramework exact**, dépendances NuGet/packages, références COM/P-Invoke, ressources embarquées (`.resx`, images, templates HTML/JS/CSS).
3. **Licence réelle** trouvée dans le dépôt (fichier `LICENSE`, en-têtes de fichiers, `AssemblyInfo.cs`). Attendu : *Non-Profit Open Software License 3.0 (NPOSL-3.0)*, auteur Vincent Le Toux. Confirme ou corrige.
4. Inventaire exhaustif de toutes les occurrences à rebrander : `grep -ri` sur `PingCastle`, `pingcastle`, `PINGCASTLE`, `Ping Castle`, `pingcastle.com`, `Netwrix`, `Vincent Le Toux`, `vincent.letoux`, `@pingcastle`. Classe-les en : (a) namespace/identifiant de code, (b) chaîne affichée à l'utilisateur, (c) nom de fichier de sortie (ex. `ad_hc_*.html`), (d) URL, (e) mention légale/copyright **à conserver**.
5. Inventaire du **mécanisme de licence / éditions** : classes type `LicenseManager`, `License`, fichiers `.lic`, vérifications `IsBasic`/`IsPro`/`IsEnterprise`/`IsAuditor`, fonctionnalités masquées derrière une édition, clé publique de vérification de signature de licence, options CLI `--license`. Liste chaque point de contrôle avec fichier:ligne.
6. Inventaire des **appels réseau sortants** (vérification de version, téléchargement de mise à jour, envoi de rapports, `HttpWebRequest`, `WebClient`, URLs en dur).
7. Inventaire des points d'entrée CLI (`Program.cs`, parsing des arguments, mode interactif) et des fichiers de configuration (`PingCastle.exe.config`, `PingCastleAutoUpdater`, etc.).
8. État du build : tente une compilation **sans rien modifier** et consigne le résultat (erreurs SDK manquants, packages NuGet à restaurer, etc.).
9. Estimation du nombre de fichiers `.cs`, de lignes de code, et des 15 fichiers les plus gros (à commenter en priorité par lots).

Arrête-toi et présente-moi cet audit avant la Phase 1.

# Phase 1 — Cadre légal et fondations du dépôt

Objectif : un dépôt Git propre, juridiquement irréprochable, avant tout rebranding.

1. `git init` (si absent), branche par défaut `main`, `.gitignore` adapté Visual Studio/.NET (`bin/`, `obj/`, `.vs/`, `*.user`, `*.suo`, `packages/`, `TestResults/`, rapports générés `*.html`/`*.xml`/`*.json` à la racine, `*.lic`).
2. **Licence** : conserve le fichier `LICENSE` d'origine (NPOSL-3.0) tel quel. Le fork reste sous **NPOSL-3.0** (licence copyleft : les dérivés doivent rester sous la même licence — c'est exactement ce qu'on veut pour garantir le caractère open source). N'ajoute PAS de double licence MIT/Apache sans mon accord écrit.
3. Crée `NOTICE.md` : « TeumesseCastle is a fork of PingCastle 3.0.0.0, Copyright (c) Vincent Le Toux, licensed under NPOSL-3.0. PingCastle is a trademark of its owner; TeumesseCastle is not affiliated with or endorsed by PingCastle/Netwrix. Modifications Copyright (c) 2026 Teumesse. » + lien vers le dépôt d'origine.
4. En-têtes de fichiers : conserve l'en-tête de copyright original dans chaque `.cs` qui en possède un, et ajoute dessous une ligne `// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project`.
5. Crée `CHANGELOG.md` (format Keep a Changelog, section `[Unreleased]`), `CODE_OF_CONDUCT.md` (Contributor Covenant 2.1), `SECURITY.md` (divulgation responsable : contact à définir avec moi, délai 90 jours, pas de scan sur AD tiers sans autorisation), `CONTRIBUTING.md` + `CONTRIBUTING.fr.md` (workflow branche/PR, Conventional Commits, obligation de commenter, build obligatoire).
6. Crée `CLAUDE.md` à la racine : résumé du projet, commandes de build/test, conventions (commentaires XML en anglais, Conventional Commits, SemVer), règles absolues ci-dessus, et « ne jamais réintroduire de vérification de licence ni d'appel réseau sortant ». Ce fichier sert à toutes les futures sessions Claude Code.
7. Premier commit : `chore: import PingCastle <tag> source as baseline (NPOSL-3.0, upstream commit <hash>)` avec la source **intacte** (le message cite le dépôt upstream, le tag et le hash), puis `chore: add license notice, contributing guides and repo scaffolding`. Ainsi l'historique Git prouve la provenance.
8. Création du remote : `gh repo view Teumesse0/TeumesseCastle` ; si le dépôt existe et est vide, `git remote add origin ...` et `git push -u origin main`. S'il n'est pas vide, arrête-toi et demande-moi.

# Phase 2 — Rebranding complet

1. **Renommage code** : namespace `PingCastle` → `TeumesseCastle` (et tous les sous-namespaces), nom d'assembly, `RootNamespace`, nom du `.csproj`, du `.sln`, de l'exécutable (`TeumesseCastle.exe`), `AssemblyInfo.cs` (Title, Product, Company = Teumesse, Copyright double, Version = `1.0.0.0`, `AssemblyInformationalVersion = 1.0.0`), GUID d'assembly régénéré, `app.config` renommé.
2. **Renommage des sorties** : préfixes de fichiers générés (`ad_hc_` → à conserver OU renommer `tc_hc_` — propose-moi les deux options avec impact sur la compatibilité des outils tiers qui parsent ces rapports, j'arbitre), titres HTML, `<meta generator>`, textes « Generated by PingCastle » → « Generated by TeumesseCastle ».
3. **Chaînes utilisateur** : bannière CLI, aide `--help`, messages console, textes des rapports, pied de page. Conserve dans le pied de page du rapport une mention : « Based on PingCastle 3.0.0.0 by Vincent Le Toux (NPOSL-3.0) ».
4. **Logo** :
   - copie `LOGO_SOURCE` dans `assets/branding/logo-white.jpg` ;
   - génère avec ImageMagick : `assets/branding/logo-white.png` (fond transparent si le JPG a un fond uni — sinon garde tel quel), `logo-256.png`, `logo-64.png`, `TeumesseCastle.ico` (multi-tailles 16/32/48/256) ;
   - remplace le logo PingCastle embarqué dans les rapports HTML (base64 dans le template ou ressource embarquée) et l'icône de l'exécutable (`<ApplicationIcon>`) ;
   - documente dans `assets/branding/README.md` les commandes utilisées pour régénérer.
5. **URLs** : toute URL `pingcastle.com` devient soit le lien GitHub du projet, soit est supprimée. Les liens vers les références externes de sécurité (MITRE, ANSSI, Microsoft Docs, CVE) sont **conservés** : ce sont des sources, pas du branding.
6. Vérification finale : `grep -ri pingcastle` ne doit plus renvoyer que `NOTICE.md`, `LICENSE`, `CHANGELOG.md`, les en-têtes de copyright et le pied de page de rapport. Joins la sortie du grep au résumé de phase.
7. Build vert, exécution de `TeumesseCastle.exe --help` et vérification que la bannière est correcte. Commits : `refactor: rename namespaces and assembly to TeumesseCastle`, `feat(branding): add Teumesse logo and icon`, `docs: update user-facing strings`.

# Phase 3 — Libération : suppression du modèle de licence et des éditions (validation requise)

1. À partir de l'inventaire Phase 0 §5, propose un plan précis : pour chaque point de contrôle d'édition, indique la branche de code à conserver (toujours la branche « fonctionnalité activée »). Attends ma validation.
2. Supprime les classes de gestion de licence, les fichiers `.lic`, la clé publique de vérification, l'option CLI `--license`, les messages « this feature requires the Pro/Enterprise edition », et toute logique d'expiration/trial. **Toutes les fonctionnalités deviennent disponibles sans condition.**
3. Si une fonctionnalité « Enterprise » dépend d'un service externe payant inexistant dans la source (ex. console centrale), ne tente pas de la réécrire : retire proprement l'option et documente-la dans `docs/ROADMAP.md` comme « non portée en v1 ».
4. Tests manuels : exécuter healthcheck en mode `--no-enum` ou sur un domaine de lab si disponible ; sinon, au minimum, exécuter chaque sous-commande avec `--help` et vérifier qu'aucune n'échoue sur une vérification de licence résiduelle.
5. Commit : `feat!: remove license verification — all features are free and open source` (BREAKING CHANGE documenté dans CHANGELOG).

# Phase 4 — Suppression des appels sortants et mise à jour via GitHub (validation requise)

1. Supprime la vérification de version et l'auto-updater pointant vers l'éditeur d'origine.
2. Implémente un remplacement **opt-in uniquement** (`--check-update`, jamais automatique) qui interroge `https://api.github.com/repos/Teumesse0/TeumesseCastle/releases/latest`, compare le tag SemVer, affiche un message et **ne télécharge rien**. Timeout 5 s, erreur réseau silencieuse en mode non interactif.
3. Vérifie avec un grep sur `http://`, `https://`, `HttpWebRequest`, `WebClient`, `HttpClient` qu'il ne reste que : l'appel opt-in ci-dessus, les connexions LDAP/ADWS/SMB/RPC/Kerberos légitimes vers l'AD audité, et les URLs de documentation dans les règles.
4. Documente le comportement réseau dans `docs/NETWORK.md` (indispensable pour un outil déployé sur des DC).
5. Commit : `feat: replace vendor update check with opt-in GitHub releases check`.

# Phase 5 — Commentaires exhaustifs (par lots, build vert à chaque lot)

1. Standard : commentaires XML `///` (`<summary>`, `<param>`, `<returns>`, `<remarks>`, `<exception>`) en **anglais** sur **toutes** les classes, interfaces, enums, méthodes publiques et internes, propriétés non triviales, et constantes magiques. Commentaires `//` inline pour tout bloc algorithmique non évident (parsing LDAP, calcul de scores, détection de chemins d'attaque dans le graph, décodage de structures natives, P/Invoke).
2. Chaque fichier commence par un bloc d'en-tête : copyright (voir Phase 1), rôle du fichier en 2-4 lignes, et liens vers la documentation Microsoft/ANSSI quand le fichier implémente une spécification (ex. `[MS-ADTS]`, `[MS-DRSR]`, `[MS-SAMR]`).
3. Pour `Rules/` : chaque règle reçoit un `<summary>` expliquant **ce qui est détecté, pourquoi c'est un risque, et comment remédier**, en cohérence avec le texte déjà présent dans les attributs de la règle (ne réécris pas le texte métier des règles — c'est le référentiel, tu le documentes, tu ne le modifies pas).
4. Ordre de traitement : `Program.cs` et le parsing CLI → `ADWS/` (connecteurs) → `Healthcheck/` (collecte) → `Rules/` → `Report/` → `Scanners/` → `Graph/` → `Exports/` → reste. Un commit par dossier : `docs(code): document <dossier> with XML comments`.
5. Active `<GenerateDocumentationFile>true</GenerateDocumentationFile>` et traite les avertissements CS1591 (membre public sans commentaire) comme **erreurs** dans la configuration Release pour garantir que la couverture reste à 100 % à l'avenir.
6. À la fin, génère `docs/ARCHITECTURE.md` : schéma (Mermaid) du flux complet « connexion AD → collecte → application des règles → scoring → génération du rapport », description de chaque module, et `docs/RULES.md` listant automatiquement toutes les règles (ID, catégorie, modèle de maturité, points) via un petit script PowerShell `tools/Generate-RulesDoc.ps1` qui lit le code.

# Phase 6 — Qualité, tests, CI/CD et releases

1. **Projet de tests** `TeumesseCastle.Tests` (xUnit, compatible avec la cible .NET Framework retenue) : tests sur le calcul de scores à partir de données healthcheck sérialisées (fixtures XML/JSON anonymisées que tu fabriques), sur la génération du rapport HTML (présence des sections, absence de `pingcastle` dans la sortie), sur le parseur CLI, et sur le comparateur SemVer de la Phase 4. Objectif réaliste v1 : tests de non-régression sur les parties pures ; aucune dépendance à un AD réel.
2. **GitHub Actions** :
   - `.github/workflows/build.yml` : sur push/PR, `windows-latest`, restauration NuGet, `msbuild /p:Configuration=Release`, exécution des tests, upload de l'artefact `TeumesseCastle-<sha>.zip`.
   - `.github/workflows/release.yml` : déclenché par tag `v*.*.*`, build Release, zip (`TeumesseCastle.exe`, `.exe.config`, `LICENSE`, `NOTICE.md`, `README.md`, `CHANGELOG.md`), calcul `SHA256SUMS.txt`, création de la GitHub Release avec notes extraites du CHANGELOG.
   - `.github/dependabot.yml` pour les packages NuGet et les actions.
3. Templates `.github/ISSUE_TEMPLATE/` (bug, feature, nouvelle règle de sécurité) et `PULL_REQUEST_TEMPLATE.md` (checklist : build vert, commentaires ajoutés, CHANGELOG mis à jour, aucune donnée réelle d'AD dans la PR).
4. **Versionnage** : SemVer. `1.0.0` = parité PingCastle 3.0.0.0 rebrandée et libérée. Version centralisée dans un seul fichier (`Directory.Build.props` → `<Version>`), affichée par `--version`. Documente la procédure de release dans `docs/RELEASING.md` (bump version → CHANGELOG → commit `chore(release): v1.0.0` → `git tag -a v1.0.0` → push tag → CI publie).
5. **Synchronisation poste ↔ GitHub** : le dossier local `C:\Users\johnny\OneDrive\Bureau\Teumesse\Projets\TeumesseCastle` **est** le clone de travail. Ajoute dans `docs/RELEASING.md` un avertissement : OneDrive et Git cohabitent mal (verrous sur `.git/`, conflits) — recommandation : exclure le dossier `.git` de la synchronisation OneDrive ou déplacer le clone hors OneDrive et ne garder dans OneDrive que les releases. Propose-moi la solution au moment venu.
6. **README.md / README.fr.md** : bannière avec le logo, pitch (« Active Directory security audit, free and open source forever »), tableau des fonctionnalités, installation (téléchargement release / build depuis les sources), usage (exemples de commandes), captures du rapport (à générer sur un lab, placeholders si indisponible), roadmap, licence, crédits à PingCastle et Vincent Le Toux, lien Teumesse.
7. **ROADMAP.md** : v1.1 (corrections, règles communautaires), v2.0 (migration .NET 8 multi-cible, modernisation du rapport HTML, i18n FR/EN des rapports, règles alignées sur les guides ANSSI/CSN), idées communauté (export JSON normalisé, intégration SIEM, mode CI).

# Phase 7 — Release v1.0.0

1. Revue finale : `grep -ri pingcastle` propre, build Release vert, tests verts, CI verte, `--help`/`--version` corrects, rapport HTML généré localement (lab ou mode démo) ouvert et vérifié visuellement (logo, textes, pied de page).
2. Mise à jour `CHANGELOG.md` → `[1.0.0] - <date>`, bump version, commit `chore(release): v1.0.0`, tag annoté `v1.0.0`, push.
3. Vérifie que la GitHub Release est publiée avec le zip et les checksums. Renseigne la description du dépôt GitHub, les topics (`active-directory`, `security-audit`, `pingcastle`, `open-source`, `csharp`, `pentest`, `blue-team`) et active Issues + Discussions via `gh`.
4. Rédige `docs/PROGRESS.md` final et un résumé pour moi : ce qui a été fait, ce qui reste, les décisions à prendre.

# Format de tes réponses

- En français, dense, orienté action. Pas de prose explicative inutile.
- Au début de chaque phase : plan sous forme de liste de tâches. À la fin : résumé 5-10 lignes + commandes exécutées + fichiers modifiés + sortie du build.
- Quand tu as une question, pose-la avec les options chiffrées et ta recommandation en premier. N'attends pas une réponse pour les points non bloquants : prends la décision la plus conservatrice (celle qui préserve le comportement d'origine) et consigne-la dans `docs/DECISIONS.md` (format ADR court).

Commence maintenant par la Phase 0.

=== FIN DU PROMPT ===

---

## Notes pour Johnny (hors prompt)

- **Ton dossier contient les binaires, pas la source.** Le prompt gère ça : Claude Code récupère le code officiel depuis GitHub (tag 3.0.0.x, licence NPOSL-3.0 vérifiée) et garde tes binaires comme référence pour comparer les rapports. Ne jamais décompiler l'exe — inutile et juridiquement fragile.
- **Licence** : PingCastle ≤ 3.2 est sous NPOSL-3.0 (copyleft, OSI-approved). Le fork est légal à condition de garder la licence, les copyrights, et de ne pas utiliser la marque « PingCastle » comme nom du produit — d'où le rebranding complet. Les versions ≥ 3.3 (Netwrix) sont propriétaires : ne jamais y piocher du code.
- **Décision à prendre dès la Phase 2** : garder les préfixes de fichiers `ad_hc_*` (compatibilité avec les scripts/outils qui consomment déjà des rapports PingCastle) ou passer à `tc_hc_*` (identité propre). Ma recommandation : garder `ad_hc_*` en v1, option `--prefix` en v1.1.
- **Langue des commentaires** : j'ai mis anglais pour le code (standard open source, plus de contributeurs), FR+EN pour la doc. Change `LANGUE_COMMENTAIRES_CODE` si tu veux tout en français.
- **OneDrive + Git** : vrai point de douleur. Idéal : clone dans `C:\Dev\TeumesseCastle` hors OneDrive, et OneDrive ne garde que `Projets\TeumesseCastle\releases\`. Le prompt fait proposer la solution par Claude Code au bon moment.
- **Sessions suivantes** : grâce au `CLAUDE.md` créé en Phase 1, tu pourras relancer `claude` dans le dossier et dire simplement « continue la Phase N » ou « ajoute la règle X » sans recoller ce prompt.
