using HarmonyLib;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Point d'entrée du mod. Instancié par RimWorld au chargement.
    /// Applique les patchs Harmony (genre forcé de certains xénotypes).
    /// </summary>
    public class StargateMod : Mod
    {
        public StargateMod(ModContentPack content) : base(content)
        {
            new Harmony("4rtefakt.stargate").PatchAll();
            Log.Message("[Stargate] Assembly chargée, patchs Harmony appliqués.");
        }
    }
}
