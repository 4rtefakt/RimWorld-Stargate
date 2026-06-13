using RimWorld;
using Verse;

namespace Stargate
{
    [DefOf]
    public static class SG_DefOf
    {
        // Xénotypes
        public static XenotypeDef SG_Jaffa;
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

        // Items (symbiotes)
        public static ThingDef SG_GoauldLarva;
        public static ThingDef SG_GoauldSymbiote;
        public static ThingDef SG_TokraSymbiote;

        static SG_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SG_DefOf));
        }
    }
}
