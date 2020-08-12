using System;
using Verse;

namespace MSAddict
{
	// Token: 0x02000027 RID: 39
	public class HediffCompProperties_MSAddict : HediffCompProperties
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x000094F2 File Offset: 0x000076F2
		public HediffCompProperties_MSAddict()
		{
			this.compClass = typeof(HediffComp_MSAddict);
		}

		// Token: 0x04000067 RID: 103
		public float AddictionLossPerHour;

		// Token: 0x04000068 RID: 104
		public float ToleranceLossPerHour;
	}
}
