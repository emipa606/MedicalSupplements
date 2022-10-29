using Verse;

namespace MSClarity;

public class MSHDCompProperties_AntiPsychotic : HediffCompProperties
{
    public IntRange disappearsAfterTicks;

    public MSHDCompProperties_AntiPsychotic()
    {
        compClass = typeof(MSHDComp_AntiPsychotic);
    }
}