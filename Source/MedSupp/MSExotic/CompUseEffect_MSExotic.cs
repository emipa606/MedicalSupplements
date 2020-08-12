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
				string Reason;
				bool Passed;
				MSExoticUtility.ChkMSImmunisation(p, out Reason, out Passed);
				if (!Passed)
				{
					failReason = Reason;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCerebrax)
			{
				string Reason2;
				bool Passed2;
				MSExoticUtility.ChkMSCerebrax(p, out Reason2, out Passed2);
				if (!Passed2)
				{
					failReason = Reason2;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSBattleStim)
			{
				string Reason3;
				bool Passed3;
				MSExoticUtility.ChkMSBattleStim(p, out Reason3, out Passed3);
				if (!Passed3)
				{
					failReason = Reason3;
					return false;
				}
			}
			if (MSExoticUtility.GetIsTranscendence(this.parent.def))
			{
				string Reason4;
				bool Passed4;
				MSExoticUtility.ChkMSTranscendence(p, this.parent.def, out Reason4, out Passed4);
				if (!Passed4)
				{
					failReason = Reason4;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSPerpetuity)
			{
				string Reason5;
				bool Passed5;
				MSExoticUtility.ChkMSPerpetuity(p, out Reason5, out Passed5);
				if (!Passed5)
				{
					failReason = Reason5;
					return false;
				}
			}
			if (this.parent.def == MSExoticDefOf.ThingDefOf.MSCondom)
			{
				string Reason6;
				bool Passed6;
				MSExoticUtility.ChkMSCondom(p, out Reason6, out Passed6);
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
