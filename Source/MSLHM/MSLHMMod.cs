using UnityEngine;
using Verse;

namespace MSLHM;

public class MSLHMMod : Mod
{
    public MSLHMMod(ModContentPack content) : base(content)
    {
        GetSettings<Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        GetSettings<Settings>().DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Medical Supplements: Luci heals more";
    }
}