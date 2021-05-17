using RimWorld;
using Verse;

namespace MSVitamins
{
    // Token: 0x02000002 RID: 2
    public class MSCompFoodPoisonable : CompFoodPoisonable
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private float poisonPct => PoisonPercent;

        // Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        public override void PostIngested(Pawn ingester)
        {
            var MShedSet = ingester.health.hediffSet;
            if (MShedSet != null)
            {
                if (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMultiVitamins_High")) == null &&
                    Rand.Chance(poisonPct * Find.Storyteller.difficulty.foodPoisonChanceFactor))
                {
                    FoodUtility.AddFoodPoisoningHediff(ingester, parent, cause);
                }
            }
            else if (Rand.Chance(poisonPct * Find.Storyteller.difficulty.foodPoisonChanceFactor))
            {
                FoodUtility.AddFoodPoisoningHediff(ingester, parent, cause);
            }
        }
    }
}