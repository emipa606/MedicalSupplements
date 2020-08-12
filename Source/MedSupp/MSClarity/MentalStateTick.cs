using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSClarity
{
	// Token: 0x02000023 RID: 35
	[HarmonyPatch(typeof(MentalState), "MentalStateTick")]
	public class MentalStateTick
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00009038 File Offset: 0x00007238
		[HarmonyPrefix]
		public static bool Prefix(MentalState __instance)
		{
			if (!(__instance.def.defName == "Wander_Psychotic"))
			{
				return true;
			}
			Pawn p = __instance.pawn;
			if (!p.InMentalState || !p.IsHashIntervalTick(150))
			{
				return true;
			}
			HediffSet MShedSet = p.health.hediffSet;
			if (MShedSet == null)
			{
				return true;
			}
			Hediff MSCheckClarity = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High"), false);
			if (MSCheckClarity != null)
			{
				__instance.RecoverFromState();
				Messages.Message(p.Label.CapitalizeFirst() + "'s condition of " + __instance.def.label.CapitalizeFirst() + " has been cured by " + MSCheckClarity.LabelBase.CapitalizeFirst(), p, MessageTypeDefOf.PositiveEvent, true);
				return false;
			}
			return true;
		}
	}
}
