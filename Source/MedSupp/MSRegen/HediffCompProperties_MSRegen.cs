using Verse;

namespace MSRegen
{
    // Token: 0x0200000B RID: 11
    public class HediffCompProperties_MSRegen : HediffCompProperties
    {
        // Token: 0x0400001F RID: 31
        public int RegenHealVal;

        // Token: 0x0400001E RID: 30
        public int RegenHoursMax;

        // Token: 0x0400001D RID: 29
        public int RegenHoursMin;

        // Token: 0x0600002B RID: 43 RVA: 0x00003E84 File Offset: 0x00002084
        public HediffCompProperties_MSRegen()
        {
            compClass = typeof(HediffComp_MSRegen);
        }
    }
}