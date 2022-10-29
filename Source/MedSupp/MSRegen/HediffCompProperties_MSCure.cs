using Verse;

namespace MSRegen;

public class HediffCompProperties_MSCure : HediffCompProperties
{
    public float CureHoursMax;

    public float CureHoursMin;

    public HediffCompProperties_MSCure()
    {
        compClass = typeof(HediffComp_MSCure);
    }
}