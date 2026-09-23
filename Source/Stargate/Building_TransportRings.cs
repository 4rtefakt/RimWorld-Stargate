using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stargate
{
    /// <summary>
    /// Anneaux de transport goa'uld : téléporte instantanément tout ce qui se trouve sur la
    /// plateforme (pions, animaux, objets) vers une autre plateforme d'anneaux de la même carte.
    /// Nécessite de l'énergie ; temps de recharge entre deux activations.
    /// </summary>
    [StaticConstructorOnStartup]
    public class Building_TransportRings : Building
    {
        private const int CooldownTicks = GenDate.TicksPerHour / 2;

        private static readonly Texture2D ActivateIcon = ContentFinder<Texture2D>.Get("UI/Commands/SG_ActivateRings");

        private int lastActivationTick = -99999;

        private CompPowerTrader Power => GetComp<CompPowerTrader>();

        private int CooldownTicksLeft => Mathf.Max(0, lastActivationTick + CooldownTicks - Find.TickManager.TicksGame);

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

            var command = new Command_Action
            {
                defaultLabel = "SG_ActivateRings".Translate(),
                defaultDesc = "SG_ActivateRingsDesc".Translate(),
                icon = ActivateIcon,
                action = BeginTargeting
            };
            if (Power != null && !Power.PowerOn)
            {
                command.Disable("SG_RingsNoPower".Translate());
            }
            else if (CooldownTicksLeft > 0)
            {
                command.Disable("SG_RingsCooldown".Translate(CooldownTicksLeft.ToStringTicksToPeriod()));
            }
            else if (!OtherRings().Any())
            {
                command.Disable("SG_RingsNoDestination".Translate());
            }
            yield return command;
        }

        private IEnumerable<Building_TransportRings> OtherRings()
        {
            return Map.listerThings.ThingsOfDef(def).OfType<Building_TransportRings>().Where(r => r != this && r.Spawned);
        }

        private void BeginTargeting()
        {
            var parms = new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = true,
                canTargetItems = false,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = t => t.Thing is Building_TransportRings rings && rings != this
            };
            Find.Targeter.BeginTargeting(parms, target =>
            {
                // L'état a pu changer pendant le ciblage : on revérifie tout.
                if (target.Thing is Building_TransportRings destination && CanTransportTo(destination))
                {
                    TransportTo(destination);
                }
            });
        }

        private bool CanTransportTo(Building_TransportRings destination)
        {
            return Spawned && destination.Spawned && destination != this && destination.Map == Map
                && (Power == null || Power.PowerOn) && CooldownTicksLeft <= 0;
        }

        /// <summary>Envoie le contenu de cette plateforme vers la plateforme de destination.</summary>
        public void TransportTo(Building_TransportRings destination)
        {
            Map map = Map;
            List<Thing> cargo = this.OccupiedRect().Cells
                .SelectMany(c => c.GetThingList(map))
                .Where(t => t != this && (t is Pawn || t.def.EverHaulable))
                .Distinct()
                .ToList();

            IntVec3 offset = destination.Position - Position;
            foreach (Thing thing in cargo)
            {
                IntVec3 target = thing.Position + offset;
                if (thing is Pawn pawn)
                {
                    if (!target.Standable(map))
                    {
                        target = CellFinder.StandableCellNear(destination.Position, map, 3f);
                    }
                    if (!target.IsValid)
                    {
                        continue;
                    }
                    pawn.Position = target;
                    pawn.Notify_Teleported();
                }
                else
                {
                    IntVec3 origin = thing.Position;
                    thing.DeSpawn();
                    if (!GenPlace.TryPlaceThing(thing, target, map, ThingPlaceMode.Near))
                    {
                        // Pas de place à l'arrivée : l'objet reste sur la plateforme de départ.
                        GenPlace.TryPlaceThing(thing, origin, map, ThingPlaceMode.Near);
                    }
                }
            }

            PlayEffects(this);
            PlayEffects(destination);
            lastActivationTick = Find.TickManager.TicksGame;
            destination.lastActivationTick = lastActivationTick;
        }

        private static void PlayEffects(Building_TransportRings rings)
        {
            Map map = rings.Map;
            Vector3 center = rings.TrueCenter();
            FleckMaker.Static(center, map, FleckDefOf.PsycastSkipFlashEntry, 3f);
            FleckMaker.Static(center, map, FleckDefOf.PsycastSkipOuterRingExit, 2.5f);
            SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(rings.Position, map));
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            int left = CooldownTicksLeft;
            if (left > 0)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "SG_RingsCooldown".Translate(left.ToStringTicksToPeriod());
            }
            return text;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastActivationTick, "SG_lastRingActivationTick", -99999);
        }
    }
}
