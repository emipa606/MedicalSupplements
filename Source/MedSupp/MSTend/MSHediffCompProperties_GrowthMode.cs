using System;
using Verse;

namespace MSTend
{
	// Token: 0x02000005 RID: 5
	public class MSHediffCompProperties_GrowthMode : HediffCompProperties
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000021D8 File Offset: 0x000003D8
		public MSHediffCompProperties_GrowthMode()
		{
			this.compClass = typeof(MSHediffComp_GrowthMode);
		}

		// Token: 0x04000001 RID: 1
		public float severityPerDayGrowing;

		// Token: 0x04000002 RID: 2
		public float severityPerDayRemission;

		// Token: 0x04000003 RID: 3
		public FloatRange severityPerDayGrowingRandomFactor = new FloatRange(1f, 1f);

		// Token: 0x04000004 RID: 4
		public FloatRange severityPerDayRemissionRandomFactor = new FloatRange(1f, 1f);
	}
}
