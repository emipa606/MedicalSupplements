using System;
using UnityEngine;
using Verse;

namespace MSLHM
{
	// Token: 0x02000004 RID: 4
	public class MSLHMMod : Mod
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002601 File Offset: 0x00000801
		public MSLHMMod(ModContentPack content) : base(content)
		{
			base.GetSettings<Settings>();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002611 File Offset: 0x00000811
		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			base.GetSettings<Settings>().DoWindowContents(inRect);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002626 File Offset: 0x00000826
		public override string SettingsCategory()
		{
			return "Medical Supplements: Luci heals more";
		}
	}
}
