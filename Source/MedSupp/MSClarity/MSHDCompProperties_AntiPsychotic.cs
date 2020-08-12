using System;
using Verse;

namespace MSClarity
{
	// Token: 0x02000024 RID: 36
	public class MSHDCompProperties_AntiPsychotic : HediffCompProperties
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x00009112 File Offset: 0x00007312
		public MSHDCompProperties_AntiPsychotic()
		{
			this.compClass = typeof(MSHDComp_AntiPsychotic);
		}

		// Token: 0x04000066 RID: 102
		public IntRange disappearsAfterTicks;
	}
}
