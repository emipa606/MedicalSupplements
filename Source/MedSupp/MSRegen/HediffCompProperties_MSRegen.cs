using Verse;

namespace MSRegen;

public class HediffCompProperties_MSRegen : HediffCompProperties
{
    public int RegenHealVal;

    public int RegenHoursMax;

    public int RegenHoursMin;

    public HediffCompProperties_MSRegen()
    {
        compClass = typeof(HediffComp_MSRegen);
    }
}