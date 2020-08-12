using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSDrugMix
{
	// Token: 0x02000022 RID: 34
	public class PlaceWorker_MSDrugMixHopper : PlaceWorker
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00008EF0 File Offset: 0x000070F0
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thingy = null)
		{
			for (int i = 0; i < 4; i++)
			{
				IntVec3 c = loc + PlaceWorker_MSDrugMixHopper.NewMethod(i);
				if (c.InBounds(map))
				{
					List<Thing> thingList = c.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						Thing thing = thingList[j];
						ThingDef thingDef = GenConstruct.BuiltDefOf(thing.def) as ThingDef;
						if (thingDef != null && thingDef.building != null && thingDef.defName == "MSDrugMixer" && this.IsCorrectSide(thing, c, rot))
						{
							return true;
						}
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
			Rot4 tRot = t.Rotation;
			IntVec3 tPos = t.Position;
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
