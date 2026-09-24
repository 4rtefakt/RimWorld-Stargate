# Notes de version

## 0.2.1-beta — correctifs (23 septembre 2026)

Merci à Hitomi pour les premiers retours.

- **Bouclier goa'uld** : il affichait « Not connected to grav engine » et ne pouvait pas s'activer.
  Il se relie maintenant au moteur de gravship (moteur vanilla et moteurs de VGE).
- **Canons de vaisseau** : les tourelles du canon à plasma et du canon à ions visaient de côté
  (le tir partait du flanc). Elles pointent maintenant vers la cible.
- **Canon à plasma** : le nuage de poussière beige autour de la tourelle a été retiré.
- **Bâton jaffa** : il est tenu dans l'axe du tir au lieu d'être penché de 45°.
- **Jaffa** : les Jaffa (raids, visiteurs, marchands, xénogerme) apparaissent directement avec leur
  larve prim'ta. Avant, elle n'arrivait qu'au premier contrôle, jusqu'à une heure de jeu plus tard.
- **Recherche des armes de vaisseau simplifiée** : les trois projets de l'onglet Gravtech sont
  supprimés, car il fallait trop de gravdata pour les atteindre. Chaque arme demande maintenant une recherche Stargate
  plus la recherche VGE équivalente, qu'on fait de toute façon :
  - canon à plasma goa'uld : *technologie goa'uld* + *Gravship weaponry* ;
  - canon à ions asgard : *technologie asgard* + *Advanced gravship weaponry* ;
  - bouclier goa'uld : *technologie goa'uld* + *Gravship defenses*.

  Sur une sauvegarde existante, les recherches déjà faites dans ces trois projets sont perdues
  (message d'avertissement possible au chargement, sans conséquence).

## 0.2.0-beta — « Au-delà de la Porte » (23 septembre 2026)

> 🧪 **Version bêta** : tout compile et passe le validateur de defs, mais n'a pas encore été
> testé en partie. Faites une sauvegarde avant de l'ajouter, et signalez les problèmes avec votre `Player.log`.
>
> ⚠️ **Nouvelles dépendances obligatoires** : Vanilla Expanded Framework, Vanilla Gravship Expanded
> – Chapitre 1 et Chapitre 2. Ordre : Harmony → Core → DLC → VEF → VGE Ch.1 → VGE Ch.2 → Stargate.
> VGE est incompatible avec les autres mods qui modifient les gravships ; Stargate l'est donc aussi.

### Nouveautés

**Porte des étoiles**
- **Porte des étoiles** et **DHD** constructibles (recherche *activation de la porte des étoiles*,
  400 naquadah). Il faut le DHD à moins de 12 cases et 1200 W.
- *Franchir la porte* envoie une équipe (colons, animaux, objets) sur un **monde lointain** généré à
  la première traversée (forêt tempérée 150×150, filons de naquadah). La porte de là-bas ramène l'équipe.
- *Composer une nouvelle adresse* referme le monde actuel. La prochaine traversée mène à un monde neuf.
  L'option est bloquée tant qu'un pion de la colonie est là-bas.
- La porte ne peut pas être démontée tant qu'un monde est relié. Si elle est détruite, le vortex
  ramène l'équipe et le monde est perdu.

**Anneaux de transport**
- Plateforme goa'uld 3×3, 400 W, recherche *technologie goa'uld*.
- Téléporte **instantanément** tout ce qui s'y trouve vers une autre plateforme de la même carte.
- Recharge de 30 minutes de jeu entre deux activations.

**Naquadah**
- **Minerai de naquadah** : veines rares sur les cartes, et filons plus riches sur les mondes de la Porte.
- On en trouve aussi au **forage profond**.
- Le naquadah devient un **métal** de construction. Il est plus solide que l'acier et très
  résistant à la chaleur, mais pas aussi dur que le plasteel. On peut en faire des murs, des
  meubles, des armes de mêlée et du **blindage de gravship**.
- **Générateur au naquadah** : 2500 W pour 2 naquadah par jour. Il **explose** s'il est détruit.
- Valeur marchande : 5 → **8**.

**Zat'nik'tel, fidèle à la série**
- **1er tir** : la cible perd connaissance pendant environ 2 h, sans blessure ni sang.
- **Second tir** sur une cible encore sous le choc (environ 20 h) : **mort**.
- Contre les **mécanoïdes et les tourelles** : paralysie de type IEM (IEM = impulsion électromagnétique).
- Les armures ne protègent pas du zat, mais les boucliers personnels l'arrêtent.

**Jaffa**
- Nouveau gène **poche à symbiote** : un Jaffa adulte vit grâce à sa **larve prim'ta**.
- Sans larve ni tretonine, il tombe en **manque de symbiote** et **meurt en environ 3 jours**.
- Tout Jaffa reçoit une larve en acquérant le gène : les Jaffa de vos sauvegardes ne mourront pas.
- Nouvelle drogue médicale, la **tretonine** : une dose tient environ une journée. Elle se programme
  dans la politique de drogues.
  - Fabrication au labo de drogues : 1 larve + 4 neutroamine → 12 doses (recherche *tretonine*).
  - Vendue par les Tok'ra et les Jaffa libres.

**Factions**
- **Commandement Stargate** (Tau'ri, militaires et scientifiques), **Tok'ra** (espions ; une reine
  tok'ra vit dans leurs colonies), **Jaffa libres** et **Asgard** (rares, très avancés).
- Ces quatre factions refusent le **culte des Goa'uld**, qui est désormais imposé aux Grands Maîtres.
- Chaque faction a ses **marchands** : naquadah, zats, bâtons, armures, tretonine, et parfois un
  symbiote tok'ra.
- Tous ces marchands rachètent l'équipement Stargate.

**Gravships (Vanilla Gravship Expanded)**
- **Canon à plasma goa'uld** (3×3) : salves explosives, recharge au naquadah, livré vide.
- **Canon à ions asgard** (5×5) : faisceau précis qui **traverse les boucliers**. Il tire environ
  180 000 W pendant la rafale.
- **Bouclier goa'uld** : bulle dorée, alimentée par un cœur de naquadah au lieu d'un gravcore.
  Un seul par gravship, cumulable avec les boucliers VGE.
- Nouvelle menace orbitale, le **Ha'tak goa'uld** :
  - il s'ajoute aux menaces de VGE quand votre gravship devient trop visible ;
  - il a un plan dédié : pyramide d'or, 4 canons à plasma, 2 tourelles de défense rapprochée ;
  - équipage de Jaffa et de seigneurs goa'uld, et **parfois une reine** à capturer ;
  - il n'apparaît que si les Grands Maîtres existent et vous sont hostiles.

**Recherche**
- Nouvel onglet **Stargate**. Les recherches existantes y sont déplacées ; la progression des
  sauvegardes est conservée.
- Trois projets de vaisseau dans l'onglet **Gravtech** de VGE, financés par les gravdata :
  *armement des Ha'tak*, *armement asgard*, *défenses des Ha'tak*.

### Changements
- Noms de faction goa'uld corrigés (« d'Apophis » au lieu de « de Apophis ») et nouveaux seigneurs.
- Les reines ne pondent plus que pour la colonie (colon, prisonnière ou esclave).
- **Graphismes** : toutes les textures du mod sont désormais dédiées (générées avec Mistral, puis
  détourées) : naquadah (3 tailles de pile), larve, symbiotes, tretonine, bâton, zat, kara kesh et leurs
  projectiles, icônes au sol des tenues, porte, DHD, anneaux, sarcophage, générateur, canons (socle +
  tourelle), bouclier goa'uld, icônes de commande, 5 factions, 5 gènes, 9 xénotypes et le mème
  *culte des Goa'uld*. Restent vanilla : les tenues **portées** par les pions et le minerai de naquadah
  (roche teintée).

### Compatibilité des sauvegardes
- Ajout **sans risque** en cours de partie.
- Les nouvelles factions n'apparaissent pas seules dans un monde existant. Au chargement,
  **VEF propose de les ajouter** (fonction « Faction Discovery »).
- Les Jaffa existants reçoivent une larve dans l'heure de jeu qui suit le chargement.

---

## À tester — guide du joueur vétéran

Le tour de base (tout se charge, tout se construit) est dans `TESTS.md`. Ici, on cherche ce qui
**casse l'équilibre ou le jeu** quand on pousse les systèmes.

**Économie et naquadah**
- Le naquadah comme matériau est-il trop fort pour son prix (8) face au plasteel ? Regarder les
  murs, les armes de mêlée et surtout le **blindage VGE**.
- Farmer le naquadah via la Porte (mondes sans raids, filons garantis) : trop rentable ?
  Le coût de 400 naquadah suffit-il comme verrou ?
- Le raffinage (25 chemfuel + 1 composant → 10 naquadah) casse-t-il le jeu couplé au générateur
  (2 naquadah/jour pour 2500 W, contre le générateur au chemfuel) ?
- Revente aux marchands de l'équipement Stargate fabriqué : boucle de profit ?

**Zat**
- **Exécutions et captures** : deux tirs = une mort propre, sans malus d'humeur « exécution » ?
  Un tir = une capture facile : trop fort contre les raids ?
- Tirs amis et tirs perdus dans une mêlée : combien de colons meurent au 2e tir ?
- Les seigneurs goa'uld et les agents tok'ra armés de zats : l'IA assomme-t-elle puis kidnappe-t-elle
  vos colons ? Reste-t-elle passive ?
- Effet IEM : durée de paralysie des mécanoïdes (adaptation), des tourelles et des tourelles de gravship.

**Jaffa, symbiotes, tretonine**
- Boucle d'exploitation : capturer des Jaffa → extraire leur larve → la réimplanter = recrutement
  instantané ? Élevage de reines → armée de Goa'uld ?
- La tretonine par politique de drogues : oubli de dose en **caravane**, en **cryptosommeil**, pour
  un **prisonnier** ou un **esclave**. Le manque se déclenche-t-il au mauvais moment ?
- Xénogermes : un non-Jaffa qui reçoit le gène poche via xénogerme reçoit-il bien sa larve ?
  Retirer le gène retire-t-il bien le manque ?
- Enfants jaffa (gène actif à 13 ans) : la larve arrive-t-elle à l'anniversaire ?
- Sarcophage et chirurgie goa'uld sur un Jaffa en manque : le manque disparaît-il avec la conversion ?

**Porte et mondes lointains**
- Lumière du jour, météo, saisons, température fixe (18 °C) : le monde est-il « vivant » ?
- Traverser avec des prisonniers, des animaux, des caravanes à moitié chargées, des pions à terre.
- Destruction de la porte par un raid avec une équipe de l'autre côté : retour correct ? Objets perdus ?
- Porte construite **sur un gravship** puis décollage et atterrissage ailleurs : le monde reste-t-il
  relié ? Le retour fonctionne-t-il ?
- Performances : plusieurs mondes ouverts à la fois (plusieurs portes) ?

**Graphismes**
- Tailles et lisibilité en jeu : porte (5×1), canons (socle/tourelle alignés, rotation de la tourelle),
  bouclier goa'uld (6×6), piles de naquadah. Détourage : restes de fond blanc ?

**Anneaux**
- Téléporter des **ennemis** ou des prisonniers, sur une plateforme encombrée ou avec un mur posé dessus.
- Usage défensif (évacuation), abus en combat (sortir d'un siège) : cooldown suffisant ?
- Plateformes sur un gravship.

**Gravships et Ha'tak**
- Difficulté du Ha'tak face à un gravship de milieu de partie : 4 canons + 2 DR, 10 à 16 pions à bord.
- Aborder le Ha'tak : pièces pressurisées ? Portes vers le vide ? Équipage qui étouffe ?
  Butin (naquadah, caisses) ?
- Fuite avant l'arrivée : le Ha'tak vous poursuit-il (40 % de chances, comme les gravships VGE) ?
- Canon à ions : le réseau d'un gravship tient-il l'appel de puissance ? Le fait de traverser les
  boucliers est-il trop fort ?
- Bouclier goa'uld + 2 boucliers VGE sur le même vaisseau : trop tanky ?
- Canon à plasma sans munitions, ou naquadah coincé dans l'inventaire du gravship en vol.

**Factions et monde**
- Fréquence des Tok'ra et des Asgard (poids de colonie faibles), raids éventuels des Jaffa libres et
  des Tau'ri si les relations se dégradent : puissance cohérente ?
- Reine tok'ra dans les colonies Tok'ra : attaquer une colonie tok'ra pour voler une reine, conséquences ?
- Idéologie : les Grands Maîtres ont-ils toujours le culte des Goa'uld ? L'ajout d'autres mods
  d'idéologie le fait-il échouer ?
