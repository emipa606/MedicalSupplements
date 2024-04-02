using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MSLHM;

[StaticConstructorOnStartup]
internal static class MultiplayerSupport
{
    private static readonly Harmony harmony = new Harmony("rimworld.mslhm.multiplayersupport");

    static MultiplayerSupport()
    {
        if (!MP.enabled)
        {
            return;
        }

        MethodInfo[] array =
        [
            AccessTools.Method(typeof(HediffComp_HealPermanentWounds), "ResetTicksToHeal"),
            AccessTools.Method(typeof(HediffComp_HealPermanentWounds), "TryHealRandomPermanentWound")
        ];
        foreach (var methodInfo in array)
        {
            FixRNG(methodInfo);
        }
    }

    private static void FixRNG(MethodInfo method)
    {
        harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
            new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
    }

    private static void FixRNGPre()
    {
        Rand.PushState(Find.TickManager.TicksAbs);
    }

    private static void FixRNGPos()
    {
        Rand.PopState();
    }
}