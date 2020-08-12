using System;
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
			if (Controller.Settings.RealisticBandages && __result != null && HealthAIUtility_FindBestMedicine_PostPatch.IsBandage(__result.def) && !HealthAIUtility_FindBestMedicine_PostPatch.BandagesValid(patient))
			{
				float medPot = __result.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null);
				__result = GenClosest.ClosestThing_Global_Reachable(patient.Position, patient.Map, patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine), PathEndMode.ClosestTouch, TraverseParms.For(healer, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Thing m) => !m.IsForbidden(healer) && healer.CanReserve(m, 1, -1, null, false) && !HealthAIUtility_FindBestMedicine_PostPatch.IsBandage(m.def) && m.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null) <= medPot, (Thing m) => m.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null));
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
				Pawn_HealthTracker health = pawn.health;
				hediffSet = ((health != null) ? health.hediffSet : null);
			}
			HediffSet hedset = hediffSet;
			if (hedset != null)
			{
				List<Hediff> injuries = hedset.GetHediffsTendable().ToList<Hediff>();
				if (injuries != null && injuries.Count > 0)
				{
					foreach (Hediff injury in injuries)
					{
						if (!(injury is Hediff_Injury) && !HealthAIUtility_FindBestMedicine_PostPatch.Inclusions().Contains(injury.def.defName))
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
			return def.IsMedicine && HealthAIUtility_FindBestMedicine_PostPatch.Bandages().Contains(def.defName);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000943F File Offset: 0x0000763F
		public static List<string> Bandages()
		{
			List<string> list = new List<string>();
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
			List<string> list = new List<string>();
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
