# Avancement TeumesseCastle v1.0.0

Dernière mise à jour : 2026-10-31

| Phase | Statut | Branche / commits | Notes |
|---|---|---|---|
| 0 — Récupération source et audit | **Terminée** | `main` : commit baseline | Audit : `docs/audit/00-inventaire-source.md` |
| 1 — Cadre légal et fondations | **Terminée** | `main` : build/chore commits, push initial | ADR-0002 résolu (SDK-style, net452) |
| 2 — Rebranding | En cours | | `ad_hc_` conservé (ADR-0005) |
| 3 — Libération (licence/éditions) | À faire (validation requise) | | 23 points de contrôle inventoriés (§5 audit) |
| 4 — Réseau et mise à jour GitHub | À faire (validation requise) | | 3 appels sortants éditeur à supprimer (§6 audit) |
| 5 — Commentaires exhaustifs | À faire | | 72 712 LOC, 356 fichiers |
| 6 — Qualité, tests, CI/CD | À faire | | |
| 7 — Release v1.0.0 | À faire | | |

## Phase 0 — réalisé

- Binaires PingCastle 3.0.0.0 déplacés dans `reference/pingcastle-3.0.0.0-binaries/` ; `.gitignore` mis à jour (exclusion du dossier sauf `changelog.txt` et `license.rtf`).
- Clone upstream dans `..\_pingcastle-upstream`, tag `3.0.0.0` (commit `d8f21e92…`, 2023-02-21) extrait à la racine du projet. Licence `LICENSE.md` = NPOSL-3.0 confirmée.
- Audit complet rédigé ; journal de décisions initialisé (`docs/DECISIONS.md`, ADR-0001 à 0004).
- Tentatives de build consignées : échec sans targeting packs ; chaîne de build à choisir en Phase 1.

## Phase 1 — réalisé (feu vert global de Johnny le 2026-10-31)

- Dépôt Git initialisé (`main`), identité locale, `core.autocrlf=false`. Commit 1 : import verbatim du tag `3.0.0.0`.
- Chaîne de build : `PingCastle.csproj` converti au format SDK, cible `net452` inchangée ; `Directory.Build.props` ; Newtonsoft.Json en PackageReference ; entrées `.resx` non-chaînes inutilisées retirées ; post-build `signtool` supprimé. `dotnet build PingCastle.sln -c Release` : 0 erreur, 0 avertissement. Parité vérifiée avec le binaire d'origine : 26 ressources, 947 types utilisateur identiques.
- En-têtes : ligne `// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.` ajoutée sous l'en-tête d'origine des 296 fichiers qui en avaient un ; en-tête neutre (origine PingCastle 3.0.0.0 + NPOSL) ajouté aux 60 fichiers qui n'en avaient pas (`misc/JSONParser.cs` et `ADWS/wse.cs` traités à part).
- Fichiers créés : `NOTICE.md`, `CHANGELOG.md`, `CODE_OF_CONDUCT.md`, `SECURITY.md`, `CONTRIBUTING.md`, `CONTRIBUTING.fr.md`, `CLAUDE.md`, `.gitignore` complété ; `PROMPT-claude-code-v1.md` déplacé dans `docs/`.
- `gh` installé (portée utilisateur), remote `origin` = https://github.com/Teumesse0/TeumesseCastle (dépôt public vide confirmé), push de `main`.

## Décisions à prendre par Johnny (non bloquantes)

1. Question juridique NPOSL-3.0 §17 (ADR-0003) : distribution par une société commerciale → NPOSL ou OSL-3.0. À trancher avant la release publique.
2. Contact sécurité (adresse e-mail) pour `SECURITY.md` et `CODE_OF_CONDUCT.md` (actuellement : GitHub private vulnerability reporting).
3. Emplacement du clone (ADR-0008) : rester dans OneDrive ou déplacer hors OneDrive ; proposition détaillée dans `docs/RELEASING.md` (Phase 6).
4. Décisions prises par défaut, à confirmer ou annuler : préfixes `ad_hc_` conservés (ADR-0005), projet au format SDK (ADR-0002), identité Git locale (ADR-0008).
