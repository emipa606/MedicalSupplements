﻿using System;
using Verse;

namespace MSClarity
{
	// Token: 0x02000025 RID: 37
	public class MSHDComp_AntiPsychotic : HediffComp_Disappears
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x0000912A File Offset: 0x0000732A
		public MSHDCompProperties_AntiPsychotic GetProps()
		{
			return (MSHDCompProperties_AntiPsychotic)this.props;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00009138 File Offset: 0x00007338
		public override void CompPostTick(ref float severityAdjustment)
		{
			this.ticksToDisappear--;
			if (this.ticksToDisappear > 0)
			{
				HediffSet MShedSet = this.parent.pawn.health.hediffSet;
				if (MShedSet != null)
				{
					if ((Def.defName == "CatatonicBreakdown" || Def.defName == "PsychicShock") && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High"), false)) != null)
					{
						this.ticksToDisappear--;
					}
					if (Def.defName == "Unease" || Def.defName == "SuicidePreparation")
					{
						bool flag = (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSClarity_High"), false)) != null;
						Hediff MSCheckRimzac = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimzac_High"), false);
						if (flag || MSCheckRimzac != null)
						{
							this.ticksToDisappear--;
						}
					}
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00009240 File Offset: 0x00007440
		public override void CompPostMerged(Hediff other)
		{
			base.CompPostMerged(other);
			MSHDComp_AntiPsychotic MSCD = other.TryGetComp<MSHDComp_AntiPsychotic>();
			if (MSCD != null && MSCD.ticksToDisappear > this.ticksToDisappear)
			{
				this.ticksToDisappear = MSCD.ticksToDisappear;
			}
		}
	}
}
