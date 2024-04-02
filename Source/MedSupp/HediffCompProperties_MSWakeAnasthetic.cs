using Verse;

namespace MedSupp;

public class HediffCompProperties_MSWakeAnasthetic : HediffCompProperties
{
    public readonly float sevReduce = 0.33f;

    public HediffCompProperties_MSWakeAnasthetic()
    {
        compClass = typeof(HediffComp_MSWakeAnasthetic);
    }
}