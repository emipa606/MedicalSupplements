using RimWorld;
using Verse;

namespace MSExotic;

public class MSExoticDefOf
{
    static MSExoticDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }

    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef MSImmunisation;

        public static ThingDef MSCerebrax;

        public static ThingDef MSBattleStim;

        public static ThingDef MSPerpetuity;

        public static ThingDef MSCondom;
    }
}