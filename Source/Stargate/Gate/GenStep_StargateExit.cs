using System.Linq;
using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Génération d'une planète composée par la Porte : place la porte de retour
    /// (portal.exitDef du portail en cours) sur un terrain dégagé près du centre.
    /// La zone autour est déblayée pour que l'équipe arrive à découvert.
    /// </summary>
    public class GenStep_StargateExit : GenStep
    {
        private const int ClearRadius = 4;

        public override int SeedPart => 734912053;

        public override void Generate(Map map, GenStepParams parms)
        {
            MapPortal portal = PocketMapUtility.currentlyGeneratingPortal;
            ThingDef exitDef = portal?.def.portal?.exitDef;
            if (exitDef == null)
            {
                Log.Error("[Stargate] GenStep_StargateExit : aucun portail en cours de génération.");
                return;
            }

            if (!CellFinder.TryFindRandomCellNear(map.Center, map, 30, c => IsGoodSpot(c, map, exitDef), out IntVec3 spot))
            {
                spot = map.Center;
            }

            foreach (IntVec3 c in GenRadial.RadialCellsAround(spot, ClearRadius, true))
            {
                if (!c.InBounds(map))
                {
                    continue;
                }
                foreach (Thing t in c.GetThingList(map).ToArray())
                {
                    if (t.def.destroyable && (t.def.category == ThingCategory.Plant || t.def.category == ThingCategory.Building))
                    {
                        t.Destroy();
                    }
                }
                map.roofGrid.SetRoof(c, null);
            }

            GenSpawn.Spawn(ThingMaker.MakeThing(exitDef), spot, map);
        }

        private static bool IsGoodSpot(IntVec3 c, Map map, ThingDef exitDef)
        {
            CellRect rect = GenAdj.OccupiedRect(c, Rot4.North, exitDef.size).ExpandedBy(2);
            if (!rect.InBounds(map))
            {
                return false;
            }
            foreach (IntVec3 cell in rect)
            {
                TerrainDef terrain = cell.GetTerrain(map);
                if (terrain.IsWater || !terrain.affordances.Contains(TerrainAffordanceDefOf.Heavy) || cell.Roofed(map))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
