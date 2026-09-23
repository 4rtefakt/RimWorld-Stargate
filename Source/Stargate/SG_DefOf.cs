using RimWorld;
using Verse;

namespace Stargate
{
    [DefOf]
    public static class SG_DefOf
    {
        // Xénotypes
        public static XenotypeDef SG_Goauld;
        public static XenotypeDef SG_GoauldQueen;
        public static XenotypeDef SG_Tokra;
        public static XenotypeDef SG_TokraQueen;
        public static XenotypeDef SG_Wraith;

        // Xénotype vanilla (utilisé pour exclure les porteurs de symbiote)
        public static XenotypeDef Sanguophage;

        // Hediffs
        public static HediffDef SG_Primta;
        public static HediffDef SG_LifeDrained;
        public static HediffDef SG_ZatShock;
        public static HediffDef SG_SymbioteWithdrawal;
        public static HediffDef SG_TretoninHigh;

        // Gènes
        public static GeneDef SG_JaffaPouch;

        // Items (symbiotes)
        public static ThingDef SG_GoauldLarva;
        public static ThingDef SG_GoauldSymbiote;
        public static ThingDef SG_TokraSymbiote;

        // Bâtiments
        public static ThingDef SG_Sarcophagus;

        // Factions
        public static FactionDef SG_SystemLords;

        // Générateurs de noms
        public static RulePackDef SG_NamerHatak;

        static SG_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SG_DefOf));
        }
    }
}
