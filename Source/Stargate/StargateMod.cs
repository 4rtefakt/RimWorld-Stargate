using Verse;

namespace Stargate
{
    /// <summary>
    /// Point d'entrée du mod. Instancié par RimWorld au chargement.
    /// Phase 0 : se contente de loguer pour valider que l'assembly se charge et s'exécute.
    /// (Harmony sera ajouté plus tard, quand un vrai patch sera nécessaire.)
    /// </summary>
    public class StargateMod : Mod
    {
        public StargateMod(ModContentPack content) : base(content)
        {
            Log.Message("[Stargate] Assembly chargée — squelette Phase 0 OK.");
        }
    }
}
