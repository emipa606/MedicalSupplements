using System;
using UnityEngine;
using Verse;

namespace MSOptions
{
	// Token: 0x02000002 RID: 2
	public class Controller : Mod
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override string SettingsCategory()
		{
			return "MSOpt.Name".Translate();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002061 File Offset: 0x00000261
		public override void DoSettingsWindowContents(Rect canvas)
		{
			Controller.Settings.DoWindowContents(canvas);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000206E File Offset: 0x0000026E
		public Controller(ModContentPack content) : base(content)
		{
			Controller.Settings = base.GetSettings<Settings>();
		}

		// Token: 0x04000001 RID: 1
		public static Settings Settings;
	}
}
