using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic;

public class Recipe_MSAdministerInject : Recipe_Surgery
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
        Bill bill)
    {
        foreach (var item in ingredients)
        {
            if (item == ingredients[0])
            {
                continue;
            }

            if (item.def.IsDrug && item.def.IsIngestible)
            {
                var ingestible = item.def.ingestible;
                var listIOD = ingestible?.outcomeDoers;
                if (listIOD is { Count: > 0 })
                {
                    foreach (var ingestionOutcomeDoer in listIOD)
                    {
                        ingestionOutcomeDoer.DoIngestionOutcome(pawn, item, 1);
                    }
                }
            }

            if (!item.DestroyedOrNull())
            {
                item.Destroy();
            }
        }

        ingredients[0].TryGetComp<CompUsable>().UsedBy(pawn);
    }

    public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
    {
    }
}