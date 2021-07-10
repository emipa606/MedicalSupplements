using System;
using RimWorld;
using Verse;

namespace MSAddict
{
    // Token: 0x02000028 RID: 40
    public class HediffComp_MSAddict : HediffComp
    {
        // Token: 0x17000013 RID: 19
        // (get) Token: 0x060000C1 RID: 193 RVA: 0x0000950A File Offset: 0x0000770A
        public HediffCompProperties_MSAddict MSProps => (HediffCompProperties_MSAddict) props;

        // Token: 0x060000C2 RID: 194 RVA: 0x00009518 File Offset: 0x00007718
        public override void CompPostTick(ref float severityAdjustment)
        {
            if ((Find.TickManager.TicksGame + Pawn.HashOffset()) % 2500 != 0)
            {
                return;
            }

            var Chemicals = DefDatabase<ChemicalDef>.AllDefsListForReading;
            var pawn = Pawn;
            HediffSet hediffSet;
            if (pawn == null)
            {
                hediffSet = null;
            }
            else
            {
                var health = pawn.health;
                hediffSet = health?.hediffSet;
            }

            var set = hediffSet;
            if (Chemicals == null || Chemicals.Count <= 0)
            {
                return;
            }

            foreach (var Chemical in Chemicals)
            {
                if (Chemical.defName == "MSMental")
                {
                    continue;
                }

                var addiction = Chemical.addictionHediff;
                if (addiction != null && set != null)
                {
                    var chkaddiction = set.GetFirstHediffOfDef(addiction);
                    if (chkaddiction != null)
                    {
                        ReduceHediff(chkaddiction, MSProps.AddictionLossPerHour);
                    }
                }

                var tolerance = Chemical.toleranceHediff;
                if (tolerance == null || set == null)
                {
                    continue;
                }

                var chktolerance = set.GetFirstHediffOfDef(tolerance);
                if (chktolerance != null)
                {
                    ReduceHediff(chktolerance, MSProps.ToleranceLossPerHour);
                }
            }
        }

        // Token: 0x060000C3 RID: 195 RVA: 0x00009640 File Offset: 0x00007840
        public void ReduceHediff(Hediff h, float sev)
        {
            var hsev = h.Severity;
            if (h.def.defName == "LuciferiumAddiction")
            {
                sev = Math.Max(0.005f, sev / 2f);
            }

            if (sev >= hsev)
            {
                Pawn.health.RemoveHediff(h);
                return;
            }

            h.Severity -= sev;
        }

        // Token: 0x060000C4 RID: 196 RVA: 0x000096A2 File Offset: 0x000078A2
        public override string CompDebugString()
        {
            return "addiction loss per hr: " + MSProps.AddictionLossPerHour.ToString("F3") +
                   "\ntolerance loss per hr: " + MSProps.ToleranceLossPerHour.ToString("F3");
        }
    }
}