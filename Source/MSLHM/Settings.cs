using System;
using UnityEngine;
using Verse;

namespace MSLHM
{
	// Token: 0x02000005 RID: 5
	internal class Settings : ModSettings
	{
		// Token: 0x06000010 RID: 16 RVA: 0x0000262D File Offset: 0x0000082D
		public static Settings Get()
		{
			return LoadedModManager.GetMod<MSLHMMod>().GetSettings<Settings>();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002639 File Offset: 0x00000839
		public void DoWindowContents(Rect wrect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(wrect);
			listing_Standard.CheckboxLabeled("Show aging messages", ref this.showAgingMessages, "Show notification every time age was affected by luci");
			listing_Standard.CheckboxLabeled("Debug luci healing", ref this.debugHealingSpeed, "Luci heal procs way more often");
			listing_Standard.End();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002678 File Offset: 0x00000878
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.showAgingMessages, "showAgingMessages", false, false);
		}

		// Token: 0x04000003 RID: 3
		public bool showAgingMessages;

		// Token: 0x04000004 RID: 4
		public bool debugHealingSpeed;
	}
}
