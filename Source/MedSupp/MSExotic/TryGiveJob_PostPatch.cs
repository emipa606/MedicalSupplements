using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSExotic;

[HarmonyPatch(typeof(JobGiver_DoLovin), "TryGiveJob")]
public class TryGiveJob_PostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref Job __result, Pawn pawn)
    {
        if (__result == null ||
            MSHediffEffecter.HasHediff(pawn, DefDatabase<HediffDef>.GetNamed("MSCondom_High", false)))
        {
            return;
        }

        var CondomJobDef = DefDatabase<JobDef>.GetNamed("MSWearCondom");
        if (CondomJobDef == null)
        {
            return;
        }

        if (pawn is { Map: null, Spawned: true })
        {
            return;
        }

        var CondomDef = DefDatabase<ThingDef>.GetNamed("MSCondom", false);
        if (CondomDef == null)
        {
            return;
        }

        var TravParms = TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.None);

        bool Validator(Thing t)
        {
            if (pawn.GetRoom() == null)
            {
                return false;
            }

            return pawn.GetRoom() == t?.GetRoom();
        }

        var Condom = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
            ThingRequest.ForDef(CondomDef), PathEndMode.ClosestTouch, TravParms, 20f, Validator);
        if (Condom == null)
        {
            return;
        }

        var JobCondom = new Job(CondomJobDef, Condom);
        __result = JobCondom;
    }
}