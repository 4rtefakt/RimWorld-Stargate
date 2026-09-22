using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VanillaGravshipExpanded2;
using VEF.Storyteller;

namespace Stargate
{
    /// <summary>
    /// Génère la carte orbitale d'un Ha'tak en approche : même procédé que le gravship ennemi
    /// de VGE Ch.2 (structure atterrissant au centre de la carte, assemblée à partir d'un
    /// StructureSetDef), mais avec la faction goa'uld et son équipage.
    /// La structure est configurable en XML (<see cref="structureSetDef"/>), pour pouvoir
    /// brancher des plans de Ha'tak dédiés sans toucher au code.
    /// </summary>
    public class GenStep_Hatak : GenStep_SpaceEncounter
    {
        public StructureSetDef structureSetDef;
        public IntRange pawnCountRange = new IntRange(10, 16);

        public override int SeedPart => 482915377;

        protected override void GenerateSpaceMap(Map map, GenStepParams parms)
        {
            Faction faction = GravshipThreatWorker_Hatak.GoauldFaction ?? Faction.OfSalvagers;
            MapParent parent = map.Parent;
            if (parent.Faction == null || parent.Faction == Faction.OfPlayer)
            {
                parent.SetFaction(faction);
            }

            StructureSetDef set = structureSetDef ?? InternalDefOf.VGE_EnemyGravshipSet;
            var landing = (LandingStructure_StructureSet)ThingMaker.MakeThing(InternalDefOf.VGE_LandingStructure_EnemyGravship);
            landing.structureSetDef = set;
            landing.selectedDefs = StructureSetGenerator.SelectStandardLayouts(set).Select(x => x.def).ToList();
            landing.shipRotation = Rot4.Random;
            landing.shipFaction = faction;
            landing.pawnCountRange = pawnCountRange;
            GenSpawn.Spawn(landing, map.Center, map);
        }
    }
}
