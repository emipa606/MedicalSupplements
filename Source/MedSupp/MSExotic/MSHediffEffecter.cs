using System;
using System.Collections.Generic;
using Verse;

namespace MSExotic
{
	// Token: 0x02000019 RID: 25
	internal class MSHediffEffecter
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00005F24 File Offset: 0x00004124
		internal static bool HediffEffect(HediffDef hediffdef, float SeverityToApply, Pawn pawn, BodyPartRecord part, out bool immune)
		{
			immune = false;
			if (!pawn.RaceProps.IsMechanoid && hediffdef != null)
			{
				if (!ImmuneTo(pawn, hediffdef))
				{
					if (!pawn.health.WouldDieAfterAddingHediff(hediffdef, part, SeverityToApply))
					{
						Pawn_HealthTracker health = pawn.health;
						Hediff hediff;
						if (health == null)
						{
							hediff = null;
						}
						else
						{
							HediffSet hediffSet = health.hediffSet;
							hediff = (hediffSet?.GetFirstHediffOfDef(hediffdef, false));
						}
						Hediff hashediff = hediff;
						if (hashediff != null)
						{
							hashediff.Severity += SeverityToApply;
							return true;
						}
						Hediff addhediff = HediffMaker.MakeHediff(hediffdef, pawn, part);
						addhediff.Severity = SeverityToApply;
						pawn.health.AddHediff(addhediff, part, null, null);
						return true;
					}
				}
				else
				{
					immune = true;
				}
			}
			return false;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005FC0 File Offset: 0x000041C0
		internal static bool ImmuneTo(Pawn pawn, HediffDef def)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				HediffStage curStage = hediffs[i].CurStage;
				if (curStage != null && curStage.makeImmuneTo != null)
				{
					for (int j = 0; j < curStage.makeImmuneTo.Count; j++)
					{
						if (curStage.makeImmuneTo[j] == def)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006030 File Offset: 0x00004230
		internal static bool HasHediff(Pawn pawn, HediffDef def)
		{
			Pawn_HealthTracker health = pawn.health;
			HediffSet HS = health?.hediffSet;
			return HS != null && HS.GetFirstHediffOfDef(def, false) != null;
		}
	}
}
