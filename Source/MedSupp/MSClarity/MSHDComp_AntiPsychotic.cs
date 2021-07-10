using Verse;

namespace MSClarity
{
    // Token: 0x02000025 RID: 37
    public class MSHDComp_AntiPsychotic : HediffComp_Disappears
    {
        // Token: 0x060000B6 RID: 182 RVA: 0x0000912A File Offset: 0x0000732A
        public MSHDCompProperties_AntiPsychotic GetProps()
        {
            return (MSHDCompProperties_AntiPsychotic) props;
        }

        // Token: 0x060000B7 RID: 183 RVA: 0x00009138 File Offset: 0x00007338
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

            if ((Def.defName == "CatatonicBreakdown" || Def.defName == "PsychicShock") &&
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

        // Token: 0x060000B8 RID: 184 RVA: 0x00009240 File Offset: 0x00007440
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
}