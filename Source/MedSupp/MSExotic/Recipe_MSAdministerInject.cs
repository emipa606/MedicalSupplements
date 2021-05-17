using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic
{
    // Token: 0x0200001E RID: 30
    public class Recipe_MSAdministerInject : Recipe_Surgery
    {
        // Token: 0x0600007F RID: 127 RVA: 0x000069CC File Offset: 0x00004BCC
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
            Bill bill)
        {
            foreach (var item in ingredients)
            {
                if (item != ingredients[0])
                {
                    if (item.def.IsDrug && item.def.IsIngestible)
                    {
                        var ingestible = item.def.ingestible;
                        var listIOD = ingestible?.outcomeDoers;
                        if (listIOD.Count > 0)
                        {
                            foreach (var ingestionOutcomeDoer in listIOD)
                            {
                                ingestionOutcomeDoer.DoIngestionOutcome(pawn, item);
                            }
                        }
                    }

                    if (!item.DestroyedOrNull())
                    {
                        item.Destroy();
                    }
                }
            }

            ingredients[0].TryGetComp<CompUsable>().UsedBy(pawn);
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00006AC0 File Offset: 0x00004CC0
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
        }
    }
}