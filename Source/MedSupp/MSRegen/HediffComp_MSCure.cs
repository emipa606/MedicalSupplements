using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSRegen
{
    // Token: 0x0200000C RID: 12
    public class HediffComp_MSCure : HediffComp
    {
        // Token: 0x04000021 RID: 33
        private bool curing;

        // Token: 0x04000020 RID: 32
        private int ticksToCure;

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600002C RID: 44 RVA: 0x00003E9C File Offset: 0x0000209C
        public HediffCompProperties_MSCure MSProps => (HediffCompProperties_MSCure) props;

        // Token: 0x0600002D RID: 45 RVA: 0x00003EAC File Offset: 0x000020AC
        public void SetTicksToCure()
        {
            var period = 2500;
            int basehours;
            if (MSProps.CureHoursMin > 0f && MSProps.CureHoursMax > 0f && MSProps.CureHoursMax >= MSProps.CureHoursMin)
            {
                basehours = (int) (Rand.Range(MSProps.CureHoursMin, MSProps.CureHoursMax) * period);
            }
            else
            {
                basehours = Rand.Range(2, 5) * period;
            }

            if (basehours < period)
            {
                basehours = period;
            }

            if (basehours > 36 * period)
            {
                basehours = 36 * period;
            }

            ticksToCure = basehours;
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003F40 File Offset: 0x00002140
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (curing && ticksToCure > 0)
            {
                ticksToCure--;
                return;
            }

            if (curing)
            {
                parent.Severity = 0f;
                if (parent != null)
                {
                    var pawn = Pawn;
                    if (pawn != null)
                    {
                        var health = pawn.health;
                        if (health != null)
                        {
                            health.RemoveHediff(parent);
                        }
                    }
                }

                Messages.Message(
                    "MSRegen.CureMsg".Translate(Pawn.LabelShort.CapitalizeFirst(), Def.label.CapitalizeFirst()), Pawn,
                    MessageTypeDefOf.PositiveEvent);
                return;
            }

            _ = new List<string>();
            List<string> Immunities;
            if (MSRegenUtility.ImmuneTo(Pawn, Def, out Immunities))
            {
                var ImmunitiesAsCure = 0;
                for (var i = 0; i < Immunities.Count; i++)
                {
                    if (Immunities[i] != "MSCondom_High")
                    {
                        ImmunitiesAsCure++;
                    }
                }

                if (ImmunitiesAsCure > 0)
                {
                    SetTicksToCure();
                    curing = true;
                }
            }
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00004058 File Offset: 0x00002258
        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksToCure, "ticksToCure");
            Scribe_Values.Look(ref curing, "curing");
        }

        // Token: 0x06000030 RID: 48 RVA: 0x0000407E File Offset: 0x0000227E
        public override string CompDebugString()
        {
            if (curing)
            {
                return "ticksToCure: " + ticksToCure;
            }

            return "No active cure.";
        }
    }
}