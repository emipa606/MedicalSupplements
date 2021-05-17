using Verse;

namespace MSExotic
{
    // Token: 0x02000014 RID: 20
    public class CompProperties_MSStimWorn : CompProperties
    {
        // Token: 0x04000051 RID: 81
        public int StimUses = 1;

        // Token: 0x0600004C RID: 76 RVA: 0x00005431 File Offset: 0x00003631
        public CompProperties_MSStimWorn()
        {
            compClass = typeof(CompMSStimWorn);
        }
    }
}