using RimWorld;
using Verse;

namespace MedSupp
{
    // Token: 0x0200002A RID: 42
    public class HediffComp_MSWakeAnasthetic : HediffComp
    {
        // Token: 0x17000014 RID: 20
        // (get) Token: 0x060000C7 RID: 199 RVA: 0x000096FB File Offset: 0x000078FB
        public HediffCompProperties_MSWakeAnasthetic MSProps => (HediffCompProperties_MSWakeAnasthetic) props;

        // Token: 0x060000C8 RID: 200 RVA: 0x00009708 File Offset: 0x00007908
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Awake())
            {
                var health = Pawn.health;
                bool flag;
                if (health == null)
                {
                    flag = false;
                }
                else
                {
                    var capacities = health.capacities;
                    var num = capacities != null
                        ? new float?(capacities.GetLevel(PawnCapacityDefOf.Consciousness))
                        : null;
                    var num2 = 0.05f;
                    flag = (num.GetValueOrDefault() > num2) & (num != null);
                }

                if (flag && Pawn.IsHashIntervalTick(2500))
                {
                    var pawn = Pawn;
                    HediffSet hediffSet;
                    if (pawn == null)
                    {
                        hediffSet = null;
                    }
                    else
                    {
                        var health2 = pawn.health;
                        hediffSet = health2?.hediffSet;
                    }

                    var set = hediffSet;
                    if (set != null)
                    {
                        var anasthetic = set.GetFirstHediffOfDef(HediffDefOf.Anesthetic);
                        if (anasthetic != null)
                        {
                            var sev = anasthetic.Severity;
                            if (sev > 0f)
                            {
                                var sevloss = sev * MSProps.sevReduce;
                                if (sev - sevloss > 0f)
                                {
                                    anasthetic.Severity = sev - sevloss;
                                    return;
                                }

                                anasthetic.Severity = 0f;
                            }
                        }
                    }
                }
            }
        }
    }
}