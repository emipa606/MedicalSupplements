using System;
using RimWorld;
using Verse;

namespace MSExotic
{
	// Token: 0x02000015 RID: 21
	public class CompUseEffect_MSExotic : CompUseEffect
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00005450 File Offset: 0x00003650
		public override void DoEffect(Pawn usedBy)
		{
			base.DoEffect(usedBy);
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSImmunisation)
			{
				MSExoticUtility.DoMSImmunisation(usedBy, this.parent.def);
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCerebrax)
			{
				MSExoticUtility.DoMSCerebrax(usedBy, this.parent.def);
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSBattleStim)
			{
				MSExoticUtility.DoMSBattleStim(usedBy, this.parent.def);
			}
			if (MSExoticUtility.GetIsTranscendence(this.parent.def))
			{
				MSExoticUtility.DoMSTranscendence(usedBy, this.parent.def);
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSPerpetuity)
			{
				MSExoticUtility.DoMSPerpetuity(usedBy, this.parent.def);
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCondom)
			{
				MSExoticUtility.DoMSCondom(usedBy, this.parent.def);
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005538 File Offset: 0x00003738
		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			failReason = null;
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSImmunisation)
			{
                MSExoticUtility.ChkMSImmunisation(p, out string Reason, out bool Passed);
                if (!Passed)
				{
					failReason = Reason;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCerebrax)
			{
                MSExoticUtility.ChkMSCerebrax(p, out string Reason2, out bool Passed2);
                if (!Passed2)
				{
					failReason = Reason2;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSBattleStim)
			{
                MSExoticUtility.ChkMSBattleStim(p, out string Reason3, out bool Passed3);
                if (!Passed3)
				{
					failReason = Reason3;
					return false;
				}
			}
			if (MSExoticUtility.GetIsTranscendence(this.parent.def))
			{
                MSExoticUtility.ChkMSTranscendence(p, this.parent.def, out string Reason4, out bool Passed4);
                if (!Passed4)
				{
					failReason = Reason4;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSPerpetuity)
			{
                MSExoticUtility.ChkMSPerpetuity(p, out string Reason5, out bool Passed5);
                if (!Passed5)
				{
					failReason = Reason5;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCondom)
			{
                MSExoticUtility.ChkMSCondom(p, out string Reason6, out bool Passed6);
                if (!Passed6)
				{
					failReason = Reason6;
					return false;
				}
			}
			return true;
		}
	}
}
