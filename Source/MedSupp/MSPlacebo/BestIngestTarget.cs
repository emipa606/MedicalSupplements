using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSPlacebo
{
    // Token: 0x0200000F RID: 15
    [HarmonyPatch(typeof(JobGiver_BingeDrug), "BestIngestTarget")]
    public class BestIngestTarget
    {
        // Token: 0x0600003F RID: 63 RVA: 0x000045D4 File Offset: 0x000027D4
        [HarmonyPrefix]
        public static bool Prefix(ref Thing __result, Pawn pawn)
        {
            bool Predicate(Thing t)
            {
                return (pawn.InMentalState || !t.IsForbidden(pawn)) && pawn.CanReserve(t) &&
                       (pawn.Position.InHorDistOf(t.Position, 100f) || t.Position.Roofed(t.Map) ||
                        pawn.Map.areaManager.Home[t.Position] || t.GetSlotGroup() != null);
            }

            var position = pawn.Position;
            var map = pawn.Map;
            var peMode = PathEndMode.OnCell;
            var traverseParams = TraverseParms.For(pawn);
            var validator = (Predicate<Thing>) Predicate;
            var PlaceboReq = ThingRequest.ForDef(ThingDef.Named("MSPlacebo"));
            __result = GenClosest.ClosestThingReachable(position, map, PlaceboReq, peMode, traverseParams, 9999f,
                validator);
            return __result == null;
        }
    }
}