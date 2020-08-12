using System;
using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MSLHM
{
	// Token: 0x02000006 RID: 6
	[StaticConstructorOnStartup]
	internal static class MultiplayerSupport
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002694 File Offset: 0x00000894
		static MultiplayerSupport()
		{
			if (!MP.enabled)
			{
				return;
			}
			MethodInfo[] array = new MethodInfo[]
			{
				AccessTools.Method(typeof(HediffComp_HealPermanentWounds), "ResetTicksToHeal", null, null),
				AccessTools.Method(typeof(HediffComp_HealPermanentWounds), "TryHealRandomPermanentWound", null, null)
			};
			for (int i = 0; i < array.Length; i++)
			{
                FixRNG(array[i]);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002707 File Offset: 0x00000907
		private static void FixRNG(MethodInfo method)
		{
            harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre", null), new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos", null), null, null);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002741 File Offset: 0x00000941
		private static void FixRNGPre()
		{
			Rand.PushState(Find.TickManager.TicksAbs);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002752 File Offset: 0x00000952
		private static void FixRNGPos()
		{
			Rand.PopState();
		}

		// Token: 0x04000005 RID: 5
		private static readonly Harmony harmony = new Harmony("rimworld.mslhm.multiplayersupport");
	}
}
