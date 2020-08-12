using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MSTend
{
	// Token: 0x02000007 RID: 7
	public class MSHediffComp_GrowthMode : HediffComp_SeverityPerDay
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000022F8 File Offset: 0x000004F8
		public MSHediffCompProperties_GrowthMode Props
		{
			get
			{
				return (MSHediffCompProperties_GrowthMode)this.props;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002305 File Offset: 0x00000505
		public override string CompLabelInBracketsExtra
		{
			get
			{
				return this.growthMode.GetLabel();
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002314 File Offset: 0x00000514
		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref this.growthMode, "growthMode", HediffGrowthMode.Growing, false);
			Scribe_Values.Look(ref this.severityPerDayGrowingRandomFactor, "severityPerDayGrowingRandomFactor", 1f, false);
			Scribe_Values.Look(ref this.severityPerDayRemissionRandomFactor, "severityPerDayRemissionRandomFactor", 1f, false);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002368 File Offset: 0x00000568
		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);
			this.growthMode = ((HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))).RandomElement();
			this.severityPerDayGrowingRandomFactor = this.Props.severityPerDayGrowingRandomFactor.RandomInRange;
			this.severityPerDayRemissionRandomFactor = this.Props.severityPerDayRemissionRandomFactor.RandomInRange;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000023C7 File Offset: 0x000005C7
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (base.Pawn.IsHashIntervalTick(5000) && Rand.MTBEventOccurs(100f, 60000f, 5000f))
			{
				this.ChangeGrowthMode();
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002400 File Offset: 0x00000600
		protected override float SeverityChangePerDay()
		{
			float growthsev;
			switch (this.growthMode)
			{
			case HediffGrowthMode.Growing:
				growthsev = this.Props.severityPerDayGrowing * this.severityPerDayGrowingRandomFactor;
				break;
			case HediffGrowthMode.Stable:
				growthsev = 0f;
				break;
			case HediffGrowthMode.Remission:
				growthsev = this.Props.severityPerDayRemission * this.severityPerDayRemissionRandomFactor;
				break;
			default:
				throw new NotImplementedException("GrowthMode");
			}
			return this.MSAdjustment(base.Pawn, growthsev, this.growthMode);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002480 File Offset: 0x00000680
		public float MSAdjustment(Pawn p, float sev, HediffGrowthMode gm)
		{
			float newSev = sev;
			if (gm == HediffGrowthMode.Growing)
			{
				HediffSet hediffSet;
				if (p == null)
				{
					hediffSet = null;
				}
				else
				{
					Pawn_HealthTracker health = p.health;
					hediffSet = (health?.hediffSet);
				}
				HediffSet hSet = hediffSet;
				if (hSet != null)
				{
					HediffDef drugDef = DefDatabase<HediffDef>.GetNamed("MSVinacol_High", false);
					if (drugDef != null && hSet.GetFirstHediffOfDef(drugDef, false) != null)
					{
						newSev *= Rand.Range(0f, 0.1f);
					}
				}
			}
			return newSev;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000024DC File Offset: 0x000006DC
		private void ChangeGrowthMode()
		{
			this.growthMode = (from x in (HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))
			where x != this.growthMode
			select x).RandomElement();
			if (PawnUtility.ShouldSendNotificationAbout(base.Pawn))
			{
				switch (this.growthMode)
				{
				case HediffGrowthMode.Growing:
					Messages.Message("DiseaseGrowthModeChanged_Growing".Translate(base.Pawn.LabelShort, base.Def.label, base.Pawn.Named("PAWN")), base.Pawn, MessageTypeDefOf.NegativeHealthEvent, true);
					return;
				case HediffGrowthMode.Stable:
					Messages.Message("DiseaseGrowthModeChanged_Stable".Translate(base.Pawn.LabelShort, base.Def.label, base.Pawn.Named("PAWN")), base.Pawn, MessageTypeDefOf.NeutralEvent, true);
					return;
				case HediffGrowthMode.Remission:
					Messages.Message("DiseaseGrowthModeChanged_Remission".Translate(base.Pawn.LabelShort, base.Def.label, base.Pawn.Named("PAWN")), base.Pawn, MessageTypeDefOf.PositiveEvent, true);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002644 File Offset: 0x00000844
		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompDebugString());
			stringBuilder.AppendLine("severity: " + this.parent.Severity.ToString("F3") + ((this.parent.Severity < base.Def.maxSeverity) ? string.Empty : " (reached max)"));
			stringBuilder.AppendLine("severityPerDayGrowingRandomFactor: " + this.severityPerDayGrowingRandomFactor.ToString("0.##"));
			stringBuilder.AppendLine("severityPerDayRemissionRandomFactor: " + this.severityPerDayRemissionRandomFactor.ToString("0.##"));
			return stringBuilder.ToString();
		}

		// Token: 0x0400000E RID: 14
		private const int CheckGrowthModeChangeInterval = 5000;

		// Token: 0x0400000F RID: 15
		private const float GrowthModeChangeMtbDays = 100f;

		// Token: 0x04000010 RID: 16
		public HediffGrowthMode growthMode;

		// Token: 0x04000011 RID: 17
		private float severityPerDayGrowingRandomFactor = 1f;

		// Token: 0x04000012 RID: 18
		private float severityPerDayRemissionRandomFactor = 1f;
	}
}
