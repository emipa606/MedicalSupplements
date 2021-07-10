using RimWorld;
using Verse;

namespace MSDrugMix
{
    // Token: 0x02000022 RID: 34
    public class PlaceWorker_MSDrugMixHopper : PlaceWorker
    {
        // Token: 0x060000AF RID: 175 RVA: 0x00008EF0 File Offset: 0x000070F0
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
                    if (GenConstruct.BuiltDefOf(thing.def) is ThingDef {building: { }, defName: "MSDrugMixer"} &&
                        IsCorrectSide(thing, c, rot))
                    {
                        return true;
                    }
                }
            }

            return "MustPlaceNextToHopperAccepter".Translate();
        }

        // Token: 0x060000B0 RID: 176 RVA: 0x00008F9E File Offset: 0x0000719E
        private static IntVec3 NewMethod(int i)
        {
            return GenAdj.CardinalDirections[i];
        }

        // Token: 0x060000B1 RID: 177 RVA: 0x00008FAC File Offset: 0x000071AC
        public bool IsCorrectSide(Thing t, IntVec3 c, Rot4 rot)
        {
            var tRot = t.Rotation;
            var tPos = t.Position;
            if (tRot.AsInt == 0)
            {
                if (c.x > tPos.x)
                {
                    return true;
                }
            }
            else if (tRot.AsInt == 1)
            {
                if (c.z >= tPos.z)
                {
                    return true;
                }
            }
            else if (tRot.AsInt == 2)
            {
                if (tPos.x > c.x)
                {
                    return true;
                }
            }
            else if (tRot.AsInt == 3 && tPos.z >= c.z)
            {
                return true;
            }

            return false;
        }
    }
}