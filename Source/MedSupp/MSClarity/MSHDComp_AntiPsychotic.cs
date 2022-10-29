using Verse;

namespace MSClarity;

public class MSHDComp_AntiPsychotic : HediffComp_Disappears
{
    public MSHDCompProperties_AntiPsychotic GetProps()
    {
        return (MSHDCompProperties_AntiPsychotic)props;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksToDisappear--;
        if (ticksToDisappear <= 0)
        {
            return;
        }

        var MShedSet = parent.pawn.health.hediffSet;
        if (MShedSet == null)
        {
            return;
        }

        if (Def.defName is "CatatonicBreakdown" or "PsychicShock" &&
            MShedSet.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High")) != null)
        {
            ticksToDisappear--;
        }

        if (Def.defName != "Unease" && Def.defName != "SuicidePreparation")
        {
            return;
        }

        var MSCheckRimzac = MShedSet.GetFirstHediffOfDef(HediffDef.Named("MSRimzac_High"));
        if (MShedSet.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High")) != null || MSCheckRimzac != null)
        {
            ticksToDisappear--;
        }
    }

    public override void CompPostMerged(Hediff other)
    {
        base.CompPostMerged(other);
        var MSCD = other.TryGetComp<MSHDComp_AntiPsychotic>();
        if (MSCD != null && MSCD.ticksToDisappear > ticksToDisappear)
        {
            ticksToDisappear = MSCD.ticksToDisappear;
        }
    }
}