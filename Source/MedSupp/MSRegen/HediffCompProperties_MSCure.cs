using Verse;

namespace MSRegen
{
    // Token: 0x0200000A RID: 10
    public class HediffCompProperties_MSCure : HediffCompProperties
    {
        // Token: 0x0400001C RID: 28
        public float CureHoursMax;

        // Token: 0x0400001B RID: 27
        public float CureHoursMin;

        // Token: 0x0600002A RID: 42 RVA: 0x00003E6C File Offset: 0x0000206C
        public HediffCompProperties_MSCure()
        {
            compClass = typeof(HediffComp_MSCure);
        }
    }
}