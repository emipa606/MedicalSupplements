using System;
using UnityEngine;
using Verse;

namespace MSTend
{
	// Token: 0x02000006 RID: 6
	public class MSHediffCompProperties_TendDuration : HediffCompProperties
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002225 File Offset: 0x00000425
		public bool TendIsPermanent
		{
			get
			{
				return this.baseTendDurationHours < 0f;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002234 File Offset: 0x00000434
		public int TendTicksFull
		{
			get
			{
				if (this.TendIsPermanent)
				{
					Log.ErrorOnce("Queried TendTicksFull on permanent-tend Hediff.", 6163263, false);
				}
				return Mathf.RoundToInt((this.baseTendDurationHours + this.tendOverlapHours) * 2500f);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002266 File Offset: 0x00000466
		public int TendTicksBase
		{
			get
			{
				if (this.TendIsPermanent)
				{
					Log.ErrorOnce("Queried TendTicksBase on permanent-tend Hediff.", 61621263, false);
				}
				return Mathf.RoundToInt(this.baseTendDurationHours * 2500f);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002291 File Offset: 0x00000491
		public int TendTicksOverlap
		{
			get
			{
				if (this.TendIsPermanent)
				{
					Log.ErrorOnce("Queried TendTicksOverlap on permanent-tend Hediff.", 1963263, false);
				}
				return Mathf.RoundToInt(this.tendOverlapHours * 2500f);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022BC File Offset: 0x000004BC
		public MSHediffCompProperties_TendDuration()
		{
			this.compClass = typeof(MSHediffComp_TendDuration);
		}

		// Token: 0x04000005 RID: 5
		private readonly float baseTendDurationHours = -1f;

		// Token: 0x04000006 RID: 6
		private readonly float tendOverlapHours = 3f;

		// Token: 0x04000007 RID: 7
		public bool tendAllAtOnce;

		// Token: 0x04000008 RID: 8
		public int disappearsAtTotalTendQuality = -1;

		// Token: 0x04000009 RID: 9
		public float severityPerDayTended;

		// Token: 0x0400000A RID: 10
		public bool showTendQuality = true;

		// Token: 0x0400000B RID: 11
		[LoadAlias("labelTreatedWell")]
		public string labelTendedWell;

		// Token: 0x0400000C RID: 12
		[LoadAlias("labelTreatedWellInner")]
		public string labelTendedWellInner;

		// Token: 0x0400000D RID: 13
		[LoadAlias("labelSolidTreatedWell")]
		public string labelSolidTendedWell;
	}
}
