using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MSOptions;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSBandageUse
{
    // Token: 0x02000026 RID: 38
    [HarmonyPatch(typeof(HealthAIUtility))]
    [HarmonyPatch("FindBestMedicine")]
    public class HealthAIUtility_FindBestMedicine_PostPatch
    {
        // Token: 0x060000BA RID: 186 RVA: 0x00009280 File Offset: 0x00007480
        [HarmonyPriority(0)]
        public static void Postfix(Pawn healer, Pawn patient, ref Thing __result)
        {
            if (Controller.Settings.RealisticBandages && __result != null && IsBandage(__result.def) &&
                !BandagesValid(patient))
            {
                var medPot = __result.def.GetStatValueAbstract(StatDefOf.MedicalPotency);
                __result = GenClosest.ClosestThing_Global_Reachable(patient.Position, patient.Map,
                    patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine), PathEndMode.ClosestTouch,
                    TraverseParms.For(healer), 9999f,
                    m => !m.IsForbidden(healer) && healer.CanReserve(m) && !IsBandage(m.def) &&
                         m.def.GetStatValueAbstract(StatDefOf.MedicalPotency) <= medPot,
                    m => m.def.GetStatValueAbstract(StatDefOf.MedicalPotency));
            }
        }

        // Token: 0x060000BB RID: 187 RVA: 0x0000935C File Offset: 0x0000755C
        public static bool BandagesValid(Pawn pawn)
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
            if (hedset != null)
            {
                var injuries = hedset.GetHediffsTendable().ToList();
                if (injuries != null && injuries.Count > 0)
                {
                    foreach (var injury in injuries)
                    {
                        if (!(injury is Hediff_Injury) && !Inclusions().Contains(injury.def.defName))
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
            }

            return false;
        }

        // Token: 0x060000BC RID: 188 RVA: 0x00009420 File Offset: 0x00007620
        public static bool IsBandage(ThingDef def)
        {
            return def.IsMedicine && Bandages().Contains(def.defName);
        }

        // Token: 0x060000BD RID: 189 RVA: 0x0000943F File Offset: 0x0000763F
        public static List<string> Bandages()
        {
            var list = new List<string>();
            list.AddDistinct("MSBandage");
            list.AddDistinct("MSASBandage");
            list.AddDistinct("MSNanoBandage");
            list.AddDistinct("AYYarrowBandage");
            list.AddDistinct("Bandagekit");
            return list;
        }

        // Token: 0x060000BE RID: 190 RVA: 0x00009480 File Offset: 0x00007680
        public static List<string> Inclusions()
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
}