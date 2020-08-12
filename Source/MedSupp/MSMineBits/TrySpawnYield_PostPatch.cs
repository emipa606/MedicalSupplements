using System;
using HarmonyLib;
using MSOptions;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSMineBits
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(Mineable), "TrySpawnYield")]
	public class TrySpawnYield_PostPatch
	{
		// Token: 0x06000046 RID: 70 RVA: 0x000051D0 File Offset: 0x000033D0
		[HarmonyPostfix]
		public static void PostFix(ref Mineable __instance, Map map, float yieldChance, bool moteOnWaste, Pawn pawn)
		{
			if (pawn != null)
			{
				bool isSource = false;
				if (__instance.def.defName == "CollapsedRocks" || __instance.def.defName == "rxCollapsedRoofRocks")
				{
					isSource = Controller.Settings.AllowCollapseRocks;
				}
				Mineable mineable = __instance;
				object obj;
				if (mineable == null)
				{
					obj = null;
				}
				else
				{
					ThingDef def = mineable.def;
					if (def == null)
					{
						obj = null;
					}
					else
					{
						BuildingProperties building = def.building;
						obj = (building?.mineableThing);
					}
				}
				if (obj != null || isSource)
				{
					int mining = 0;
					bool flag;
					if (pawn == null)
					{
						flag = (null != null);
					}
					else
					{
						Pawn_SkillTracker skills = pawn.skills;
						flag = ((skills?.GetSkill(SkillDefOf.Mining)) != null);
					}
					if (flag)
					{
						mining = pawn.skills.GetSkill(SkillDefOf.Mining).Level / 4;
					}
					if (Rand.Range(1, 100) <= 20 + mining)
					{
						Mineable mineable2 = __instance;
						ThingDef defSource;
						if (mineable2 == null)
						{
							defSource = null;
						}
						else
						{
							ThingDef def2 = mineable2.def;
							if (def2 == null)
							{
								defSource = null;
							}
							else
							{
								BuildingProperties building2 = def2.building;
								defSource = (building2?.mineableThing);
							}
						}
                        if (MSBitsUtility.GetIsBitsSource(defSource, isSource, pawn, out ThingDef bitsdef, out int bitsyield) && bitsdef != null && bitsyield > 0)
                        {
                            int num = Mathf.Max(1, Mathf.RoundToInt((float)bitsyield * Find.Storyteller.difficulty.mineYieldFactor));
                            Thing thing = ThingMaker.MakeThing(bitsdef, null);
                            thing.stackCount = num;
                            GenPlace.TryPlaceThing(thing, pawn.Position, map, ThingPlaceMode.Near, out Thing newbitsthing, null, null, default);
                            if ((pawn == null || !pawn.IsColonist) && newbitsthing.def.EverHaulable && !newbitsthing.def.designateHaulable)
                            {
                                newbitsthing.SetForbidden(true, true);
                            }
                        }
                    }
				}
			}
		}
	}
}
