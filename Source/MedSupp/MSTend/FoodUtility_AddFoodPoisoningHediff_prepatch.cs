using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSTend
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(FoodUtility), "AddFoodPoisoningHediff")]
    public class FoodUtility_AddFoodPoisoningHediff_prepatch
    {
        // Token: 0x06000005 RID: 5 RVA: 0x0000210C File Offset: 0x0000030C
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        public static bool Prefix(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
            return !ImmuneToFP(pawn, HediffDefOf.FoodPoisoning);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002120 File Offset: 0x00000320
        public static bool ImmuneToFP(Pawn pawn, HediffDef FPdef)
        {
            var drugHDefs = MSFPImmDrug();
            var hediffs = pawn.health.hediffSet.hediffs;
            for (var i = 0; i < hediffs.Count; i++)
            {
                if (drugHDefs.Contains(hediffs[i].def.defName))
                {
                    var curStage = hediffs[i].CurStage;
                    if (curStage != null && curStage.makeImmuneTo != null)
                    {
                        for (var j = 0; j < curStage.makeImmuneTo.Count; j++)
                        {
                            if (curStage.makeImmuneTo[j] == FPdef)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000021B3 File Offset: 0x000003B3
        public static List<string> MSFPImmDrug()
        {
            var list = new List<string>();
            list.AddDistinct("MSMultiVitamins_High");
            list.AddDistinct("MSRimpepticHigh");
            return list;
        }
    }
}