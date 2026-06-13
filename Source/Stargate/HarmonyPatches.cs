using HarmonyLib;
using System.Linq;
using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Force le genre de certains xénotypes Stargate (reines goa'uld/tok'ra = femelle,
    /// Wraith = mâle), de façon ROBUSTE :
    ///   - Prefix : si le genre forcé est déterminable depuis la requête (xénotype forcé,
    ///     ou xénotype imposé par le pawnkind), on le fixe AVANT génération -> le pion naît
    ///     cohérent (tête/corps/nom du bon genre), sans rafistolage. C'est le cas des reines.
    ///   - Postfix : filet de sécurité quand le xénotype n'est résolu que pendant la
    ///     génération. On corrige genre + type de tête + nom, sinon une tête genrée
    ///     incompatible plante le rendu du portrait (ce qui cassait la barre de colons).
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static void Prefix(ref PawnGenerationRequest request)
        {
            if (request.FixedGender != null) return;
            Gender? forced = ForcedGenderFor(request.ForcedXenotype) ?? ForcedGenderForKind(request.KindDef);
            if (forced != null) request.FixedGender = forced.Value;
        }

        public static void Postfix(Pawn __result)
        {
            if (__result?.genes == null) return;

            Gender? forced = ForcedGenderFor(__result.genes.Xenotype);
            if (forced == null || __result.gender == forced.Value) return;

            __result.gender = forced.Value;

            // Type de tête genré et incompatible -> re-tirage pour le bon genre, sinon le
            // rendu du portrait peut planter et casser la barre de colons (bug du draft).
            try
            {
                if (__result.story != null)
                {
                    HeadTypeDef head = __result.story.headType;
                    if (head == null || (head.gender != Gender.None && head.gender != forced.Value))
                    {
                        var opts = DefDatabase<HeadTypeDef>.AllDefs
                            .Where(h => h.randomChosen && (h.gender == Gender.None || h.gender == forced.Value))
                            .ToList();
                        if (opts.Count > 0) __result.story.headType = opts.RandomElement();
                    }
                }
            }
            catch
            {
            }

            // Le nom a pu être généré pour l'autre genre : on le régénère.
            try
            {
                __result.Name = PawnBioAndNameGenerator.GeneratePawnName(__result);
            }
            catch
            {
            }

            // Rafraîchit les graphismes (corps/tête dépendants du genre).
            try
            {
                __result.Drawer?.renderer?.SetAllGraphicsDirty();
            }
            catch
            {
            }
        }

        private static Gender? ForcedGenderForKind(PawnKindDef kind)
        {
            if (kind?.xenotypeSet == null) return null;
            for (int i = 0; i < kind.xenotypeSet.Count; i++)
            {
                Gender? g = ForcedGenderFor(kind.xenotypeSet[i].xenotype);
                if (g != null) return g;
            }
            return null;
        }

        private static Gender? ForcedGenderFor(XenotypeDef xeno)
        {
            if (xeno == null) return null;
            if (xeno == SG_DefOf.SG_GoauldQueen || xeno == SG_DefOf.SG_TokraQueen) return Gender.Female;
            if (xeno == SG_DefOf.SG_Wraith) return Gender.Male;
            return null;
        }
    }

    /// <summary>
    /// Le sarcophage ne doit jamais servir de lit normal (sommeil/médical/prisonnier) :
    /// sinon des pions vont s'y reposer et créent une double assignation qui plante
    /// l'overlay des propriétaires. On l'exclut donc des lits utilisables. La régénération
    /// (deathrest) passe par son propre chemin (CompDeathrestBindable) et n'est pas affectée.
    /// </summary>
    [HarmonyPatch(typeof(RestUtility), nameof(RestUtility.CanUseBedEver))]
    public static class Patch_RestUtility_CanUseBedEver
    {
        public static void Postfix(ThingDef bedDef, ref bool __result)
        {
            if (__result && bedDef == SG_DefOf.SG_Sarcophagus)
            {
                __result = false;
            }
        }
    }
}
