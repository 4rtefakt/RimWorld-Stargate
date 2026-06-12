# Mod RimWorld — Univers Stargate

> Document de cadrage / plan. Version initiale du 2026-06-12.
> Décisions actées : races via **xénotypes Biotech** (core-friendly), **Claude écrit tout le code** (XML + C#/Harmony), **textures placeholder** d'abord puis art final ensuite.

---

## 1. Résumé & réalité du projet

L'objectif est un mod Stargate en **5 étapes** : races → équipement/factions/idéologie → vaisseaux par race (gravships) → armes de vaisseau → système de quêtes vers une cité légendaire.

**Ampleur réelle :** chacune des 5 étapes est, à elle seule, un mod de taille moyenne. L'ensemble représente plusieurs centaines d'heures si on vise du contenu fini. On ne construit donc PAS les 5 étapes en parallèle. On procède par **tranches verticales** : on fait d'abord une petite chose qui marche *de bout en bout* en jeu (1 race + 2 objets + 1 faction qui apparaît), on valide la chaîne d'outils, puis on élargit. C'est ce qui évite de découvrir au bout de 3 semaines que le build ne se charge pas en jeu.

---

## 2. Prérequis

### 2.1 DLC (bloquant)
Le mod cible un joueur qui a **tous les DLC** (cas du commanditaire). Mais pour *développer et tester*, l'environnement de dev a besoin de :

| DLC | Nécessaire pour | Statut |
|-----|-----------------|--------|
| **Base game** (RimWorld 1.6) | tout | requis |
| **Odyssey** | gravships (étapes 3‑5) | **requis pour dev** |
| **Biotech** | races = xénotypes/gènes (étape 1), mécanique symbiote | **requis pour dev** |
| **Ideology** | idéologie Goa'uld / culte Jaffa / rituels (étape 2) | **requis pour dev** |
| Royalty | psy/titres → Anciens/Ascension (optionnel, plus tard) | optionnel |
| Anomaly | entités étranges → Réplicateurs (optionnel, plus tard) | optionnel |

➡️ **Action utilisateur :** acheter/installer au minimum **Odyssey + Biotech + Ideology** avant qu'on attaque le contenu. (Royalty/Anomaly peuvent attendre les phases tardives.)

### 2.2 Outils de développement
- **RimWorld installé** (Steam) + le dossier `Mods/` local pour tester (symlink vers ce repo).
- **.NET SDK** (build du C#). Les assemblies RimWorld ciblent **.NET Framework 4.7.2/4.8**. On utilise le package NuGet **`Krafs.Rimworld.Ref`** (fournit les DLL de référence sans copier les fichiers du jeu) + **`Lib.Harmony`**.
- **IDE** : VS Code suffit (Claude écrit le code, build via `dotnet build`).
- **Éditeur d'images** (plus tard, pour l'art) : Paint.NET / GIMP / Aseprite. Placeholders d'abord.
- **Git** (ce repo n'est pas encore un dépôt git — à initialiser).

### 2.3 Dépendances mod (runtime)
- **Harmony** (mod communautaire, Workshop id `2009463077`) — déclaré en dépendance pour le patching C#.
- DLC déclarés en `modDependencies` / `loadAfter` dans `About.xml`.
- ⚠️ **Vanilla Gravship Expanded** : Chapitre 1 sorti ; **Chapitre 2 en dev, Chapitre 3 inexistant** à ce jour. → On **ne dépend pas** d'un mod non sorti. Les étapes 3 et 5 se construisent sur la mécanique **gravship vanilla d'Odyssey**, avec une *compat optionnelle* VGE si dispo.

---

## 3. Stack technique & structure du mod

```
rimworld-sg1/
├─ About/
│  ├─ About.xml            # métadonnées, DLC requis, deps, supportedVersions=1.6
│  ├─ Preview.png          # image Workshop (plus tard)
│  └─ PublishedFileId.txt  # généré à la 1re publication Workshop
├─ Defs/                   # contenu XML (ThingDef, GeneDef, XenotypeDef, FactionDef…)
├─ Patches/                # PatchOperations XML (modifs non-destructives du vanilla)
├─ Textures/               # PNG (placeholders d'abord)
├─ Sounds/
├─ Languages/
│  ├─ French/
│  └─ English/
├─ Assemblies/             # DLL compilée (sortie du build C#)
├─ Source/                 # code C# + .csproj (non livré au joueur)
└─ LoadFolders.xml         # (optionnel) chargement par version
```

**Règle d'or :** le **XML** porte 90 % du contenu (objets, gènes, factions, recherches). Le **C#/Harmony** ne sert qu'aux mécaniques que le XML ne sait pas faire (cycle du symbiote, comportement d'arme spécial, logique de quête).

**Principe qualité (acté par le demandeur) :** on écrit du **code propre et définitif**, pas des contournements jetables qui laissent du code mort. Si une mécanique mérite un vrai système (ex : cycle de vie du symbiote Goa'uld), on le construit proprement plutôt que d'empiler un placeholder qu'on devra supprimer.

---

## 4. Cartographie Stargate → RimWorld

### 4.1 Races (xénotypes Biotech)
| Stargate | Approche RimWorld | Gènes / mécaniques clés |
|----------|-------------------|--------------------------|
| **Tau'ri** (Terriens) | xénotype quasi-baseline | léger bonus tir/recherche ; sert de « faction joueur » par défaut |
| **Jaffa** | xénotype guerrier | force/robustesse, vieillissement lent ; **abrite un symbiote** = hediff « prim'ta » qui satisfait son besoin vital (voir §4.6) ; fort en mêlée/tir |
| **Goa'uld** | xénotype basé sur **Deathrest + Deathless** (voir §4.5) ; on **devient** Goa'uld via le cycle du symbiote (§4.6) | doit retourner au **sarcophage** (Deathrest re-skinné) pour rester en vie + ressuscite (Deathless) ; mémoire génétique (skills élevés), naquadah dans le sang |
| **Tok'ra** | variante Goa'uld bénéfique (blended) | comme Goa'uld mais relation hôte coopérative |
| **Wraith** (Atlantis) | xénotype basé sur **Hemogenic + Bloodfeeder** (voir §4.5) | se nourrit de la **force vitale** (hémogène re-skinné) via morsure/drain ; fort, régénération |
| **Asgard** | xénotype | corps frêle, intellect énorme, imberbe/peau grise, **stérile (clonage)** ; *morphologie limitée sans HAR — compromis assumé* |
| **Unas** | xénotype primitif | très fort/robuste, lent en recherche, hôte Goa'uld d'origine |
| **Réplicateurs** | **faction d'entités/mécanoïdes** (pas une race jouable) | menace auto-réplicante hostile ; candidat DLC Anomaly |
| **Anciens / Ori** | **contenu de quête / faction tardive** | ascension, tech avancée ; pas une race de départ |

### 4.2 Ressources & mécaniques transverses
- **Naquadah** : minerai + ressource raffinée (alimente armes, générateurs, tech Goa'uld).
- **Symbiote / prim'ta** : need des Jaffa ; objet vivant transférable.
- **Sarcophage** : bâtiment de soin Goa'uld (puissant, avec contrepartie : addiction/corruption).
- **Stargate + DHD + anneaux de transport** : téléportation / voyage de carte (gros morceau, lié aux quêtes étape 5).

### 4.3 Équipement (étape 2)
- **Armes** : bâton Jaffa (plasma, puissant/imprécis), **zat'nik'tel** (étourdir→tuer→désintégrer), **kara kesh / ribbon device**, intar (entraînement), P90 (Tau'ri), variantes enrichies au naquadah.
- **Armures** : gardes serpent/horus (Jaffa), robes Goa'uld, uniformes SGC, armure Kull.
- **Bâtiments** : sarcophage, générateur à naquadah, générateur de bouclier, anneaux, DHD, Stargate.

### 4.4 Factions & idéologie
- **Grands Maîtres (System Lords)** — empire Goa'uld hostile, armées Jaffa, tech avancée.
- **Tau'ri / SGC** & **Jaffa Libres** — alliés/neutres.
- **Tok'ra**, **Asgard** — alliés avancés.
- **Réplicateurs** — menace mécanique hostile.
- **Idéologie (DLC)** : meme « culte des Goa'uld (faux dieux) », servitude Jaffa, rituel prim'ta ; meme « Ascension » pour les Anciens.

### 4.5 Réutilisation des mécaniques Biotech vanilla (re-skin) — *principe directeur*
Plutôt que coder des besoins en C# from scratch, on **réutilise les sous-systèmes Biotech existants et on les re-skinne** (stats, hediffs, icône, textes). Beaucoup moins de bugs.

| Concept Stargate | Mécanique vanilla réutilisée | Effort de re-skin |
|---|---|---|
| **Wraith** — drain de force vitale | gène **Hemogenic** (barre de ressource) + **Bloodfeeder** (morsure/drain) | 🟢 **Facile, ~XML pur** : `resourceLabel`/`resourceDescription`/icône dans le `GeneDef`, label de l'`AbilityDef` de morsure. « hémogène » → « force vitale », ajuster stats. C# quasi nul. |
| **Goa'uld** — dépendance au sarcophage | gène **Deathrest** + bâtiment **caisson** → re-skinné en **sarcophage** | 🟡 **Moyen** : ThingDef du sarcophage facile ; mais le besoin/UI « deathrest » a des **strings codés en dur** → effacer totalement le mot « deathrest » demande des **patchs Harmony**, sinon on garde quelques libellés vanilla. |
| **Goa'uld** — résurrection | gène **Deathless** | 🟢 Réutilisable tel quel, très canon. |

> Le **besoin de symbiote du Jaffa** n'est PAS un gène de dépendance chimique (écarté = code mort) : c'est le hediff « prim'ta » du cycle de vie — voir §4.6.

**Implication :** Wraith = re-skin XML rapide et sûr ; Goa'uld (Deathrest) garde une petite part Harmony pour un UI 100 % thématique ; le cycle du symbiote (§4.6) est un système custom propre, pas un re-skin.

### 4.6 Cycle de vie du symbiote Goa'uld & Reine *(système custom, code propre — committed)*
Mécanique signature qui **lie Jaffa et Goa'uld**. Dirigée par le joueur (chirurgies), pas d'IA de ponte autonome. Sources : surgeries + hediffs + comps custom dédiés.

**Le cycle :**
1. **Reine Goa'uld** (pawn/xénotype rare, capturable sur les Grands Maîtres) **produit des larves** via un `ThingComp`/`HediffComp` custom `CompGoauldBrooder` : à intervalle, si vivante + nourrie, pond une larve (consomme nutrition/naquadah, plafond de stock).
2. **Larve** = item **périssable** : meurt si pas implantée à temps (comp de péremption dédié).
3. Chirurgie **« implanter prim'ta »** sur un Jaffa → hediff **« symbiote immature »** : donne au Jaffa son **bonus d'immunité** ET satisfait son **besoin vital** (sans symbiote → hediff de déclin de santé).
4. Le hediff **mûrit avec le temps** (`SeverityPerDay` ou comp de maturation) → **« symbiote mature »**.
5. Chirurgie **« extraire symbiote mature »** → item **« symbiote Goa'uld adulte »**.
6. Chirurgie **« implanter dans un hôte humain »** → `RecipeWorker` C# qui **convertit le pion en xénotype Goa'uld** (Deathrest+Deathless) + hediff de prise de contrôle.

**C# propre nécessaire (purpose-built, zéro contournement) :**
- `CompGoauldBrooder` — production de larves par la reine.
- comp de maturation du symbiote (si `SeverityPerDay` XML insuffisant).
- comp de péremption de la larve hors hôte.
- `Recipe_ImplantGoauldSymbiote` — conversion hôte → xénotype Goa'uld.

**Tretonin** (drogue qui remplace le besoin de symbiote) = contenu *alternatif* optionnel plus tard, pas un placeholder.

---

## 5. Plan par phases

### Phase 0 — Squelette & chaîne d'outils ✅ TERMINÉE (2026-06-12)
- [x] `git init` + `.gitignore` (ignore `Assemblies/*.dll`, `obj/`, `bin/`).
- [x] `About/About.xml` (1.6, deps DLC). *(Harmony reporté : pas nécessaire en Phase 0.)*
- [x] `Source/Stargate/` : `.csproj` (net472, `Krafs.Rimworld.Ref` 1.6.4850) + classe `Mod` minimale qui logue.
- [x] Build `dotnet build` → `Assemblies\Stargate.dll` (sortie propre, 0 erreur).
- [x] Def de test `SG_Naquadah` chargée **en jeu sans erreur** (Player.log vérifié).
- [x] Jonction du repo dans `RimWorld/Mods/Stargate` pour itérer.
> **Sortie atteinte :** mod reconnu (auteur 4rtefakt, id `4rtefakt.stargate`, v1.6), chargé après les DLC, C# exécuté (`[Stargate] Assembly chargée`), aucune erreur XML. Toolchain validée de bout en bout.

### Étape 1 — Races & mécaniques *(xénotypes Biotech — voir §4.5 et §4.6)*
- [ ] Gènes custom (force Jaffa, intellect Asgard, naquadah, etc.).
- [x] **Wraith** : xénotype `SG_Wraith` re-skinnant **Hemogenic → « essence vitale »** + **Bloodfeeder → « drain de vie »** (gènes `SG_VitalEssence`, `SG_LifeDrain`). Charge sans erreur. *(reste : tester le comportement du drain en partie + art dédié.)*
- [~] **Jaffa** (`SG_Jaffa`) : xénotype guerrier de base FAIT (force, immunité, régén — gènes vanilla). *Reste : hediff « prim'ta » comme besoin vital, lié au cycle §4.6.*
- [~] **Goa'uld** (`SG_Goauld`) : xénotype de base FAIT — gène custom `SG_SarcophagusRegen` (re-skin `Gene_Deathrest` sans prereq Hemogenic) + `Deathless` + `Ageless` + mémoire génétique ; **sarcophage** `SG_Sarcophagus` (re-skin caisson deathrest) construit. *Reste : valider la mécanique deathrest en jeu, re-skin UI « deathrest » (Harmony), conversion via cycle §4.6.*
- [ ] Autres xénotypes : Tau'ri, Asgard, Unas, Tok'ra.
- [ ] **Cycle du symbiote Goa'uld + Reine (§4.6)** — système custom committed : larve périssable, 3 chirurgies, `CompGoauldBrooder`, `Recipe_ImplantGoauldSymbiote`.
> **Ordre de construction (tranches verticales, chaque pièce est définitive — pas de placeholder) :**
> 1. **Wraith** (valide le pipeline xénotype + re-skin Hemogenic, le plus autonome).
> 2. **Jaffa + Goa'uld** xénotypes qui se chargent et se jouent.
> 3. **Cycle du symbiote + Reine** branché par-dessus des races déjà fonctionnelles.

### Étape 2 — Équipement, armes, factions, idéologie
- [ ] Ressource naquadah (minerai, raffinage, recherche).
- [ ] Armes : bâton, zat, kara kesh (comportements C# pour zat/kara kesh).
- [ ] Armures & apparel.
- [ ] Bâtiments : sarcophage, générateur naquadah.
- [ ] FactionDefs (System Lords, Tau'ri, Tok'ra…) + PawnKindDefs.
- [ ] Memes/precepts Ideology (culte Goa'uld, prim'ta).

### Étape 3 — Vaisseaux (gravships) par race
- [ ] Construits sur la mécanique **gravship vanilla d'Odyssey** (pas sur VGE non sorti).
- [ ] Layouts/structures de gravship thématiques par faction (Ha'tak Goa'uld, vaisseau Asgard…).
- [ ] Rencontres de combat : vaisseaux ennemis que le joueur affronte.
- [ ] *(Optionnel)* compat VGE Chapitre 1/2 si présent.

### Étape 4 — Armes de vaisseau spécifiques
- [ ] Armement embarqué par race (canons plasma Goa'uld, faisceau Asgard…).
- [ ] Intégration aux vaisseaux de l'étape 3.
- [ ] Mécanique de combat vaisseau-vs-vaisseau (dépend de ce qu'Odyssey expose ; C# probable).

### Étape 5 — Système de quêtes : cité légendaire (Atlantis/cité des Anciens)
- [ ] Chaîne de quêtes (QuestScriptDefs / C#).
- [ ] Voyage via Stargate / gravship vers une carte spéciale.
- [ ] Récompense de fin (tech Ancienne, Ascension).
- [ ] *(Le « basé sur VGE Chapitre 3 » est reporté : Ch.3 n'existe pas. On fait une version autonome ; on intégrera VGE Ch.3 s'il sort.)*

---

## 6. Risques & dépendances
1. **DLC manquants côté dev** (bloquant) → acheter Odyssey + Biotech + Ideology.
2. **VGE Chapitre 2/3 non sortis** → on construit sur le vanilla Odyssey, compat optionnelle.
3. **API gravship d'Odyssey** : étendue exacte de ce qu'on peut scripter (combat vaisseau) à explorer une fois Odyssey installé — peut limiter les étapes 4‑5.
4. **Morphologie des Asgard** sans HAR → compromis visuel assumé (gènes peau/tête, pas un corps alien complet).
5. **Charge artistique** (textures) ~50 % du travail final → placeholders d'abord, art en dernier.
6. **Compat & équilibrage** : à tester en continu, d'où l'approche tranches verticales.

---

## 7. Prochaine action proposée
1. **Utilisateur :** installer RimWorld + Odyssey + Biotech + Ideology, confirmer le chemin d'install.
2. **Claude :** lancer **Phase 0** (squelette + build + def de test qui se charge en jeu).
3. Puis **Étape 1, tranche Jaffa** comme première verticale jouable.
