using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Gène de reine (goa'uld ou tok'ra) : pond périodiquement un symbiote tant
    /// que la reine est vivante, valide et nourrie, et que le plafond sur la carte
    /// n'est pas atteint. La cadence et le produit sont définis en XML via
    /// <see cref="GeneExtension_Brooder"/> (donc trouvables sans dev mode).
    /// </summary>
    public class Gene_GoauldBrooder : Gene
    {
        private int ticksToNextLarva = -1;

        // Délai de re-tentative court quand la ponte est bloquée (faim, plafond…).
        private const int RetryTicks = 2500;

        private GeneExtension_Brooder Ext => def.GetModExtension<GeneExtension_Brooder>();

        private int Interval => Ext?.productionIntervalTicks ?? 300000;

        public override void Tick()
        {
            base.Tick();
            if (pawn == null || !pawn.Spawned) return;

            if (ticksToNextLarva < 0)
            {
                ticksToNextLarva = Interval;
            }
            ticksToNextLarva--;
            if (ticksToNextLarva <= 0)
            {
                // Réussite -> on repart pour un cycle complet ; échec -> re-tentative courte.
                ticksToNextLarva = TryProduce() ? Interval : RetryTicks;
            }
        }

        private bool TryProduce()
        {
            GeneExtension_Brooder ext = Ext;
            if (ext?.product == null) return false;
            if (pawn.Dead || pawn.Downed) return false;
            if (pawn.needs?.food != null && pawn.needs.food.CurLevelPercentage < 0.3f) return false;

            Map map = pawn.MapHeld;
            if (map == null) return false;
            if (map.listerThings.ThingsOfDef(ext.product).Count >= ext.maxOnMap) return false;

            Thing produced = ThingMaker.MakeThing(ext.product);
            GenPlace.TryPlaceThing(produced, pawn.PositionHeld, map, ThingPlaceMode.Near);
            Messages.Message("SG_QueenProducedSymbiote".Translate(pawn.LabelShort, produced.LabelNoCount),
                produced, MessageTypeDefOf.PositiveEvent, false);
            return true;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            if (Prefs.DevMode)
            {
                int remaining = ticksToNextLarva < 0 ? Interval : ticksToNextLarva;
                yield return new Command_Action
                {
                    defaultLabel = "DEV : pondre maintenant",
                    defaultDesc = "Prochaine ponte dans " + Mathf.Max(0, remaining).ToStringTicksToPeriod()
                                  + " (intervalle : " + Interval.ToStringTicksToPeriod() + ").",
                    // Production immédiate (ne dépend pas du tick) pour pouvoir tester sur-le-champ.
                    action = () => { TryProduce(); ticksToNextLarva = Interval; }
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToNextLarva, "SG_ticksToNextLarva", -1);
        }
    }
}
