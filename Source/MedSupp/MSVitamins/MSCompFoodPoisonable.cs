using System;
using RimWorld;
using Verse;

namespace MSVitamins
{
	// Token: 0x02000002 RID: 2
	public class MSCompFoodPoisonable : CompFoodPoisonable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private float poisonPct
		{
			get
			{
				return base.PoisonPercent;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public override void PostIngested(Pawn ingester)
		{
			HediffSet MShedSet = ingester.health.hediffSet;
			if (MShedSet != null)
			{
				if ((MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMultiVitamins_High"), false)) == null && Rand.Chance(this.poisonPct * Find.Storyteller.difficulty.foodPoisonChanceFactor))
				{
					FoodUtility.AddFoodPoisoningHediff(ingester, this.parent, this.cause);
					return;
				}
			}
			else if (Rand.Chance(this.poisonPct * Find.Storyteller.difficulty.foodPoisonChanceFactor))
			{
				FoodUtility.AddFoodPoisoningHediff(ingester, this.parent, this.cause);
			}
		}
	}
}
