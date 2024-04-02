using MSOptions;
using UnityEngine;
using Verse;

namespace MSLHM;

internal class Settings : ModSettings
{
    public bool debugHealingSpeed;

    public bool showAgingMessages;

    public static Settings Get()
    {
        return LoadedModManager.GetMod<MSLHMMod>().GetSettings<Settings>();
    }

    public void DoWindowContents(Rect wrect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(wrect);
        listing_Standard.CheckboxLabeled("MSOpt.ShowAging".Translate(), ref showAgingMessages,
            "MSOpt.ShowAgingTT".Translate());
        listing_Standard.CheckboxLabeled("MSOpt.DebugLuci".Translate(), ref debugHealingSpeed,
            "MSOpt.DebugLuciTT".Translate());
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
        Scribe_Values.Look(ref showAgingMessages, "showAgingMessages");
    }
}