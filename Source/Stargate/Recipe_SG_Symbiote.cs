using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Stargate
{
    public static class GoauldUtility
    {
        /// <summary>
        /// Convertit un pion en hôte d'un symbiote : retire TOUS ses gènes
        /// (endogènes + xénogènes) puis applique le xénotype cible. Évite que
        /// les gènes d'origine de l'hôte entrent en conflit avec ceux du symbiote.
        /// </summary>
        public static void ConvertXenotype(Pawn pawn, XenotypeDef xeno)
        {
            if (pawn?.genes == null || xeno == null) return;

            foreach (Gene g in pawn.genes.GenesListForReading.ToList())
            {
                pawn.genes.RemoveGene(g);
            }
            foreach (GeneDef gd in xeno.genes)
            {
                // Xénotype non héritable -> gènes posés en xénogènes (comme Sanguophage).
                pawn.genes.AddGene(gd, !xeno.inheritable);
            }
            pawn.genes.SetXenotypeDirect(xeno);
        }

        /// <summary>Un pion porte-t-il déjà un symbiote (ou en est déjà un) ?
        /// Goa'uld, Reine Goa'uld, Tok'ra, Reine Tok'ra et Sanguophage sont exclus
        /// comme hôtes d'un nouvel implant.</summary>
        public static bool AlreadyHosted(Pawn p)
        {
            XenotypeDef x = p?.genes?.Xenotype;
            return x == SG_DefOf.SG_Goauld
                || x == SG_DefOf.SG_GoauldQueen
                || x == SG_DefOf.SG_Tokra
                || x == SG_DefOf.SG_TokraQueen
                || x == SG_DefOf.Sanguophage;
        }
    }

    /// <summary>Implante une larve goa'uld dans un Jaffa : hediff prim'ta.
    /// Recrute instantanément le sujet, même s'il est de loyauté inébranlable.</summary>
    public class Recipe_SG_ImplantPrimta : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (!pawn.health.hediffSet.HasHediff(SG_DefOf.SG_Primta))
            {
                pawn.health.AddHediff(SG_DefOf.SG_Primta);
            }

            // Le porteur d'une prim'ta est lié à vie au colon : recrutement immédiat,
            // y compris pour un prisonnier de loyauté inébranlable.
            if (pawn.Faction != Faction.OfPlayer)
            {
                if (pawn.guest != null)
                {
                    pawn.guest.Recruitable = true;
                }
                RecruitUtility.Recruit(pawn, Faction.OfPlayer, billDoer);
            }
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null
                && p.genes?.Xenotype == SG_DefOf.SG_Jaffa
                && !p.health.hediffSet.HasHediff(SG_DefOf.SG_Primta);
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

    /// <summary>Implante un symbiote goa'uld adulte dans un hôte : le transforme en Goa'uld.</summary>
    public class Recipe_SG_ImplantGoauld : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            GoauldUtility.ConvertXenotype(pawn, SG_DefOf.SG_Goauld);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null && p.genes != null && !GoauldUtility.AlreadyHosted(p);
        }
    }

    /// <summary>Implante un symbiote tok'ra adulte dans un hôte consentant : le transforme en Tok'ra.</summary>
    public class Recipe_SG_ImplantTokra : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            GoauldUtility.ConvertXenotype(pawn, SG_DefOf.SG_Tokra);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            Pawn p = thing as Pawn;
            return p != null && p.genes != null && !GoauldUtility.AlreadyHosted(p);
        }
    }
}
