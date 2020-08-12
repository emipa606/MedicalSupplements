using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSExotic
{
	// Token: 0x0200001C RID: 28
	public class MSStimWorn : Apparel
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000065E7 File Offset: 0x000047E7
		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			Pawn wearer = Wearer;
			if (Wearer != null)
			{
				if (Find.Selector.SingleSelectedThing == Wearer)
				{
					string text = "MSExotic.StimUses".Translate(this.def.label.CapitalizeFirst(), this.StimUses.ToString());
					string desc = "MSExotic.StimDesc".Translate(this.def.label.CapitalizeFirst());
					yield return new Command_Action
					{
						defaultLabel = text,
						defaultDesc = desc,
						icon = this.def.uiIcon,
						action = delegate()
						{
							this.MSDoStimSelect(Wearer);
						}
					};
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000065F8 File Offset: 0x000047F8
		public void MSDoStimSelect(Pawn p)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			string text = "MSExotic.StimSelDoNothing".Translate();
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.MSUseStim(p, false);
			}, MenuOptionPriority.Default, null, null, 29f, null, null));
			text = "MSExotic.StimSelUseStim".Translate(this.def.label.CapitalizeFirst());
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.MSUseStim(p, true);
			}, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, this.def), null));
			Find.WindowStack.Add(new FloatMenu(list));
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000066B3 File Offset: 0x000048B3
		public CompMSStimWorn StimWornComp
		{
			get
			{
				return GetComp<CompMSStimWorn>();
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000066BB File Offset: 0x000048BB
		public override void PostMake()
		{
			base.PostMake();
			this.StimUses = GetComp<CompMSStimWorn>().Props.StimUses;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000066D9 File Offset: 0x000048D9
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref this.StimUses, "StimUses", 1, false);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000066F4 File Offset: 0x000048F4
		public void MSUseStim(Pawn p, bool Using)
		{
			if (Using && p != null)
			{
				if (this.def.defName == "MSBattleStimBelt")
				{
                    MSExoticUtility.ChkMSBattleStim(p, out string Reason, out bool Passed);
                    if (!Passed)
					{
						Messages.Message("MSExotic.ReasonPrefix".Translate() + Reason, p, MessageTypeDefOf.NeutralEvent, false);
						return;
					}
					this.StimUses--;
					MSExoticUtility.DoMSBattleStim(p, this.def);
					if (this.StimUses < 1 && !this.DestroyedOrNull())
					{
						this.Destroy(DestroyMode.Vanish);
						return;
					}
				}
				else
				{
					Log.Message(string.Concat(new string[]
					{
						"ERR: Stim Worn item def not found for ",
						this.def.label.CapitalizeFirst(),
						" : (",
						this.def.defName,
						")"
					}), false);
				}
			}
		}

		// Token: 0x04000052 RID: 82
		private int StimUses = 1;
	}
}
