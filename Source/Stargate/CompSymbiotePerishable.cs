using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    public class CompProperties_SymbiotePerishable : CompProperties
    {
        // Durée de vie hors d'un hôte avant que le symbiote ne périsse.
        public float lifespanDays = 5f;

        // Libellé affiché dans l'inspecteur (avant le temps restant).
        public string survivalLabel = "Survie restante";

        public CompProperties_SymbiotePerishable()
        {
            compClass = typeof(CompSymbiotePerishable);
        }
    }

    /// <summary>
    /// Symbiote fragile (larve ou adulte) : hors d'un hôte, il meurt s'il n'est
    /// pas implanté avant la fin de sa durée de vie.
    /// (Nécessite tickerType="Rare" sur le ThingDef pour que CompTickRare tourne.)
    /// </summary>
    public class CompSymbiotePerishable : ThingComp
    {
        private int ageTicks;

        public CompProperties_SymbiotePerishable Props => (CompProperties_SymbiotePerishable)props;

        private int LifespanTicks => Mathf.RoundToInt(Props.lifespanDays * GenDate.TicksPerDay);

        public int TicksRemaining => Mathf.Max(0, LifespanTicks - ageTicks);

        public override void CompTickRare()
        {
            base.CompTickRare();
            ageTicks += GenTicks.TickRareInterval;
            if (ageTicks >= LifespanTicks && !parent.Destroyed)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ageTicks, "SG_symbioteAgeTicks", 0);
        }

        public override string CompInspectStringExtra()
        {
            return Props.survivalLabel + " : " + TicksRemaining.ToStringTicksToPeriod();
        }
    }
}
