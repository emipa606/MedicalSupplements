using System;
using UnityEngine;
using Verse;

namespace MSOptions
{
	// Token: 0x02000004 RID: 4
	public class Settings : ModSettings
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002344 File Offset: 0x00000544
		public void DoWindowContents(Rect canvas)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap(12f);
			listing_Standard.CheckboxLabeled("MSOpt.RealisticBandages".Translate(), ref this.RealisticBandages, null);
			listing_Standard.Gap(12f);
			listing_Standard.CheckboxLabeled("MSOpt.AllowCollapseRocks".Translate(), ref this.AllowCollapseRocks, null);
			listing_Standard.Gap(24f);
			checked
			{
				listing_Standard.Label("MSOpt.ResPct".Translate() + "  " + (int)this.ResPct, -1f, null);
				this.ResPct = (float)((int)listing_Standard.Slider((float)((int)this.ResPct), 10f, 200f));
				listing_Standard.Gap(12f);
				Text.Font = GameFont.Tiny;
				listing_Standard.Label("          " + "MSOpt.ResWarn".Translate(), -1f, null);
				listing_Standard.Gap(12f);
				listing_Standard.Label("          " + "MSOpt.ResTip".Translate(), -1f, null);
				Text.Font = GameFont.Small;
				listing_Standard.Gap(12f);
				listing_Standard.End();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002490 File Offset: 0x00000690
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref this.RealisticBandages, "RealisticBandages", false, false);
			Scribe_Values.Look(ref this.AllowCollapseRocks, "AllowCollapseRocks", true, false);
			Scribe_Values.Look(ref this.ResPct, "ResPct", 100f, false);
		}

		// Token: 0x04000002 RID: 2
		public bool RealisticBandages;

		// Token: 0x04000003 RID: 3
		public bool AllowCollapseRocks = true;

		// Token: 0x04000004 RID: 4
		public float ResPct = 100f;
	}
}
