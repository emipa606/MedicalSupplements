using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSTend;

[HarmonyPatch(typeof(FoodUtility), nameof(FoodUtility.AddFoodPoisoningHediff))]
public class FoodUtility_AddFoodPoisoningHediff
{
    [HarmonyPriority(800)]
    public static bool Prefix(Pawn pawn)
    {
        return !ImmuneToFP(pawn, HediffDefOf.FoodPoisoning);
    }

    private static bool ImmuneToFP(Pawn pawn, HediffDef FPdef)
    {
        var drugHDefs = MSFPImmDrug();
        var hediffs = pawn.health.hediffSet.hediffs;
        foreach (var hediff in hediffs)
        {
            if (!drugHDefs.Contains(hediff.def.defName))
            {
                continue;
            }

            var curStage = hediff.CurStage;
            if (curStage?.makeImmuneTo == null)
            {
                continue;
            }

            foreach (var hediffDef in curStage.makeImmuneTo)
            {
                if (hediffDef == FPdef)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static List<string> MSFPImmDrug()
    {
        var list = new List<string>();
        list.AddDistinct("MSMultiVitamins_High");
        list.AddDistinct("MSRimpepticHigh");
        return list;
    }
}