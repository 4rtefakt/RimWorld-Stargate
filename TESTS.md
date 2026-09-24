# Tests en jeu — nuit du 22 → 23 septembre 2026

Tout ce qui suit **compile** et passe le **validateur hors-jeu** (`Tools/validate.sh`, 0 erreur,
0 avertissement), mais **rien n'a encore tourné dans RimWorld**. Ordre conseillé : du plus simple
au plus risqué. Pour chaque point, noter ✅ / ❌ et joindre le `Player.log` en cas de souci
(`%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`).

## 0. Préparation
- [ ] S'abonner sur le Workshop à **Vanilla Expanded Framework** (2023507013), **Vanilla Gravship
      Expanded – Chapter 1** (3609835606) et **Chapter 2** (3799737423).
- [ ] Ordre de chargement : Harmony → Core → DLC → VEF → VGE Ch.1 → VGE Ch.2 → **Stargate**.
- [ ] Activer le **mode développeur** (Options).
- [ ] Au lancement, chercher dans le `Player.log` : `[Stargate]`, `Config error`,
      `Could not resolve cross-reference`, `Could not load`, `Exception`.
      Attendu : seulement `[Stargate] Assembly chargée, patchs Harmony appliqués.`

## 1. Monde et factions (nouvelle partie)
- [ ] À la création du monde, 5 factions Stargate : **Grands Maîtres goa'uld**, **Commandement
      Stargate**, **Tok'ra**, **Jaffa libres**, **Asgard**, avec leurs icônes (placeholders).
- [ ] Noms : « Glaive d'Apophis »-style pour les Goa'uld (plus de « de Apophis »), « Site Bêta », etc.
- [ ] Idéologie des Grands Maîtres : contient le mème **culte des Goa'uld** ; les 4 autres factions non.
- [ ] Dev : *Spawn pawn* → `soldat du SGC`, `agent tok'ra`, `guerrier jaffa libre`, `défenseur asgard`,
      `reine tok'ra` : bon xénotype, équipement, histoire.
- [ ] Une caravane / un visiteur d'une faction alliée : stocks Stargate (naquadah, zats, bâtons,
      tretonine chez Tok'ra et Jaffa libres, parfois un symbiote tok'ra).

## 2. Recherche
- [ ] Nouvel onglet **Stargate** : technologie stargate → arsenal / technologie goa'uld / énergie
      au naquadah → technologie asgard, tretonine, activation de la porte. Pas de chevauchement.
- [ ] Pas de projet Stargate dans l'onglet **Gravtech**. La fiche *Gravship weaponry* (VGE) liste le canon
      à plasma goa'uld (« débloqué avec technologie goa'uld »), *Advanced gravship weaponry* le canon
      à ions asgard (avec technologie asgard), *Gravship defenses* le bouclier goa'uld (avec technologie goa'uld).

## 3. Naquadah
- [ ] Minerai de naquadah visible sur la carte (veines vertes, rares) ; se mine → naquadah.
- [ ] Le naquadah est un **matériau** : construire un mur / une table en naquadah (couleur vert-gris).
- [ ] **Générateur au naquadah** (onglet Énergie) : 2500 W une fois rempli de naquadah ; le détruire
      (dev : *Destroy*) → explosion.

## 4. Zat'nik'tel
- [ ] Dev : donner un zat à un colon, tirer sur un raider : il tombe **inconscient**, sans blessure
      (hediff *choc de zat*).
- [ ] Second tir sur la même cible (dans les ~20 h) : **mort** (« a succombé à une seconde décharge »).
- [ ] Sur un mécanoïde / une tourelle : **paralysie** (effet IEM), pas de dégâts.

## 5. Jaffa et tretonine
- [ ] Dev : *Spawn pawn* guerrier jaffa → il a **déjà** le gène poche à symbiote et le **symbiote prim'ta** (onglet santé). Un colon Jaffa existant le reçoit
      dans l'heure de jeu qui suit — pas de manque, pas de mort surprise.
- [ ] Chirurgie *extraire le symbiote goa'uld* → dans l'heure : message + hediff **manque de
      symbiote** (mortel en ~3 jours).
- [ ] Soigner : *implanter une larve goa'uld* **ou** faire prendre une **tretonine** → le manque disparaît
      dans l'heure. Programmer la tretonine « tous les 1 jour » dans une politique de drogues.
- [ ] Recette **synthétiser de la tretonine** au labo de drogues (1 larve + 4 neutroamine → 12 doses).

## 6. Anneaux de transport
- [ ] Construire 2 plateformes (onglet Divers, recherche technologie goa'uld), les alimenter.
- [ ] Mettre un colon + un objet sur l'une, *Activer les anneaux* → cibler l'autre : téléportation,
      flash, recharge de 30 min affichée.

## 7. Porte des étoiles (amorce de l'étape 5) — le plus risqué
- [ ] Construire une **porte des étoiles** + un **DHD** à moins de 12 cases, alimenter la porte.
      Sans DHD / sans énergie : message clair, porte non franchissable.
- [ ] *Franchir la porte* → choisir des colons (+ objets) → ils y vont et disparaissent.
- [ ] Un **monde lointain** est généré (forêt tempérée, 150×150), lettre « Monde lointain », équipe
      à côté de la porte de retour ; quelques filons de naquadah.
- [ ] Vérifier : lumière du jour / météo normales ? faune ? (point le plus incertain).
- [ ] *Rentrer par la porte* ramène l'équipe.
- [ ] *Composer une nouvelle adresse* (grisé tant qu'un pion de la colonie est là-bas, même à terre)
      → confirmation → la traversée suivante génère un **nouveau** monde.
- [ ] Démonter la porte pendant qu'un monde est relié : refusé (message). Dev *Destroy* sur la porte
      avec une équipe là-bas : l'équipe revient près de la porte, message « monde perdu ».

## 8. Gravship (VGE)
- [ ] **Canon à plasma goa'uld** (et canon à ions) : la tourelle pointe vers la cible, le tir part du bout du canon ; se construit sur la sous-structure, se recharge en naquadah
      (vide à la construction), se relie à un terminal de ciblage VGE, tire (projectiles orange).
- [ ] **Canon à ions asgard** : faisceau bleu, gros appel de puissance au tir.
- [ ] **Bouclier goa'uld** : relié au moteur (pas de « Not connected to grav engine »), un seul par gravship, bulle dorée, activation/charge comme le grand
      bouclier VGE.
- [ ] **Blindage de gravship** VGE : le naquadah est proposé comme matériau.

## 9. Ha'tak goa'uld (menace orbitale VGE Ch.2)
- [ ] Aperçu du plan : dev *VGE2 → Spawn StructureSet as skyfaller* → `SG_HatakSet` sur une zone
      dégagée : pyramide d'or, 4 canons à plasma, 2 tourelles DR, propulseurs à l'arrière, pièces
      pressurisées (pas de vide intérieur).
- [ ] Avec un gravship : dev *VGE2 → Force encounter (specific)* → **Ha'tak goa'uld** : lettre avec
      un nom de Ha'tak, compte à rebours, puis arrivée : équipage Jaffa/seigneurs (parfois une reine).
- [ ] Le Ha'tak bombarde avec des décharges de plasma ; détruire son moteur + son artillerie = victoire.

## 10. Non-régression (déjà testé avant cette nuit)
- [ ] Reines goa'uld / tok'ra : ponte toujours fonctionnelle (le décompte passe désormais par
      `TickInterval` : vérifier qu'une larve apparaît après ~5 jours, ou via le bouton dev).
      Nouveau : seules les reines **de la colonie** pondent (colon, prisonnière ou esclave) — plus
      de ponte chez l'ennemi.
- [ ] Wraith (drain), sarcophage, chirurgies du symbiote, raids goa'uld : inchangés.

## Points connus, non modifiés cette nuit
- Le patch de genre forcé (reines, Wraith) régénère le nom après génération : un nom de famille
  imposé par un scénario peut être perdu (cas rare).
- Les chirurgies du symbiote ne peuvent pas échouer (choix de conception à confirmer).
