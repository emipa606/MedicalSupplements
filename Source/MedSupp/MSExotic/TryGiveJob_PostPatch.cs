using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSExotic
{
    // Token: 0x0200001F RID: 31
    [HarmonyPatch(typeof(JobGiver_DoLovin), "TryGiveJob")]
    public class TryGiveJob_PostPatch
    {
        // Token: 0x06000082 RID: 130 RVA: 0x00006ACC File Offset: 0x00004CCC
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

            var pawn2 = pawn;
            if (pawn2?.Map == null || !pawn.Spawned)
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
                var pawn3 = pawn;
                if (pawn3.GetRoom() == null)
                {
                    return false;
                }

                var pawn4 = pawn;
                if (pawn4.GetRoom() == t?.GetRoom())
                {
                    return true;
                }

                return false;
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
}