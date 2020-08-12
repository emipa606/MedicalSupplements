using System;
using RimWorld;
using Verse;

namespace MSExotic
{
	// Token: 0x02000017 RID: 23
	public class MSExoticDefOf
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00005700 File Offset: 0x00003900
		static MSExoticDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(MSExoticDefOf.ThingDefOf));
		}

		// Token: 0x02000034 RID: 52
		[DefOf]
		public static class ThingDefOf
		{
			// Token: 0x0400007A RID: 122
			public static ThingDef MSImmunisation;

			// Token: 0x0400007B RID: 123
			public static ThingDef MSCerebrax;

			// Token: 0x0400007C RID: 124
			public static ThingDef MSBattleStim;

			// Token: 0x0400007D RID: 125
			public static ThingDef MSPerpetuity;

			// Token: 0x0400007E RID: 126
			public static ThingDef MSCondom;
		}
	}
}
