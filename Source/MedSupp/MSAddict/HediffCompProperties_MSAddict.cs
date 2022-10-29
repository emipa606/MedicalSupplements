using Verse;

namespace MSAddict;

public class HediffCompProperties_MSAddict : HediffCompProperties
{
    public float AddictionLossPerHour;

    public float ToleranceLossPerHour;

    public HediffCompProperties_MSAddict()
    {
        compClass = typeof(HediffComp_MSAddict);
    }
}