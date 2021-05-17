using HarmonyLib;
using Verse;

namespace MSTend
{
    // Token: 0x02000009 RID: 9
    [HarmonyPatch(typeof(Hediff), "TendableNow")]
    public class MSTendableNow_Patch
    {
        // Token: 0x06000028 RID: 40 RVA: 0x00003DE8 File Offset: 0x00001FE8
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
                if (hediffComp_TendDuration != null && !hediffComp_TendDuration.AllowTend)
                {
                    __result = false;
                    return;
                }

                var MShediffComp_TendDuration = __instance.TryGetComp<MSHediffComp_TendDuration>();
                if (MShediffComp_TendDuration != null && !MShediffComp_TendDuration.AllowTend)
                {
                    __result = false;
                    return;
                }
            }

            __result = true;
        }
    }
}