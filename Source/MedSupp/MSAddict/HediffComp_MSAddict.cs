using System;
using RimWorld;
using Verse;

namespace MSAddict;

public class HediffComp_MSAddict : HediffComp
{
    public HediffCompProperties_MSAddict MSProps => (HediffCompProperties_MSAddict)props;

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
        if (Chemicals is not { Count: > 0 })
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

    public override string CompDebugString()
    {
        return
            $"addiction loss per hr: {MSProps.AddictionLossPerHour:F3}\ntolerance loss per hr: {MSProps.ToleranceLossPerHour:F3}";
    }
}