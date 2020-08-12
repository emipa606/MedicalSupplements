using System;
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
			if (__result != null && !MSHediffEffecter.HasHediff(pawn, DefDatabase<HediffDef>.GetNamed("MSCondom_High", false)))
			{
				JobDef CondomJobDef = DefDatabase<JobDef>.GetNamed("MSWearCondom", true);
				if (CondomJobDef != null)
				{
					Pawn pawn2 = pawn;
					if (((pawn2 != null) ? pawn2.Map : null) != null && pawn.Spawned)
					{
						ThingDef CondomDef = DefDatabase<ThingDef>.GetNamed("MSCondom", false);
						if (CondomDef != null)
						{
							TraverseParms TravParms = TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.None, false);
							Predicate<Thing> validator = delegate(Thing t)
							{
								Pawn pawn3 = pawn;
								if (((pawn3 != null) ? pawn3.GetRoom(RegionType.Set_Passable) : null) != null)
								{
									Pawn pawn4 = pawn;
									if (((pawn4 != null) ? pawn4.GetRoom(RegionType.Set_Passable) : null) == ((t != null) ? t.GetRoom(RegionType.Set_Passable) : null))
									{
										return true;
									}
								}
								return false;
							};
							Thing Condom = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(CondomDef), PathEndMode.ClosestTouch, TravParms, 20f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
							if (Condom != null)
							{
								Job JobCondom = new Job(CondomJobDef, Condom);
								__result = JobCondom;
							}
						}
					}
				}
			}
		}
	}
}
