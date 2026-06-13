using HarmonyLib;
using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Force le genre de certains xénotypes Stargate, quel que soit le moyen de
    /// génération (scénario, faction, conversion, dev mode) :
    ///   - Reine goa'uld et reine tok'ra : toujours femelle.
    ///   - Wraith : toujours mâle.
    /// On patche le point d'entrée public et stable PawnGenerator.GeneratePawn,
    /// puis on corrige genre + nom + graphismes après coup.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static void Postfix(Pawn __result)
        {
            if (__result?.genes == null) return;

            Gender? forced = ForcedGenderFor(__result.genes.Xenotype);
            if (forced == null || __result.gender == forced.Value) return;

            __result.gender = forced.Value;

            // Le nom a pu être généré pour l'autre genre : on le régénère pour rester cohérent.
            try
            {
                __result.Name = PawnBioAndNameGenerator.GeneratePawnName(__result);
            }
            catch
            {
                // Nom non régénérable (ex. pion sans bio) : on garde l'ancien, sans bloquer.
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

        private static Gender? ForcedGenderFor(XenotypeDef xeno)
        {
            if (xeno == null) return null;
            if (xeno == SG_DefOf.SG_GoauldQueen || xeno == SG_DefOf.SG_TokraQueen) return Gender.Female;
            if (xeno == SG_DefOf.SG_Wraith) return Gender.Male;
            return null;
        }
    }
}
