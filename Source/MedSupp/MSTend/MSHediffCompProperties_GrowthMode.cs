using Verse;

namespace MSTend;

public class MSHediffCompProperties_GrowthMode : HediffCompProperties
{
    public float severityPerDayGrowing;

    public FloatRange severityPerDayGrowingRandomFactor = new(1f, 1f);

    public float severityPerDayRemission;

    public FloatRange severityPerDayRemissionRandomFactor = new(1f, 1f);

    public MSHediffCompProperties_GrowthMode()
    {
        compClass = typeof(MSHediffComp_GrowthMode);
    }
}