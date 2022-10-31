using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MSTend;

public class MSHediffComp_GrowthMode : HediffComp_SeverityModifierBase
{
    private const int CheckGrowthModeChangeInterval = 5000;

    private const float GrowthModeChangeMtbDays = 100f;

    public HediffGrowthMode growthMode;

    private float severityPerDayGrowingRandomFactor = 1f;

    private float severityPerDayRemissionRandomFactor = 1f;

    public MSHediffCompProperties_GrowthMode Props => (MSHediffCompProperties_GrowthMode)props;

    public override string CompLabelInBracketsExtra => growthMode.GetLabel();

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref growthMode, "growthMode");
        Scribe_Values.Look(ref severityPerDayGrowingRandomFactor, "severityPerDayGrowingRandomFactor", 1f);
        Scribe_Values.Look(ref severityPerDayRemissionRandomFactor, "severityPerDayRemissionRandomFactor", 1f);
    }

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        Log.Message("CompPostPostAdd override");
        base.CompPostPostAdd(dinfo);
        growthMode = ((HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))).RandomElement();
        severityPerDayGrowingRandomFactor = Props.severityPerDayGrowingRandomFactor.RandomInRange;
        severityPerDayRemissionRandomFactor = Props.severityPerDayRemissionRandomFactor.RandomInRange;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (Pawn.IsHashIntervalTick(5000) && Rand.MTBEventOccurs(100f, 60000f, 5000f))
        {
            ChangeGrowthMode();
        }
    }

    public override float SeverityChangePerDay()
    {
        float growthsev;
        switch (growthMode)
        {
            case HediffGrowthMode.Growing:
                growthsev = Props.severityPerDayGrowing * severityPerDayGrowingRandomFactor;
                break;
            case HediffGrowthMode.Stable:
                growthsev = 0f;
                break;
            case HediffGrowthMode.Remission:
                growthsev = Props.severityPerDayRemission * severityPerDayRemissionRandomFactor;
                break;
            default:
                throw new NotImplementedException("GrowthMode");
        }

        return MSAdjustment(Pawn, growthsev, growthMode);
    }

    public float MSAdjustment(Pawn p, float sev, HediffGrowthMode gm)
    {
        var newSev = sev;
        if (gm != HediffGrowthMode.Growing)
        {
            return newSev;
        }

        HediffSet hediffSet;
        if (p == null)
        {
            hediffSet = null;
        }
        else
        {
            var health = p.health;
            hediffSet = health?.hediffSet;
        }

        var hSet = hediffSet;
        if (hSet == null)
        {
            return newSev;
        }

        var drugDef = DefDatabase<HediffDef>.GetNamed("MSVinacol_High", false);
        if (drugDef != null && hSet.GetFirstHediffOfDef(drugDef) != null)
        {
            newSev *= Rand.Range(0f, 0.1f);
        }

        return newSev;
    }

    private void ChangeGrowthMode()
    {
        growthMode = (from x in (HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))
            where x != growthMode
            select x).RandomElement();
        if (!PawnUtility.ShouldSendNotificationAbout(Pawn))
        {
            return;
        }

        switch (growthMode)
        {
            case HediffGrowthMode.Growing:
                Messages.Message(
                    "DiseaseGrowthModeChanged_Growing".Translate(Pawn.LabelShort, Def.label,
                        Pawn.Named("PAWN")), Pawn, MessageTypeDefOf.NegativeHealthEvent);
                return;
            case HediffGrowthMode.Stable:
                Messages.Message(
                    "DiseaseGrowthModeChanged_Stable".Translate(Pawn.LabelShort, Def.label, Pawn.Named("PAWN")),
                    Pawn, MessageTypeDefOf.NeutralEvent);
                return;
            case HediffGrowthMode.Remission:
                Messages.Message(
                    "DiseaseGrowthModeChanged_Remission".Translate(Pawn.LabelShort, Def.label,
                        Pawn.Named("PAWN")), Pawn, MessageTypeDefOf.PositiveEvent);
                break;
            default:
                return;
        }
    }

    public override string CompDebugString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.CompDebugString());
        stringBuilder.AppendLine(
            $"severity: {parent.Severity:F3}{(parent.Severity < Def.maxSeverity ? string.Empty : " (reached max)")}");
        stringBuilder.AppendLine($"severityPerDayGrowingRandomFactor: {severityPerDayGrowingRandomFactor:0.##}");
        stringBuilder.AppendLine($"severityPerDayRemissionRandomFactor: {severityPerDayRemissionRandomFactor:0.##}");
        return stringBuilder.ToString();
    }
}