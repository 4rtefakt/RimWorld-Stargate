using RimWorld;
using Verse;
using VanillaGravshipExpanded2;

namespace Stargate
{
    /// <summary>
    /// Menace orbitale VGE : un Ha'tak goa'uld (vaisseau-mère) repère le gravship du joueur
    /// et vient le bombarder. Réutilise toute la mécanique du « gravship ennemi » de VGE Ch.2
    /// (compte à rebours, bombardement, abordage, moteur à détruire, poursuite en cas de fuite)
    /// mais sous la bannière des Grands Maîtres Goa'uld : leur faction, leur équipage (Jaffa,
    /// seigneurs, parfois une reine) et des noms de vaisseaux goa'uld.
    /// </summary>
    public class GravshipThreatWorker_Hatak : GravshipThreatWorker_EnemyGravship
    {
        /// <summary>La faction goa'uld de la partie, ou null si elle n'existe pas / plus.</summary>
        public static Faction GoauldFaction
        {
            get
            {
                Faction faction = Find.FactionManager?.FirstFactionOfDef(SG_DefOf.SG_SystemLords);
                return faction != null && !faction.defeated ? faction : null;
            }
        }

        public override Faction EnemyFaction => GoauldFaction ?? base.EnemyFaction;

        /// <summary>Pas de Ha'tak sans Grands Maîtres hostiles dans la partie.</summary>
        public override bool CanFire(Building_GravEngine engine)
        {
            Faction goauld = GoauldFaction;
            return goauld != null && goauld.HostileTo(Faction.OfPlayer) && base.CanFire(engine);
        }

        /// <summary>
        /// Appelé une seule fois par <see cref="GravshipThreatWorker.Fire"/>, juste avant l'envoi de la
        /// lettre d'alerte : c'est le dernier moment pour baptiser le vaisseau. Le parent VGE lui a
        /// déjà donné un nom de gravship générique ; on le remplace par un nom de Ha'tak goa'uld,
        /// conservé ensuite par VGE pour toutes les lettres et alertes de la rencontre.
        /// </summary>
        public override TaggedString GetLetterDesc(CompSignalJammer jammer)
        {
            WorldComponent_GravshipCombat.Instance.enemyGravshipName = NameGenerator.GenerateName(SG_DefOf.SG_NamerHatak);
            return base.GetLetterDesc(jammer);
        }
    }
}
