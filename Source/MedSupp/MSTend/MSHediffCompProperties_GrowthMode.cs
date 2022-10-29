using Verse;

namespace MSTend;

public class MSHediffCompProperties_GrowthMode : HediffCompProperties
{
    public float severityPerDayGrowing;

    public FloatRange severityPerDayGrowingRandomFactor = new FloatRange(1f, 1f);

    public float severityPerDayRemission;

    public FloatRange severityPerDayRemissionRandomFactor = new FloatRange(1f, 1f);

    public MSHediffCompProperties_GrowthMode()
    {
        compClass = typeof(MSHediffComp_GrowthMode);
    }
}