using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSPlacebo;

[HarmonyPatch(typeof(JobGiver_BingeDrug), "BestIngestTarget")]
public class BestIngestTarget
{
    [HarmonyPrefix]
    public static bool Prefix(ref Thing __result, Pawn pawn)
    {
        var position = pawn.Position;
        var map = pawn.Map;
        var peMode = PathEndMode.OnCell;
        var traverseParams = TraverseParms.For(pawn);
        var validator = (Predicate<Thing>)Predicate;
        var PlaceboReq = ThingRequest.ForDef(ThingDef.Named("MSPlacebo"));
        __result = GenClosest.ClosestThingReachable(position, map, PlaceboReq, peMode, traverseParams, 9999f,
            validator);
        return __result == null;

        bool Predicate(Thing t)
        {
            return (pawn.InMentalState || !t.IsForbidden(pawn)) && pawn.CanReserve(t) &&
                   (pawn.Position.InHorDistOf(t.Position, 100f) || t.Position.Roofed(t.Map) ||
                    pawn.Map.areaManager.Home[t.Position] || t.GetSlotGroup() != null);
        }
    }
}