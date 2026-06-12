using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Stargate
{
    [DefOf]
    public static class SG_DefOf
    {
        public static XenotypeDef SG_Goauld;
        public static HediffDef SG_Primta;
        public static ThingDef SG_GoauldLarva;
        public static ThingDef SG_GoauldSymbiote;

        static SG_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SG_DefOf));
        }
    }

    public static class GoauldUtility
    {
        /// <summary>Transforme un pion en hôte Goa'uld : remplace ses xénogènes
        /// par ceux du xénotype Goa'uld et fixe son identité de xénotype.</summary>
        public static void MakeGoauld(Pawn pawn)
        {
            if (pawn?.genes == null) return;
            XenotypeDef xeno = SG_DefOf.SG_Goauld;

            foreach (Gene g in pawn.genes.Xenogenes.ToList())
            {
                pawn.genes.RemoveGene(g);
            }
            foreach (GeneDef gd in xeno.genes)
            {
                pawn.genes.AddGene(gd, true);
            }
            pawn.genes.SetXenotypeDirect(xeno);
        }
    }

    /// <summary>Implante une larve goa'uld dans un porteur (Jaffa) : hediff prim'ta.</summary>
    public class Recipe_SG_ImplantPrimta : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn.health.hediffSet.HasHediff(SG_DefOf.SG_Primta)) return;
            pawn.health.AddHediff(SG_DefOf.SG_Primta);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null && !p.health.hediffSet.HasHediff(SG_DefOf.SG_Primta);
        }
    }

    /// <summary>Extrait le symbiote d'un porteur. Mûr -> symbiote adulte, sinon larve.</summary>
    public class Recipe_SG_ExtractSymbiote : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff h = pawn.health.hediffSet.GetFirstHediffOfDef(SG_DefOf.SG_Primta);
            if (h == null) return;
            bool mature = h.Severity >= 1f;
            pawn.health.RemoveHediff(h);
            ThingDef productDef = mature ? SG_DefOf.SG_GoauldSymbiote : SG_DefOf.SG_GoauldLarva;
            Thing product = ThingMaker.MakeThing(productDef);
            GenPlace.TryPlaceThing(product, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null && p.health.hediffSet.HasHediff(SG_DefOf.SG_Primta);
        }
    }

    /// <summary>Implante un symbiote adulte dans un hôte humain : le transforme en Goa'uld.</summary>
    public class Recipe_SG_ImplantGoauld : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            GoauldUtility.MakeGoauld(pawn);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null && p.genes != null && p.genes.Xenotype != SG_DefOf.SG_Goauld;
        }
    }
}
