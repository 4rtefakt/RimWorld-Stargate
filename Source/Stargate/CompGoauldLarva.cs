using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    public class CompProperties_GoauldLarva : CompProperties
    {
        // Durée de vie d'une larve hors hôte avant qu'elle ne périsse.
        public float lifespanDays = 5f;

        public CompProperties_GoauldLarva()
        {
            compClass = typeof(CompGoauldLarva);
        }
    }

    /// <summary>
    /// Larve Goa'uld fragile : une fois hors d'un hôte, elle meurt si elle n'est
    /// pas implantée dans un Jaffa avant la fin de sa durée de vie.
    /// (Nécessite tickerType="Rare" sur le ThingDef pour que CompTickRare tourne.)
    /// </summary>
    public class CompGoauldLarva : ThingComp
    {
        private int ageTicks;

        public CompProperties_GoauldLarva Props => (CompProperties_GoauldLarva)props;

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
            Scribe_Values.Look(ref ageTicks, "SG_larvaAgeTicks", 0);
        }

        public override string CompInspectStringExtra()
        {
            return "Survie restante : " + TicksRemaining.ToStringTicksToPeriod();
        }
    }
}
