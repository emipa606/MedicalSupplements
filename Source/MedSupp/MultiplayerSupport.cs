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
    [StaticConstructorOnStartup]
    internal static class MultiplayerSupport
    {
        static MultiplayerSupport()
        {
            if (!MP.enabled) return;

            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.MSMixerSelectChem));
            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.SetProdControlValues));
            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.ToggleProducing));
            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.MSMixerSelectLimit));
            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.SetStockLimits));
            MP.RegisterSyncMethod(typeof(Building_MSDrugMix), nameof(Building_MSDrugMix.ToggleDebug));
            MP.RegisterSyncMethod(typeof(MSStimWorn), nameof(MSStimWorn.MSDoStimSelect));
            MP.RegisterSyncMethod(typeof(MSStimWorn), nameof(MSStimWorn.MSUseStim));

            var rngMethods = new MethodInfo[]
            {
                AccessTools.Method(typeof(MSBitsUtility), nameof(MSBitsUtility.GetBitsYield)),
                AccessTools.Method(typeof(MSBitsUtility), nameof(MSBitsUtility.GetIsBitsSource)),
                AccessTools.Method(typeof(MSExoticUtility), nameof(MSExoticUtility.DoMSTranscendence)),
                AccessTools.Method(typeof(MSExoticUtility), nameof(MSExoticUtility.DoMSPerpetuity)),
                AccessTools.Method(typeof(HediffComp_MSRegen), nameof(HediffComp_MSRegen.ResetTicksToHeal)),
                AccessTools.Method(typeof(HediffComp_MSRegen), nameof(HediffComp_MSRegen.TryHealRandomOldWound)),
                AccessTools.Method(typeof(MSHediffComp_TendDuration), nameof(MSHediffComp_TendDuration.CompTended_NewTemp)),
                AccessTools.Method(typeof(MSHediffComp_TendDuration), nameof(MSHediffComp_TendDuration.CompPostTick)),
                AccessTools.Method(typeof(MSCompFoodPoisonable), nameof(MSCompFoodPoisonable.PostIngested)),
                AccessTools.Method(typeof(HediffComp_MSCure), nameof(HediffComp_MSCure.SetTicksToCure)),
                AccessTools.Method(typeof(MSHediffComp_GrowthMode), nameof(MSHediffComp_GrowthMode.CompPostPostAdd)),
                AccessTools.Method(typeof(MSHediffComp_GrowthMode), nameof(MSHediffComp_GrowthMode.CompPostTick)),
                AccessTools.Method(typeof(MSHediffComp_GrowthMode), nameof(MSHediffComp_GrowthMode.MSAdjustment))
            };
            for (int i = 0; i < rngMethods.Length; i++)
            {
                FixRNG(rngMethods[i]);
            }
        }

        private static void FixRNG(MethodInfo method)
        {
            harmony.Patch(method,
                prefix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPre)),
                postfix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPos))
            );
        }

        private static void FixRNGPre()
        {
            Rand.PushState(Find.TickManager.TicksAbs);
        }

        private static void FixRNGPos()
        {
            Rand.PopState();
        }

        private static readonly Harmony harmony = new Harmony("rimworld.medicalsupplements.multiplayersupport");
    }
}