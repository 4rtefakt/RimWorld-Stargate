using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Gène de la Reine Goa'uld : produit périodiquement une larve de symbiote
    /// si la reine est vivante, en forme et nourrie, et tant que le nombre de
    /// larves déjà présentes sur la carte reste sous un plafond.
    /// </summary>
    public class Gene_GoauldBrooder : Gene
    {
        private int ticksToNextLarva = -1;

        private const int ProductionIntervalTicks = 480000; // ~8 jours
        private const int MaxLarvaeOnMap = 6;

        public override void Tick()
        {
            base.Tick();
            if (pawn == null || !pawn.Spawned)
            {
                return;
            }
            if (ticksToNextLarva < 0)
            {
                ticksToNextLarva = ProductionIntervalTicks;
            }
            ticksToNextLarva--;
            if (ticksToNextLarva <= 0)
            {
                ticksToNextLarva = ProductionIntervalTicks;
                TryProduceLarva();
            }
        }

        private void TryProduceLarva()
        {
            if (pawn.Dead || pawn.Downed)
            {
                return;
            }
            if (pawn.needs?.food != null && pawn.needs.food.CurLevelPercentage < 0.3f)
            {
                return;
            }
            Map map = pawn.MapHeld;
            if (map == null)
            {
                return;
            }
            if (map.listerThings.ThingsOfDef(SG_DefOf.SG_GoauldLarva).Count >= MaxLarvaeOnMap)
            {
                return;
            }
            Thing larva = ThingMaker.MakeThing(SG_DefOf.SG_GoauldLarva);
            GenPlace.TryPlaceThing(larva, pawn.PositionHeld, map, ThingPlaceMode.Near);
            Messages.Message("Une reine goa'uld a engendré une nouvelle larve de symbiote.",
                larva, MessageTypeDefOf.PositiveEvent, false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToNextLarva, "SG_ticksToNextLarva", -1);
        }
    }
}
