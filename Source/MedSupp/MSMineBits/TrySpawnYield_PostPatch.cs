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
                var isSource = false;
                if (__instance.def.defName == "CollapsedRocks" || __instance.def.defName == "rxCollapsedRoofRocks")
                {
                    isSource = Controller.Settings.AllowCollapseRocks;
                }

                var mineable = __instance;
                object obj;
                if (mineable == null)
                {
                    obj = null;
                }
                else
                {
                    var def = mineable.def;
                    if (def == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        var building = def.building;
                        obj = building?.mineableThing;
                    }
                }

                if (obj != null || isSource)
                {
                    var mining = 0;
                    bool flag;
                    if (pawn == null)
                    {
                        flag = null != null;
                    }
                    else
                    {
                        var skills = pawn.skills;
                        flag = skills?.GetSkill(SkillDefOf.Mining) != null;
                    }

                    if (flag)
                    {
                        mining = pawn.skills.GetSkill(SkillDefOf.Mining).Level / 4;
                    }

                    if (Rand.Range(1, 100) <= 20 + mining)
                    {
                        var mineable2 = __instance;
                        ThingDef defSource;
                        if (mineable2 == null)
                        {
                            defSource = null;
                        }
                        else
                        {
                            var def2 = mineable2.def;
                            if (def2 == null)
                            {
                                defSource = null;
                            }
                            else
                            {
                                var building2 = def2.building;
                                defSource = building2?.mineableThing;
                            }
                        }

                        if (MSBitsUtility.GetIsBitsSource(defSource, isSource, pawn, out var bitsdef,
                            out var bitsyield) && bitsdef != null && bitsyield > 0)
                        {
                            var num = Mathf.Max(1,
                                Mathf.RoundToInt(bitsyield * Find.Storyteller.difficulty.mineYieldFactor));
                            var thing = ThingMaker.MakeThing(bitsdef);
                            thing.stackCount = num;
                            GenPlace.TryPlaceThing(thing, pawn.Position, map, ThingPlaceMode.Near,
                                out var newbitsthing);
                            if ((pawn == null || !pawn.IsColonist) && newbitsthing.def.EverHaulable &&
                                !newbitsthing.def.designateHaulable)
                            {
                                newbitsthing.SetForbidden(true);
                            }
                        }
                    }
                }
            }
        }
    }
}