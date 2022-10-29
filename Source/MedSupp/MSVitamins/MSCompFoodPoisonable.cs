using RimWorld;
using Verse;

namespace MSVitamins;

public class MSCompFoodPoisonable : CompFoodPoisonable
{
    private float poisonPct => PoisonPercent;

    public override void PostIngested(Pawn ingester)
    {
        var MShedSet = ingester.health.hediffSet;
        if (MShedSet != null)
        {
            if (MShedSet.GetFirstHediffOfDef(HediffDef.Named("MSMultiVitamins_High")) == null &&
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