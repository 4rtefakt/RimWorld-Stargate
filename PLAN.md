# Mod RimWorld — Univers Stargate

> Document de cadrage / plan. Version initiale du 2026-06-12.
> Décisions actées : races via **xénotypes Biotech** (core-friendly), **Claude écrit tout le code** (XML + C#/Harmony), **textures placeholder** d'abord puis art final ensuite.
> **2026-09-22 :** VGE Chapitre 2 est sorti → **Vanilla Gravship Expanded (Ch.1 + Ch.2) devient une dépendance obligatoire** ; les étapes 3‑4 se construisent dessus (voir §2.3).

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
- **Vanilla Expanded Framework** (`OskarPotocki.VanillaFactionsExpanded.Core`, Workshop id `2023507013`) — requis par VGE.
- **Vanilla Gravship Expanded – Chapitre 1** (`vanillaexpanded.gravship`, Workshop id `3609835606`) — **dépendance obligatoire** : systèmes du gravship (oxygène, carburant, énergie, chaleur, équipage).
- **Vanilla Gravship Expanded – Chapitre 2 « The Battle »** (`vanillaexpanded.gravship2`, Workshop id `3799737423`, sorti en septembre 2026) — **dépendance obligatoire** : combat orbital (duels d'artillerie vs gravships ennemis, plateformes/satellites, abordages en hellpods), armes de vaisseau, coques blindées, brouilleurs, détection de menace orbitale. Débloqué après *Advanced gravtech*.
- ⚠️ **Chapitre 3 VGE : toujours inexistant** (ni sorti ni daté au 2026-09-22) → l'étape 5 reste autonome.
- **Décision (2026-09-22) :** VGE Ch.1 + Ch.2 en **dépendance obligatoire** (pas de compat optionnelle) : on étend leurs systèmes (defs de coques/armes/rencontres) au lieu de recoder le combat vaisseau. Contrepartie assumée : VGE est **incompatible avec tout autre mod qui modifie les gravships**, notre mod hérite de cette restriction.
- ✅ VEF, `vanillaexpanded.gravship`, `vanillaexpanded.gravship2` déclarés dans `About.xml` (`modDependencies` + `loadAfter`). Reste : références C# (DLL VEF/VGE) au `.csproj` si on les patche.

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
- [x] `Source/Stargate/` : `.csproj` (net472 → **net48** depuis la dépendance VGE, `Krafs.Rimworld.Ref`) + classe `Mod` minimale qui logue.
- [x] Build `dotnet build` → `Assemblies\Stargate.dll` (sortie propre, 0 erreur).
- [x] Def de test `SG_Naquadah` chargée **en jeu sans erreur** (Player.log vérifié).
- [x] Jonction du repo dans `RimWorld/Mods/Stargate` pour itérer.
> **Sortie atteinte :** mod reconnu (auteur 4rtefakt, id `4rtefakt.stargate`, v1.6), chargé après les DLC, C# exécuté (`[Stargate] Assembly chargée`), aucune erreur XML. Toolchain validée de bout en bout.

### Étape 1 — Races & mécaniques *(xénotypes Biotech — voir §4.5 et §4.6)*
- [~] Gènes custom : **poche à symbiote jaffa** `SG_JaffaPouch` FAIT (nuit du 22→23/09). *Reste optionnel : force Jaffa, intellect Asgard, naquadah dans le sang (accès aux appareils goa'uld).*
- [x] **Wraith** : xénotype `SG_Wraith` re-skinnant **Hemogenic → « essence vitale »** + **Bloodfeeder → « drain de vie »** (gènes `SG_VitalEssence`, `SG_LifeDrain`). Charge sans erreur. *(reste : art dédié.)*
- [x] **Jaffa** (`SG_Jaffa`) : xénotype guerrier + **dépendance au symbiote** : gène `SG_JaffaPouch` (`Gene_JaffaPouch`) — sans prim'ta ni tretonine → hediff `SG_SymbioteWithdrawal`, mortel en ~3 jours. Tout porteur reçoit une larve à l'acquisition du gène (pions générés, colons existants, xénogerme). **Tretonine** `SG_Tretonin` (drogue médicale, 1 dose/jour ; recette : larve + neutroamine au labo de drogues ; recherche `SG_Tretonin`).
- [~] **Goa'uld** (`SG_Goauld`) : xénotype de base FAIT — gène custom `SG_SarcophagusRegen` (re-skin `Gene_Deathrest` sans prereq Hemogenic) + `Deathless` + `Ageless` + mémoire génétique ; **sarcophage** `SG_Sarcophagus` (re-skin caisson deathrest) construit. *Reste : re-skin UI « deathrest » (Harmony).*
- [x] Autres xénotypes : **Tau'ri, Asgard, Unas, Tok'ra** FAITS (gènes vanilla confirmés).
- [x] **Cycle du symbiote Goa'uld + Reine (§4.6)** : larve périssable `SG_GoauldLarva`, symbiote adulte `SG_GoauldSymbiote`, hediff `SG_Primta` (maturation), chirurgies (`Recipe_SG_ImplantPrimta` / `ExtractSymbiote` / `ImplantGoauld` / `ImplantTokra`), Reines goa'uld & tok'ra (`Gene_GoauldBrooder`, décompte via `TickInterval`).

### Étape 2 — Équipement, armes, factions, idéologie *(complète hors precepts/rituel)*
- [x] **Naquadah** : `SG_Naquadah` = ressource **et matériau métallique** (murs, meubles, armes de mêlée, blindage de gravship VGE). Sources : raffinage `SG_RefineNaquadah`, **minerai** `SG_MineableNaquadah`, forage profond, marchands.
- [x] Armes : **bâton `SG_StaffWeapon`**, **kara kesh `SG_KaraKesh`**, **zat `SG_Zat` fidèle** (`DamageWorker_Zat` : 1er tir = inconscience `SG_ZatShock`, tir sur cible choquée = mort, IEM contre mécanoïdes/machines).
- [x] Armures & apparel : **armure + casque jaffa**, **robe goa'uld**, **uniforme SGC** (tag commercial `SG_StargateTech`).
- [x] Bâtiments : **sarcophage**, **générateur au naquadah** `SG_NaquadahGenerator` (2500 W, explose si détruit), **anneaux de transport** `SG_TransportRings` (téléportation même carte), bouclier → voir étape 3.
- [x] Factions : **Grands Maîtres Goa'uld** (culte des Goa'uld imposé) + **Commandement Stargate (Tau'ri)**, **Tok'ra** (avec la reine tok'ra), **Jaffa libres**, **Asgard** — pawnkinds, backstories, noms, icônes placeholder, **marchands dédiés** (`TraderKinds_Stargate.xml`).
- [x] **Recherche** : onglet dédié **Stargate** (`SG_StargateTab`) : technologie stargate → arsenal / technologie goa'uld / énergie au naquadah → technologie asgard, tretonine, activation de la porte.
- [~] Idéologie : **meme `SG_GoauldWorship`** (imposé aux Grands Maîtres, refusé par les factions alliées). *Reste : precepts + rituel prim'ta (passe testée).*

### Étape 3 — Vaisseaux (gravships) par race *(sur VGE Ch.1 + Ch.2, dépendance obligatoire)*
- [x] VEF + VGE Ch.1 + VGE Ch.2 déclarés dans `About.xml` ; C# compilé contre leurs DLL (références jamais copiées dans `Assemblies/`).
- [x] Defs/extensions VGE étudiées (sources publiques) ; validateur hors-jeu `Tools/validate.sh` qui vérifie nos defs contre les DLL du jeu + VEF + VGE.
- [x] Composants thématiques : **générateur au naquadah**, **bouclier goa'uld de gravship** `SG_GoauldShieldGenerator` (grand bouclier VGE, cœur de naquadah au lieu d'un gravcore), **blindage VGE en naquadah** (via le matériau).
- [x] **Menace orbitale Ha'tak** `SG_Hatak` (GravshipThreatDef VGE Ch.2 ; `GravshipThreatWorker_Hatak`, `GenStep_Hatak`) : faction goa'uld, équipage Jaffa/seigneurs (parfois une reine), noms de Ha'tak, **plan dédié** `SG_Hatak_1` (pyramide d'or, canons à plasma ennemis) généré et vérifié par `Tools/hatak/gen_hatak.py`.
- [ ] *(Optionnel)* Plans supplémentaires `SG_Hatak_N` (tirés au hasard), vaisseaux Asgard alliés, gravship de départ thématique.

### Étape 4 — Armes de vaisseau spécifiques *(extension de l'arsenal VGE Ch.2)*
- [x] **Canon à plasma goa'uld** `SG_GoauldStaffCannon` (artillerie VGE, munition = naquadah) + version ennemie pour les Ha'tak.
- [x] **Canon à ions asgard** `SG_AsgardIonCannon` (faisceau lourd VGE, traverse les boucliers, très énergivore).
- [x] Recherche des armes de vaisseau : recherche Stargate (goa'uld / asgard) + recherche VGE correspondante
  (armement, armement avancé, défenses). Pas de projet dédié dans l'onglet Gravtech (retour de playtest 0.2.1).
- [x] ~~Mécanique de combat vaisseau-vs-vaisseau à coder~~ → **fournie par VGE Ch.2**.

### Étape 5 — Système de quêtes : cité légendaire (Atlantis/cité des Anciens)
- [x] **Amorce : Porte des étoiles** `SG_Stargate` (MapPortal vanilla) + **DHD** `SG_DHD` : la traversée génère un **monde lointain** (carte-poche : surface tempérée, filons de naquadah) avec une porte de retour ; « composer une nouvelle adresse » referme le monde (recherche `SG_StargateActivation`).
- [ ] Chaîne de quêtes (QuestScriptDefs / C#) : adresses spéciales découvertes (ruines, Tok'ra, Jaffa libres…), jusqu'à la cité des Anciens.
- [ ] Mondes variés (biomes, ruines, présence goa'uld) au lieu d'un monde tempéré unique.
- [ ] Récompense de fin (tech Ancienne, Ascension).
- [ ] *(Le « basé sur VGE Chapitre 3 » est reporté : Ch.3 n'existe toujours pas au 2026-09-22. Version autonome, qui peut s'appuyer sur VGE Ch.1/2 déjà en dépendance ; on intégrera VGE Ch.3 s'il sort.)*

---

## 6. Risques & dépendances
1. **DLC manquants côté dev** (bloquant) → acheter Odyssey + Biotech + Ideology.
2. **Dépendance à VGE Ch.1 + Ch.2** (obligatoire) : incompatibilité avec les autres mods de gravships ; une mise à jour VGE peut casser nos defs/patchs → épingler la version testée et re-tester à chaque update. **VGE Ch.3 non sorti** → étape 5 autonome.
3. **Extensibilité de VGE** : explorée — armes, boucliers, menaces orbitales et plans de vaisseaux se déclinent en XML ; le C# se limite à des sous-classes (menace Ha'tak, génération). Les références à VGE sont vérifiées par `Tools/validate.sh`.
7. **Validation hors-jeu** : les Defs vanilla (Data du jeu) ne sont pas publiques ; une référence vanilla absente des `[DefOf]` et des mods de référence ne se prouve qu'en jeu (liste `Tools/DefValidator/vanilla-verified.txt`).
4. **Morphologie des Asgard** sans HAR → compromis visuel assumé (gènes peau/tête, pas un corps alien complet).
5. **Charge artistique** (textures) ~50 % du travail final → placeholders d'abord, art en dernier.
6. **Compat & équilibrage** : à tester en continu, d'où l'approche tranches verticales.

---

## 7. Prochaine action proposée
1. **Utilisateur :** tester en jeu la nuit du 22→23/09 en suivant `TESTS.md` (s'abonner d'abord à VEF + VGE Ch.1 + Ch.2), puis remonter le `Player.log`.
2. **Claude :** corriger les retours de test, puis enchaîner sur l'étape 5 (quêtes d'adresses → cité des Anciens) et l'art (prompts dans `Art/PROMPTS_MISTRAL.md`).
