using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stargate
{
    /// <summary>
    /// La Porte des étoiles. Portail (MapPortal vanilla) vers une planète lointaine :
    /// la première traversée « compose une adresse » et génère la planète (carte-poche),
    /// avec une porte de retour à l'arrivée. L'interface d'envoi (pions, objets), le
    /// chargement et le retour sont ceux des portails vanilla.
    ///   - Il faut un DHD (dispositif de composition) à proximité et de l'énergie.
    ///   - « Composer une nouvelle adresse » referme la planète actuelle (si personne
    ///     du joueur n'y est resté) : la prochaine traversée mènera à un nouveau monde.
    /// </summary>
    [StaticConstructorOnStartup]
    public class Building_Stargate : MapPortal
    {
        public const float DhdRange = 12f;

        private static readonly Texture2D EnterIcon = ContentFinder<Texture2D>.Get("UI/Commands/SG_StargateEnter");
        private static readonly Texture2D DialIcon = ContentFinder<Texture2D>.Get("UI/Commands/SG_StargateDial");

        protected override Texture2D EnterTex => EnterIcon;

        public override string EnterString => "SG_StargateEnter".Translate();

        public override string CancelEnterString => "SG_StargateCancelEnter".Translate();

        public override string EnteringString => "SG_StargateEntering".Translate();

        private CompPowerTrader Power => GetComp<CompPowerTrader>();

        public bool HasDhdNearby => Map.listerThings.ThingsOfDef(SG_DefOf.SG_DHD)
            .Any(dhd => dhd.Position.InHorDistOf(Position, DhdRange));

        public override bool IsEnterable(out string reason)
        {
            if (!HasDhdNearby)
            {
                reason = "SG_StargateNoDHD".Translate();
                return false;
            }
            if (Power != null && !Power.PowerOn)
            {
                reason = "SG_StargateNoPower".Translate();
                return false;
            }
            return base.IsEnterable(out reason);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (Faction != Faction.OfPlayer)
            {
                yield break;
            }

            var dial = new Command_Action
            {
                defaultLabel = "SG_StargateDialNew".Translate(),
                defaultDesc = "SG_StargateDialNewDesc".Translate(),
                icon = DialIcon,
                action = ConfirmDialNewAddress
            };
            if (!PocketMapExists)
            {
                dial.Disable("SG_StargateNoPlanetYet".Translate());
            }
            else if (PocketMap.mapPawns.AnyPawnBlockingMapRemoval)
            {
                dial.Disable("SG_StargatePawnsStillThere".Translate());
            }
            yield return dial;
        }

        private void ConfirmDialNewAddress()
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                "SG_StargateDialNewConfirm".Translate(), DialNewAddress, destructive: true));
        }

        /// <summary>Referme la planète actuelle : la prochaine traversée en générera une nouvelle.</summary>
        private void DialNewAddress()
        {
            if (!PocketMapExists || PocketMap.mapPawns.AnyPawnBlockingMapRemoval)
            {
                return;
            }
            PocketMapUtility.DestroyPocketMap(PocketMap);
            pocketMap = null;
            exit = null;
            beenEntered = false;
            Messages.Message("SG_StargateAddressCleared".Translate(), this, MessageTypeDefOf.NeutralEvent, false);
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string status = !HasDhdNearby ? "SG_StargateNoDHD".Translate().ToString()
                : PocketMapExists ? "SG_StargateConnected".Translate().ToString()
                : "SG_StargateIdle".Translate().ToString();
            return text.NullOrEmpty() ? status : text + "\n" + status;
        }
    }
}
