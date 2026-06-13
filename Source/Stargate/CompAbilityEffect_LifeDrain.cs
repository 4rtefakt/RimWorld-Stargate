using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    public class CompProperties_AbilityLifeDrain : CompProperties_AbilityEffect
    {
        // Fraction de la réserve d'essence vitale rendue au lanceur par drain.
        public float essenceGainPct = 0.5f;

        // Sévérité du hediff "vie drainée" au 1er drain, puis au 2nd (létal).
        public float firstDrainSeverity = 0.5f;
        public float secondDrainSeverity = 1.0f;

        public CompProperties_AbilityLifeDrain()
        {
            compClass = typeof(CompAbilityEffect_LifeDrain);
        }
    }

    /// <summary>
    /// Drain de vie Wraith : rend une part de l'essence vitale au lanceur et
    /// inflige à la cible un hediff "vie drainée" permanent qui s'aggrave.
    /// 1er drain -> 50 % (conscience -30 %, douleur +20 %). 2e drain -> 100 % = mort.
    /// </summary>
    public class CompAbilityEffect_LifeDrain : CompAbilityEffect
    {
        public new CompProperties_AbilityLifeDrain Props => (CompProperties_AbilityLifeDrain)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Pawn caster = parent?.pawn;
            Pawn victim = target.Pawn;
            if (victim == null) return;

            // 1) Le lanceur regagne de l'essence vitale (ressource hémogène re-skinnée).
            Gene_Hemogen hemogen = caster?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen != null)
            {
                hemogen.Value = Mathf.Min(hemogen.Max, hemogen.Value + Props.essenceGainPct * hemogen.Max);
            }

            // 2) La cible voit sa vie drainée, de façon permanente et cumulative.
            Hediff drained = victim.health.hediffSet.GetFirstHediffOfDef(SG_DefOf.SG_LifeDrained);
            if (drained == null)
            {
                drained = HediffMaker.MakeHediff(SG_DefOf.SG_LifeDrained, victim);
                drained.Severity = Props.firstDrainSeverity;
                victim.health.AddHediff(drained);
            }
            else
            {
                // 2e drain (ou plus) : pousse à la sévérité létale -> mort via lethalSeverity.
                drained.Severity = Props.secondDrainSeverity;
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn p = target.Pawn;
            if (p == null || !p.RaceProps.Humanlike)
            {
                if (throwMessages)
                {
                    Messages.Message("SG_LifeDrainInvalidTarget".Translate(), p, MessageTypeDefOf.RejectInput, false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }
    }
}
