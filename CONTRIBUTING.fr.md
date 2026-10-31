# Contribuer à TeumesseCastle

*English version: [CONTRIBUTING.md](CONTRIBUTING.md)*

Merci de contribuer à un outil d'audit Active Directory libre. Ce document décrit notre façon de travailler.

## Règles de base

1. **TeumesseCastle reste open source, pour toujours.** Toute contribution est acceptée sous la licence du
   projet (NPOSL-3.0, voir `LICENSE.md`). Nous n'ajoutons jamais de vérification de licence, d'édition
   payante, de télémétrie ni d'appel réseau sortant non documenté. Les pull requests qui en introduisent
   sont fermées.
2. **Aucune donnée réelle d'Active Directory.** Ne jamais committer, coller dans une issue ou joindre un
   rapport, un log, un XML ou une capture contenant des données d'un vrai domaine. Anonymisez ou
   fabriquez des données.
3. **Le référentiel de règles est une référence.** Modifier le score, le seuil, le niveau de maturité ou le
   texte d'une règle est un changement fonctionnel qui exige une issue dédiée et une justification sourcée
   (ANSSI, Microsoft, MITRE, CIS).
4. Restez courtois. Voir `CODE_OF_CONDUCT.md`.

## Déroulement

1. Ouvrez d'abord une issue pour tout ce qui dépasse la coquille (bug, fonctionnalité, nouvelle règle) avec
   les modèles fournis.
2. Forkez, puis créez une branche depuis `main` : `feat/<sujet>`, `fix/<sujet>`, `docs/<sujet>`,
   `rule/<ID-REGLE>`.
3. Commitez en **Conventional Commits** (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`, `ci:` ;
   `feat!:` ou un pied `BREAKING CHANGE:` pour un changement incompatible). Un changement logique par commit.
4. Vérifiez que le build est vert et que les tests passent (voir ci-dessous) **avant chaque commit**.
5. Ouvrez une pull request vers `main` et remplissez la checklist. Un mainteneur relit ; la CI doit être verte.

## Build et tests

Prérequis : Windows, .NET SDK 8.x (le projet cible .NET Framework 4.5.2 ; les assemblys de référence sont
restaurés depuis NuGet, aucun targeting pack à installer).

```powershell
dotnet build TeumesseCastle.sln -c Release
dotnet test  TeumesseCastle.Tests -c Release
.\bin\Release\TeumesseCastle.exe --help
```

Visual Studio 2022 (toute édition) ouvre aussi la solution directement.

## Conventions de code

- Les commentaires et la documentation XML du code C# sont écrits **en anglais**.
- Chaque type et membre public ou interne porte un commentaire XML (`<summary>`, `<param>`, `<returns>`,
  `<remarks>` si utile). `CS1591` est une erreur en configuration Release : le build l'impose.
- Les algorithmes non évidents (parsing LDAP, calcul des scores, parcours du graphe, décodage de structures
  natives, P/Invoke) reçoivent des commentaires `//` expliquant le *pourquoi*, pas seulement le *quoi*.
- Ne commentez jamais une méthode que vous n'avez pas lue. En cas de doute, écrivez `// TODO(review): ...`
  plutôt que d'inventer.
- Conservez l'en-tête de copyright upstream dans chaque fichier. Les fichiers que vous modifiez portent la
  ligne `// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.` en dessous.
- Préservez le comportement. Un refactoring ne doit pas changer les rapports générés (HTML, XML, JSON) pour
  une même entrée. En cas de doute, comparez avec la sortie de référence de PingCastle 3.0.0.0.

## Ajouter une règle de sécurité

1. Ouvrez une issue « Nouvelle règle de sécurité » décrivant la détection, le risque et la remédiation, avec
   les sources.
2. Implémentez-la dans `Healthcheck/Rules/` sur le modèle d'une règle existante (attributs `[RuleModel]`,
   `[RuleComputation]`, `[RuleMaturityLevel]`, références `[RuleANSSI]`, `[RuleSTIG]`, `[RuleMitreAttack]`).
3. Ajoutez les textes dans `Healthcheck/Rules/RuleDescription.resx` et régénérez `docs/RULES.md` avec
   `tools/Generate-RulesDoc.ps1`.
4. Ajoutez un test unitaire avec une fixture synthétique.
5. Mettez à jour `CHANGELOG.md` sous `[Unreleased]`.

## Documentation

La documentation utilisateur existe en anglais (`README.md`) et en français (`README.fr.md`). Quand vous
modifiez l'une, modifiez l'autre, ou signalez dans la PR qu'une traduction est nécessaire.
