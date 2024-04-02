using RimWorld;
using Verse;

namespace MSDrugMix;

public class PlaceWorker_MSDrugMixHopper : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thingy = null)
    {
        for (var i = 0; i < 4; i++)
        {
            var c = loc + NewMethod(i);
            if (!c.InBounds(map))
            {
                continue;
            }

            var thingList = c.GetThingList(map);
            foreach (var thing in thingList)
            {
                if (GenConstruct.BuiltDefOf(thing.def) is ThingDef { building: not null, defName: "MSDrugMixer" } &&
                    IsCorrectSide(thing, c, rot))
                {
                    return true;
                }
            }
        }

        return "MustPlaceNextToHopperAccepter".Translate();
    }

    private static IntVec3 NewMethod(int i)
    {
        return GenAdj.CardinalDirections[i];
    }

    public bool IsCorrectSide(Thing t, IntVec3 c, Rot4 rot)
    {
        var tRot = t.Rotation;
        var tPos = t.Position;
        switch (tRot.AsInt)
        {
            case 0 when c.x > tPos.x:
            case 1 when c.z >= tPos.z:
            case 2 when tPos.x > c.x:
            case 3 when tPos.z >= c.z:
                return true;
            default:
                return false;
        }
    }
}