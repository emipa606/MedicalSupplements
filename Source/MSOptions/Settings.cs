using UnityEngine;
using Verse;

namespace MSOptions;

public class Settings : ModSettings
{
    public bool AllowCollapseRocks = true;

    public bool RealisticBandages;

    public float ResPct = 100f;

    public void DoWindowContents(Rect canvas)
    {
        var listing_Standard = new Listing_Standard { ColumnWidth = canvas.width };
        listing_Standard.Begin(canvas);
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("MSOpt.RealisticBandages".Translate(), ref RealisticBandages);
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("MSOpt.AllowCollapseRocks".Translate(), ref AllowCollapseRocks);
        listing_Standard.Gap(24f);
        checked
        {
            listing_Standard.Label("MSOpt.ResPct".Translate() + "  " + (int)ResPct);
            ResPct = (int)listing_Standard.Slider((int)ResPct, 10f, 200f);
            listing_Standard.Gap();
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + "MSOpt.ResWarn".Translate());
            listing_Standard.Gap();
            listing_Standard.Label("          " + "MSOpt.ResTip".Translate());
            Text.Font = GameFont.Small;
            listing_Standard.Gap();
        }

        if (Controller.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("MSOpt.ModVersion".Translate(Controller.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref RealisticBandages, "RealisticBandages");
        Scribe_Values.Look(ref AllowCollapseRocks, "AllowCollapseRocks", true);
        Scribe_Values.Look(ref ResPct, "ResPct", 100f);
    }
}