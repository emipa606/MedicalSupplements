using System;
using Verse;

namespace MedSupp
{
	// Token: 0x0200002B RID: 43
	public class HediffCompProperties_MSWakeAnasthetic : HediffCompProperties
	{
		// Token: 0x060000CA RID: 202 RVA: 0x0000980D File Offset: 0x00007A0D
		public HediffCompProperties_MSWakeAnasthetic()
		{
			this.compClass = typeof(HediffComp_MSWakeAnasthetic);
		}

		// Token: 0x04000069 RID: 105
		public float sevReduce = 0.33f;
	}
}
