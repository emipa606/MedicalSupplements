using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSAddict
{
	// Token: 0x02000028 RID: 40
	public class HediffComp_MSAddict : HediffComp
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000950A File Offset: 0x0000770A
		public HediffCompProperties_MSAddict MSProps
		{
			get
			{
				return (HediffCompProperties_MSAddict)this.props;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00009518 File Offset: 0x00007718
		public override void CompPostTick(ref float severityAdjustment)
		{
			if ((Find.TickManager.TicksGame + Pawn.HashOffset()) % 2500 == 0)
			{
				List<ChemicalDef> Chemicals = DefDatabase<ChemicalDef>.AllDefsListForReading;
				Pawn pawn = Pawn;
				HediffSet hediffSet;
				if (pawn == null)
				{
					hediffSet = null;
				}
				else
				{
					Pawn_HealthTracker health = pawn.health;
					hediffSet = (health?.hediffSet);
				}
				HediffSet set = hediffSet;
				if (Chemicals != null && Chemicals.Count > 0)
				{
					foreach (ChemicalDef Chemical in Chemicals)
					{
						if (!(Chemical.defName == "MSMental"))
						{
							HediffDef addiction = Chemical?.addictionHediff;
							if (addiction != null && set != null)
							{
								Hediff chkaddiction = set.GetFirstHediffOfDef(addiction, false);
								if (chkaddiction != null)
								{
									this.ReduceHediff(chkaddiction, this.MSProps.AddictionLossPerHour);
								}
							}
							HediffDef tolerance = Chemical?.toleranceHediff;
							if (tolerance != null && set != null)
							{
								Hediff chktolerance = set.GetFirstHediffOfDef(tolerance, false);
								if (chktolerance != null)
								{
									this.ReduceHediff(chktolerance, this.MSProps.ToleranceLossPerHour);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00009640 File Offset: 0x00007840
		public void ReduceHediff(Hediff h, float sev)
		{
			float hsev = h.Severity;
			if (h.def.defName == "LuciferiumAddiction")
			{
				sev = Math.Max(0.005f, sev / 2f);
			}
			if (sev >= hsev)
			{
                Pawn.health.RemoveHediff(h);
				return;
			}
			h.Severity -= sev;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000096A2 File Offset: 0x000078A2
		public override string CompDebugString()
		{
			return "addiction loss per hr: " + this.MSProps.AddictionLossPerHour.ToString("F3") + "\ntolerance loss per hr: " + this.MSProps.ToleranceLossPerHour.ToString("F3");
		}
	}
}
