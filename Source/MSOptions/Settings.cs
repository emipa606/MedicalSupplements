using UnityEngine;
using Verse;

namespace MSOptions
{
    // Token: 0x02000004 RID: 4
    public class Settings : ModSettings
    {
        // Token: 0x04000003 RID: 3
        public bool AllowCollapseRocks = true;

        // Token: 0x04000002 RID: 2
        public bool RealisticBandages;

        // Token: 0x04000004 RID: 4
        public float ResPct = 100f;

        // Token: 0x06000007 RID: 7 RVA: 0x00002344 File Offset: 0x00000544
        public void DoWindowContents(Rect canvas)
        {
            var listing_Standard = new Listing_Standard {ColumnWidth = canvas.width};
            listing_Standard.Begin(canvas);
            listing_Standard.Gap();
            listing_Standard.CheckboxLabeled("MSOpt.RealisticBandages".Translate(), ref RealisticBandages);
            listing_Standard.Gap();
            listing_Standard.CheckboxLabeled("MSOpt.AllowCollapseRocks".Translate(), ref AllowCollapseRocks);
            listing_Standard.Gap(24f);
            checked
            {
                listing_Standard.Label("MSOpt.ResPct".Translate() + "  " + (int) ResPct);
                ResPct = (int) listing_Standard.Slider((int) ResPct, 10f, 200f);
                listing_Standard.Gap();
                Text.Font = GameFont.Tiny;
                listing_Standard.Label("          " + "MSOpt.ResWarn".Translate());
                listing_Standard.Gap();
                listing_Standard.Label("          " + "MSOpt.ResTip".Translate());
                Text.Font = GameFont.Small;
                listing_Standard.Gap();
                listing_Standard.End();
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002490 File Offset: 0x00000690
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref RealisticBandages, "RealisticBandages");
            Scribe_Values.Look(ref AllowCollapseRocks, "AllowCollapseRocks", true);
            Scribe_Values.Look(ref ResPct, "ResPct", 100f);
        }
    }
}