using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSMedMat
{
    // Token: 0x02000012 RID: 18
    [HarmonyPatch(typeof(GenLeaving), "GetBuildingResourcesLeaveCalculator")]
    public class GetBuildingResourcesLeaveCalculator_PostPatch
    {
        // Token: 0x06000048 RID: 72 RVA: 0x00005364 File Offset: 0x00003564
        [HarmonyPostfix]
        public static void PostFix(ref Func<int, int> __result, Thing destroyedThing, ref DestroyMode mode)
        {
            if (!destroyedThing.def.defName.StartsWith("MSMedicalMat") || mode != DestroyMode.Deconstruct ||
                !(destroyedThing.def.resourcesFractionWhenDeconstructed >= 1f))
            {
                return;
            }

            if (!GenLeaving.CanBuildingLeaveResources(destroyedThing, mode))
            {
                __result = _ => 0;
                return;
            }

            if (mode == DestroyMode.Deconstruct && destroyedThing is Frame)
            {
                mode = DestroyMode.Cancel;
                return;
            }

            if (mode == DestroyMode.Deconstruct)
            {
                __result = count => count;
            }
        }
    }
}