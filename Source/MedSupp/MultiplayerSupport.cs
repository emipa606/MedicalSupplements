using System;
using System.Reflection;
using HarmonyLib;
using MSDrugMix;
using MSExotic;
using MSMineBits;
using MSRegen;
using MSTend;
using MSVitamins;
using Multiplayer.API;
using Verse;

namespace MedSupp
{
	// Token: 0x0200002C RID: 44
	[StaticConstructorOnStartup]
	internal static class MultiplayerSupport
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00009830 File Offset: 0x00007A30
		static MultiplayerSupport()
		{
			if (!MP.enabled)
			{
				return;
			}
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "MSMixerSelectChem", null);
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "SetProdControlValues", null);
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "ToggleProducing", null);
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "MSMixerSelectLimit", null);
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "SetStockLimits", null);
			MP.RegisterSyncMethod(typeof(Building_MSDrugMix), "ToggleDebug", null);
			MP.RegisterSyncMethod(typeof(MSStimWorn), "MSDoStimSelect", null);
			MP.RegisterSyncMethod(typeof(MSStimWorn), "MSUseStim", null);
			MethodInfo[] array = new MethodInfo[]
			{
				AccessTools.Method(typeof(MSBitsUtility), "GetBitsYield", null, null),
				AccessTools.Method(typeof(MSBitsUtility), "GetIsBitsSource", null, null),
				AccessTools.Method(typeof(MSExoticUtility), "DoMSTranscendence", null, null),
				AccessTools.Method(typeof(MSExoticUtility), "DoMSPerpetuity", null, null),
				AccessTools.Method(typeof(HediffComp_MSRegen), "ResetTicksToHeal", null, null),
				AccessTools.Method(typeof(HediffComp_MSRegen), "TryHealRandomOldWound", null, null),
				AccessTools.Method(typeof(MSHediffComp_TendDuration), "CompTended", null, null),
				AccessTools.Method(typeof(MSHediffComp_TendDuration), "CompPostTick", null, null),
				AccessTools.Method(typeof(MSCompFoodPoisonable), "PostIngested", null, null),
				AccessTools.Method(typeof(HediffComp_MSCure), "SetTicksToCure", null, null),
				AccessTools.Method(typeof(MSHediffComp_GrowthMode), "CompPostPostAdd", null, null),
				AccessTools.Method(typeof(MSHediffComp_GrowthMode), "CompPostTick", null, null),
				AccessTools.Method(typeof(MSHediffComp_GrowthMode), "MSAdjustment", null, null)
			};
			for (int i = 0; i < array.Length; i++)
			{
				MultiplayerSupport.FixRNG(array[i]);
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00009A6B File Offset: 0x00007C6B
		private static void FixRNG(MethodInfo method)
		{
			MultiplayerSupport.harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre", null), new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos", null), null, null);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009AA5 File Offset: 0x00007CA5
		private static void FixRNGPre()
		{
			Rand.PushState(Find.TickManager.TicksAbs);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009AB6 File Offset: 0x00007CB6
		private static void FixRNGPos()
		{
			Rand.PopState();
		}

		// Token: 0x0400006A RID: 106
		private static readonly Harmony harmony = new Harmony("rimworld.medicalsupplements.multiplayersupport");
	}
}
