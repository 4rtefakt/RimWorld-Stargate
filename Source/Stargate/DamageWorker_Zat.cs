using RimWorld;
using Verse;

namespace Stargate
{
    /// <summary>
    /// Décharge de zat'nik'tel, fidèle à la série :
    ///   - 1er tir sur un être vivant : choc nerveux (hediff SG_ZatShock) qui le rend
    ///     inconscient, sans blessure ;
    ///   - tir sur un être encore sous le choc : arrêt cardiaque, mort ;
    ///   - mécanoïdes et bâtiments paralysables : paralysie façon IEM (règles vanilla,
    ///     adaptation des mécanoïdes comprise).
    /// Aucune blessure n'est infligée : le montant de dégâts ne sert qu'à la durée de paralysie IEM.
    /// </summary>
    public class DamageWorker_Zat : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageResult result = new DamageResult();

            if (victim is Pawn pawn)
            {
                if (pawn.Dead)
                {
                    return result;
                }
                if (!pawn.RaceProps.IsFlesh)
                {
                    pawn.stances?.stunner?.Notify_DamageApplied(AsEmp(dinfo));
                    result.stunned = true;
                    return result;
                }

                if (pawn.health.hediffSet.HasHediff(SG_DefOf.SG_ZatShock))
                {
                    // Seconde décharge sur un système nerveux déjà saturé.
                    pawn.Kill(dinfo);
                    return result;
                }

                Hediff shock = HediffMaker.MakeHediff(SG_DefOf.SG_ZatShock, pawn);
                pawn.health.AddHediff(shock, null, dinfo);
                result.AddHediff(shock);
                result.stunned = true;
                return result;
            }

            CompStunnable stunnable = victim.TryGetComp<CompStunnable>();
            if (stunnable != null)
            {
                stunnable.StunHandler.Notify_DamageApplied(AsEmp(dinfo));
                result.stunned = true;
            }
            return result;
        }

        private static DamageInfo AsEmp(DamageInfo dinfo)
        {
            return new DamageInfo(DamageDefOf.EMP, dinfo.Amount, 0f, dinfo.Angle, dinfo.Instigator,
                dinfo.HitPart, dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown, dinfo.IntendedTarget);
        }
    }
}
