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
    ///   - Non démontable tant qu'un monde est relié ; si elle est détruite, le vortex
    ///     ramène les pions du joueur restés là-bas puis le monde se referme.
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

        public bool HasDhdNearby => Spawned && Map.listerThings.ThingsOfDef(SG_DefOf.SG_DHD)
            .Any(dhd => dhd.Position.InHorDistOf(Position, DhdRange));

        /// <summary>Pions du joueur (colons, animaux, prisonniers, y compris à terre) sur le monde relié.</summary>
        private bool PlayerPawnsOnPlanet => PocketMapExists && PocketMap.mapPawns.AllPawns
            .Any(p => p.Faction == Faction.OfPlayer || p.HostFaction == Faction.OfPlayer);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (pocketMap == null)
            {
                return;
            }
            if (!Find.Maps.Contains(pocketMap))
            {
                // Le monde a été retiré entre-temps (carte d'origine abandonnée) : on repart de zéro.
                ForgetPlanet();
            }
            else if (pocketMap.PocketMapParent != null)
            {
                // Porte déplacée avec un gravship : le monde reste rattaché à la carte courante.
                pocketMap.PocketMapParent.sourceMap = map;
            }
        }

        public override AcceptanceReport DeconstructibleBy(Faction faction)
        {
            if (PocketMapExists)
            {
                return "SG_StargateStillConnected".Translate();
            }
            return base.DeconstructibleBy(faction);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode != DestroyMode.WillReplace && Spawned && PocketMapExists)
            {
                CollapsePlanet();
            }
            base.Destroy(mode);
        }

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
            else if (PlayerPawnsOnPlanet)
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
            if (!PocketMapExists || PlayerPawnsOnPlanet)
            {
                return;
            }
            PocketMapUtility.DestroyPocketMap(PocketMap);
            ForgetPlanet();
            Messages.Message("SG_StargateAddressCleared".Translate(), this, MessageTypeDefOf.NeutralEvent, false);
        }

        /// <summary>
        /// La porte est détruite alors qu'un monde est relié : les pions du joueur restés là-bas
        /// sont ramenés près de la porte, puis le monde se referme (ce qui y reste est perdu).
        /// </summary>
        private void CollapsePlanet()
        {
            Map home = Map;
            foreach (Pawn pawn in PocketMap.mapPawns.AllPawnsSpawned.ToList())
            {
                if (pawn.Faction != Faction.OfPlayer && pawn.HostFaction != Faction.OfPlayer)
                {
                    continue;
                }
                IntVec3 cell = CellFinder.StandableCellNear(Position, home, 8f);
                if (!cell.IsValid)
                {
                    cell = Position;
                }
                pawn.DeSpawn();
                GenSpawn.Spawn(pawn, cell, home);
            }
            Messages.Message("SG_StargateCollapsed".Translate(), new TargetInfo(Position, home), MessageTypeDefOf.NegativeEvent);
            PocketMapUtility.DestroyPocketMap(PocketMap);
            ForgetPlanet();
        }

        private void ForgetPlanet()
        {
            pocketMap = null;
            exit = null;
            beenEntered = false;
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
