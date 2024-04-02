using HarmonyLib;
using MSOptions;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSMineBits;

[HarmonyPatch(typeof(Mineable), "TrySpawnYield", typeof(Map), typeof(bool), typeof(Pawn))]
public class TrySpawnYield_PostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref Mineable __instance, Map map, Pawn pawn)
    {
        if (pawn == null)
        {
            return;
        }

        var isSource = false;
        if (__instance.def.defName is "CollapsedRocks" or "rxCollapsedRoofRocks")
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

        if (obj == null && !isSource)
        {
            return;
        }

        var mining = 0;
        var skills = pawn.skills;
        var b = skills?.GetSkill(SkillDefOf.Mining) != null;

        if (b)
        {
            mining = pawn.skills.GetSkill(SkillDefOf.Mining).Level / 4;
        }

        if (Rand.Range(1, 100) > 20 + mining)
        {
            return;
        }

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

        if (!MSBitsUtility.GetIsBitsSource(defSource, isSource, pawn, out var bitsdef,
                out var bitsyield) || bitsdef == null || bitsyield <= 0)
        {
            return;
        }

        var num = Mathf.Max(1,
            Mathf.RoundToInt(bitsyield * Find.Storyteller.difficulty.mineYieldFactor));
        var thing = ThingMaker.MakeThing(bitsdef);
        thing.stackCount = num;
        GenPlace.TryPlaceThing(thing, pawn.Position, map, ThingPlaceMode.Near,
            out var newbitsthing);
        if (!pawn.IsColonist && newbitsthing.def.EverHaulable &&
            !newbitsthing.def.designateHaulable)
        {
            newbitsthing.SetForbidden(true);
        }
    }
}