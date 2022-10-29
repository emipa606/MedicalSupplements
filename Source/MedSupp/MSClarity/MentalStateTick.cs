using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSClarity;

[HarmonyPatch(typeof(MentalState), "MentalStateTick")]
public class MentalStateTick
{
    [HarmonyPrefix]
    public static bool Prefix(MentalState __instance)
    {
        if (__instance.def.defName != "Wander_Psychotic")
        {
            return true;
        }

        var p = __instance.pawn;
        if (!p.InMentalState || !p.IsHashIntervalTick(150))
        {
            return true;
        }

        var MShedSet = p.health.hediffSet;

        var MSCheckClarity = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High"));
        if (MSCheckClarity == null)
        {
            return true;
        }

        __instance.RecoverFromState();
        Messages.Message(
            $"{p.Label.CapitalizeFirst()}'s condition of {__instance.def.label.CapitalizeFirst()} has been cured by {MSCheckClarity.LabelBase.CapitalizeFirst()}",
            p,
            MessageTypeDefOf.PositiveEvent);
        return false;
    }
}