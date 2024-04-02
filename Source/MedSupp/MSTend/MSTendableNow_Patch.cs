using HarmonyLib;
using Verse;

namespace MSTend;

[HarmonyPatch(typeof(Hediff), nameof(Hediff.TendableNow))]
public class MSTendableNow_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref Hediff __instance, ref bool __result, bool ignoreTimer = false)
    {
        if (!__instance.def.tendable || __instance.Severity <= 0f || __instance.FullyImmune() ||
            !__instance.Visible || __instance.IsPermanent())
        {
            __result = false;
            return;
        }

        if (!ignoreTimer)
        {
            var hediffComp_TendDuration = __instance.TryGetComp<HediffComp_TendDuration>();
            if (hediffComp_TendDuration is { AllowTend: false })
            {
                __result = false;
                return;
            }

            var MShediffComp_TendDuration = __instance.TryGetComp<MSHediffComp_TendDuration>();
            if (MShediffComp_TendDuration is { AllowTend: false })
            {
                __result = false;
                return;
            }
        }

        __result = true;
    }
}