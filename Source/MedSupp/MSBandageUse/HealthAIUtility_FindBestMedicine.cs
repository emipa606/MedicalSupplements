using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MSOptions;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSBandageUse;

[HarmonyPatch(typeof(HealthAIUtility), nameof(HealthAIUtility.FindBestMedicine))]
public class HealthAIUtility_FindBestMedicine
{
    [HarmonyPriority(0)]
    public static void Postfix(Pawn healer, Pawn patient, ref Thing __result)
    {
        if (!Controller.Settings.RealisticBandages || __result == null || !IsBandage(__result.def) ||
            BandagesValid(patient))
        {
            return;
        }

        var medPot = __result.def.GetStatValueAbstract(StatDefOf.MedicalPotency);
        __result = GenClosest.ClosestThing_Global_Reachable(patient.Position, patient.Map,
            patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine), PathEndMode.ClosestTouch,
            TraverseParms.For(healer), 9999f,
            m => !m.IsForbidden(healer) && healer.CanReserve(m) && !IsBandage(m.def) &&
                 m.def.GetStatValueAbstract(StatDefOf.MedicalPotency) <= medPot,
            m => m.def.GetStatValueAbstract(StatDefOf.MedicalPotency));
    }

    private static bool BandagesValid(Pawn pawn)
    {
        HediffSet hediffSet;
        if (pawn == null)
        {
            hediffSet = null;
        }
        else
        {
            var health = pawn.health;
            hediffSet = health?.hediffSet;
        }

        var hedset = hediffSet;

        var injuries = hedset?.GetHediffsTendable().ToList();
        if (injuries is not { Count: > 0 })
        {
            return false;
        }

        foreach (var injury in injuries)
        {
            if (injury is not Hediff_Injury && !Inclusions().Contains(injury.def.defName))
            {
                return false;
            }

            if (injury is Hediff_Injury && injury.Part.depth == BodyPartDepth.Inside)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsBandage(ThingDef def)
    {
        return def.IsMedicine && Bandages().Contains(def.defName);
    }

    private static List<string> Bandages()
    {
        var list = new List<string>();
        list.AddDistinct("MSBandage");
        list.AddDistinct("MSASBandage");
        list.AddDistinct("MSNanoBandage");
        list.AddDistinct("AYYarrowBandage");
        list.AddDistinct("Bandagekit");
        return list;
    }

    private static List<string> Inclusions()
    {
        var list = new List<string>();
        list.AddDistinct("CA_Knick_Hand");
        list.AddDistinct("CA_Knick_Arm");
        list.AddDistinct("CA_Knick_Foot");
        list.AddDistinct("CA_Knick_Leg");
        list.AddDistinct("CA_Sprain_Hand");
        list.AddDistinct("CA_Sprain_Arm");
        list.AddDistinct("CA_Sprain_Foot");
        list.AddDistinct("CA_Sprain_Leg");
        return list;
    }
}