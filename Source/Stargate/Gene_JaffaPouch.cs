using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Poche à symbiote des Jaffa. Le système immunitaire d'un Jaffa adulte dépend
    /// entièrement de la larve goa'uld (prim'ta) qu'il porte, ou à défaut de tretonine.
    ///   - Un Jaffa qui acquiert ce gène porte déjà une larve (pions générés, colons
    ///     existants, xénogerme) : elle lui est posée au premier contrôle.
    ///   - Sans prim'ta ni tretonine active : « manque de symbiote », mortel en ~3 jours.
    ///   - Dès qu'une larve est réimplantée ou qu'une dose de tretonine est prise, le manque cesse.
    /// Contrôle toutes les heures de jeu (TickInterval, compatible avec l'échelonnage 1.6).
    /// </summary>
    public class Gene_JaffaPouch : Gene
    {
        private const int CheckIntervalTicks = GenDate.TicksPerHour;

        private bool initialized;
        private int ticksSinceCheck;

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            ticksSinceCheck += delta;
            if (ticksSinceCheck < CheckIntervalTicks)
            {
                return;
            }
            ticksSinceCheck = 0;
            CheckSymbiote();
        }

        public static bool IsSustained(Pawn pawn)
        {
            HediffSet set = pawn.health.hediffSet;
            return set.HasHediff(SG_DefOf.SG_Primta) || set.HasHediff(SG_DefOf.SG_TretoninHigh);
        }

        private void CheckSymbiote()
        {
            if (pawn == null || pawn.Dead || !Active)
            {
                return;
            }

            if (!initialized)
            {
                initialized = true;
                if (!IsSustained(pawn))
                {
                    Hediff primta = HediffMaker.MakeHediff(SG_DefOf.SG_Primta, pawn);
                    primta.Severity = Rand.Range(0.05f, 0.9f);
                    pawn.health.AddHediff(primta);
                }
            }

            Hediff withdrawal = pawn.health.hediffSet.GetFirstHediffOfDef(SG_DefOf.SG_SymbioteWithdrawal);
            if (IsSustained(pawn))
            {
                if (withdrawal != null)
                {
                    pawn.health.RemoveHediff(withdrawal);
                }
            }
            else if (withdrawal == null)
            {
                pawn.health.AddHediff(SG_DefOf.SG_SymbioteWithdrawal);
                if (pawn.Faction == Faction.OfPlayer)
                {
                    Messages.Message("SG_SymbioteWithdrawalStarted".Translate(pawn.LabelShort),
                        pawn, MessageTypeDefOf.NegativeHealthEvent);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref initialized, "SG_pouchInitialized", false);
            Scribe_Values.Look(ref ticksSinceCheck, "SG_pouchTicksSinceCheck", 0);
        }
    }
}
