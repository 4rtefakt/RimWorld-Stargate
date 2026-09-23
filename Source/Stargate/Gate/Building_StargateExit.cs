using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    /// <summary>Porte des étoiles de la planète d'arrivée : ramène vers la porte d'origine.</summary>
    [StaticConstructorOnStartup]
    public class Building_StargateExit : PocketMapExit
    {
        private static readonly Texture2D ReturnIcon = ContentFinder<Texture2D>.Get("UI/Commands/SG_StargateEnter");

        protected override Texture2D EnterTex => ReturnIcon;

        public override string EnterString => "SG_StargateReturn".Translate();

        public override string CancelEnterString => "SG_StargateCancelEnter".Translate();
    }
}
