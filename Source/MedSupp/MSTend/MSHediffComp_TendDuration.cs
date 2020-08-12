using System;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSTend
{
	// Token: 0x02000008 RID: 8
	[StaticConstructorOnStartup]
	public class MSHediffComp_TendDuration : HediffComp_SeverityPerDay
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002723 File Offset: 0x00000923
		public MSHediffCompProperties_TendDuration TProps
		{
			get
			{
				return (MSHediffCompProperties_TendDuration)this.props;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002730 File Offset: 0x00000930
		private void MSCure(Hediff MShediffToCure, Hediff MShediffCuredBy)
		{
			Pawn pawn = MShediffToCure.pawn;
			pawn.health.RemoveHediff(MShediffToCure);
			if (MShediffToCure.def.cureAllAtOnceIfCuredByItem)
			{
				int num = 0;
				for (;;)
				{
					num++;
					if (num > 10000)
					{
						break;
					}
					Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(MShediffToCure.def, false);
					if (firstHediffOfDef == null)
					{
						goto IL_62;
					}
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
				Log.Error("Too many iterations.", false);
			}
			IL_62:
			Messages.Message(pawn.Label.CapitalizeFirst() + "'s condition of " + MShediffToCure.LabelBase.CapitalizeFirst() + " has been cured by " + MShediffCuredBy.LabelBase.CapitalizeFirst(), pawn, MessageTypeDefOf.PositiveEvent, true);
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000027E5 File Offset: 0x000009E5
		public override bool CompShouldRemove
		{
			get
			{
				return base.CompShouldRemove || (this.TProps.disappearsAtTotalTendQuality >= 0 && this.totalTendQuality >= (float)this.TProps.disappearsAtTotalTendQuality);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002818 File Offset: 0x00000A18
		public bool IsTended
		{
			get
			{
				return Current.ProgramState == ProgramState.Playing && this.tendTicksLeft > 0;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001E RID: 30 RVA: 0x0000282D File Offset: 0x00000A2D
		public bool AllowTend
		{
			get
			{
				if (this.TProps.TendIsPermanent)
				{
					return !this.IsTended;
				}
				return this.TProps.TendTicksOverlap > this.tendTicksLeft;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000285C File Offset: 0x00000A5C
		public override string CompTipStringExtra
		{
			get
			{
				if (this.parent.IsPermanent())
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (!this.IsTended)
				{
					if (!Pawn.Dead && this.parent.TendableNow(false))
					{
						stringBuilder.AppendLine("NeedsTendingNow".Translate());
					}
				}
				else
				{
					if (this.TProps.showTendQuality)
					{
						string text = (this.parent.Part != null && this.parent.Part.def.IsSolid(this.parent.Part, Pawn.health.hediffSet.hediffs)) ? this.TProps.labelSolidTendedWell : ((this.parent.Part == null || this.parent.Part.depth != BodyPartDepth.Inside) ? this.TProps.labelTendedWell : this.TProps.labelTendedWellInner);
						if (text != null)
						{
							stringBuilder.AppendLine(text.CapitalizeFirst() + " (" + "Quality".Translate().ToLower() + " " + this.tendQuality.ToStringPercent("F0") + ")");
						}
						else
						{
							stringBuilder.AppendLine(string.Format("{0}: {1}", "TendQuality".Translate(), this.tendQuality.ToStringPercent()));
						}
					}
					if (!Pawn.Dead && !this.TProps.TendIsPermanent && this.parent.TendableNow(true))
					{
						int num = this.tendTicksLeft - this.TProps.TendTicksOverlap;
						if (num < 0)
						{
							stringBuilder.AppendLine("CanTendNow".Translate());
						}
						else if ("NextTendIn".CanTranslate())
						{
							stringBuilder.AppendLine("NextTendIn".Translate(num.ToStringTicksToPeriod(true, false, true, true)));
						}
						else
						{
							stringBuilder.AppendLine("NextTreatmentIn".Translate(num.ToStringTicksToPeriod(true, false, true, true)));
						}
						stringBuilder.AppendLine("TreatmentExpiresIn".Translate(this.tendTicksLeft.ToStringTicksToPeriod(true, false, true, true)));
					}
				}
				return stringBuilder.ToString().TrimEndNewlines();
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002AD8 File Offset: 0x00000CD8
		public override TextureAndColor CompStateIcon
		{
			get
			{
				if (this.parent is Hediff_Injury)
				{
					if (this.IsTended && !this.parent.IsPermanent())
					{
						Color color = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(this.tendQuality));
						return new TextureAndColor(TendedIcon_Well_Injury, color);
					}
				}
				else if (!(this.parent is Hediff_MissingPart) && !this.parent.FullyImmune())
				{
					if (this.IsTended)
					{
						Color color2 = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(this.tendQuality));
						return new TextureAndColor(TendedIcon_Well_General, color2);
					}
					return TendedIcon_Need_General;
				}
				return TextureAndColor.None;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B8A File Offset: 0x00000D8A
		public override void CompExposeData()
		{
			Scribe_Values.Look(ref this.tendTicksLeft, "tendTicksLeft", -1, false);
			Scribe_Values.Look(ref this.tendQuality, "tendQuality", 0f, false);
			Scribe_Values.Look(ref this.totalTendQuality, "totalTendQuality", 0f, false);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002BCA File Offset: 0x00000DCA
		protected override float SeverityChangePerDay()
		{
			if (this.IsTended)
			{
				return this.TProps.severityPerDayTended * this.tendQuality;
			}
			return 0f;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002BEC File Offset: 0x00000DEC
		public override void CompTended_NewTemp(float quality, float maxQuality, int batchPosition = 0)
		{
			float MSAddQuality = 0f;
			HediffSet MShedSet = this.parent.pawn.health.hediffSet;
			if (Def.defName == "WoundInfection" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High"), false)) != null)
			{
				MSAddQuality += 0.25f;
			}
			if (Def.defName == "Asthma" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSInhaler_High"), false)) != null)
			{
				MSAddQuality += 0.2f;
			}
			if (Def.defName == "Flu" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High"), false)) != null)
			{
				MSAddQuality += 0.25f;
			}
			if (Def.defName == "MuscleParasites" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh"), false)) != null)
			{
				MSAddQuality += 0.15f;
			}
			if (Def.defName == "GutWorms" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"), false)) != null)
			{
				MSAddQuality += 0.18f;
			}
			if ((Def.defName == "Carcinoma" || Def.defName == "BloodCancer") && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High"), false)) != null)
			{
				MSAddQuality += 0.25f;
			}
			if (Def.defName == "HepatitisK")
			{
				bool flag = (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false)) != null;
				Hediff MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
				if (flag || MSCheckDrug2 != null)
				{
					MSAddQuality += 0.15f;
				}
			}
			if (Def.defName == "StomachUlcer")
			{
				bool flag2 = (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"), false)) != null;
				Hediff MSCheckDrug3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false);
				Hediff MSCheckDrug4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
				if (flag2 || MSCheckDrug3 != null || MSCheckDrug4 != null)
				{
					MSAddQuality += 0.2f;
				}
			}
			if (Def.defName == "Tuberculosis" || Def.defName == "KindredDickVirus" || Def.defName == "Sepsis" || Def.defName == "Toothache" || Def.defName == "VoightBernsteinDisease" || Def.defName == "NewReschianFever")
			{
				bool flag3 = (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High"), false)) != null;
				Hediff MSCheckDrug5 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false);
				Hediff MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
				if (flag3 || MSCheckDrug5 != null || MSCheckDrug6 != null)
				{
					MSAddQuality += 0.15f;
				}
			}
			if (Def.defName == "Migraine")
			{
				bool flag4 = (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimCodamol_High"), false)) != null;
				Hediff MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMorphine_High"), false);
				Hediff MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSOpiumPipe_High"), false);
				if (flag4)
				{
					MSAddQuality += 0.25f;
				}
				if (MSCheckDrug7 != null || MSCheckDrug8 != null)
				{
					MSAddQuality += 0.5f;
				}
			}
			this.tendQuality = Mathf.Clamp01(quality + Rand.Range(-0.25f + MSAddQuality, 0.25f + MSAddQuality));
			this.totalTendQuality += this.tendQuality;
			if (this.TProps.TendIsPermanent)
			{
				this.tendTicksLeft = 1;
			}
			else
			{
				this.tendTicksLeft = Mathf.Max(0, this.tendTicksLeft) + this.TProps.TendTicksFull;
			}
			if (batchPosition == 0 && Pawn.Spawned)
			{
				string text = "TextMote_Tended".Translate(this.parent.Label).CapitalizeFirst() + "\n" + "Quality".Translate() + " " + this.tendQuality.ToStringPercent();
				MoteMaker.ThrowText(Pawn.DrawPos, Pawn.Map, text, Color.white, 3.65f);
			}
            Pawn.health.Notify_HediffChanged(this.parent);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000030B8 File Offset: 0x000012B8
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (this.tendTicksLeft > 0 && !this.TProps.TendIsPermanent)
			{
				this.tendTicksLeft--;
				if (this.tendTicksLeft > 0)
				{
					bool MSDebug = false;
					HediffSet MShedSet = this.parent.pawn.health.hediffSet;
					float MSOverComeLevel = 0.5f;
					float MSRimpepticCureLevel = 0.01f;
					float MSRimpepticCureTend = 0.85f;
					int MSTicksSafeTendTime = 5000;
					if (MSRimpepticCureTend < MSOverComeLevel)
					{
						MSRimpepticCureTend = MSOverComeLevel;
					}
					if (this.tendQuality >= MSOverComeLevel)
					{
						if (Def.defName == "Asthma" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSInhaler_High"), false)) != null && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
						{
							if (MSDebug && this.tendTicksLeft % 250 == 0)
							{
								Messages.Message(string.Concat(new string[]
								{
									"Debug: ",
                                    Def.label,
									" for ",
                                    Pawn.Label,
									" is INCREASING with tendqual ",
									this.tendQuality.ToString("N2")
								}), Pawn, MessageTypeDefOf.PositiveEvent, true);
							}
							this.tendTicksLeft++;
						}
						if (Def.defName == "Flu" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High"), false)) != null && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
						{
							if (MSDebug && this.tendTicksLeft % 250 == 0)
							{
								Messages.Message(string.Concat(new string[]
								{
									"Debug: ",
                                    Def.label,
									" for ",
                                    Pawn.Label,
									" is INCREASING with tendqual ",
									this.tendQuality.ToString("N2")
								}), Pawn, MessageTypeDefOf.PositiveEvent, true);
							}
							this.tendTicksLeft++;
						}
						if (Def.defName == "WoundInfection" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High"), false)) != null && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
						{
							if (MSDebug && this.tendTicksLeft % 250 == 0)
							{
								Messages.Message(string.Concat(new string[]
								{
									"Debug: ",
                                    Def.label,
									" for ",
                                    Pawn.Label,
									" is INCREASING with tendqual ",
									this.tendQuality.ToString("N2")
								}), Pawn, MessageTypeDefOf.PositiveEvent, true);
							}
							this.tendTicksLeft++;
						}
						if (this.tendTicksLeft > MSTicksSafeTendTime && Def.defName == "MuscleParasites" && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh"), false)) != null && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
						{
							if (MSDebug && this.tendTicksLeft % 250 == 0)
							{
								Messages.Message(string.Concat(new string[]
								{
									"Debug: ",
                                    Def.label,
									" for ",
                                    Pawn.Label,
									" is REDUCING with tendqual ",
									this.tendQuality.ToString("N2")
								}), Pawn, MessageTypeDefOf.PositiveEvent, true);
							}
							this.tendTicksLeft--;
						}
						if (Def.defName == "GutWorms")
						{
							Hediff MSCheckRimpeptic = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"), false);
							if (MSCheckRimpeptic != null)
							{
								if (this.tendTicksLeft > MSTicksSafeTendTime && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
								{
									if (MSDebug && this.tendTicksLeft % 250 == 0)
									{
										Messages.Message(string.Concat(new string[]
										{
											"Debug: ",
                                            Def.label,
											" for ",
                                            Pawn.Label,
											" is REDUCING with tendqual ",
											this.tendQuality.ToString("N2")
										}), Pawn, MessageTypeDefOf.PositiveEvent, true);
									}
									this.tendTicksLeft--;
								}
								if (this.tendTicksLeft > 0 && (this.tendTicksLeft % 1000 == 0 || (this.tendTicksLeft + 1) % 1000 == 0) && this.tendQuality >= MSRimpepticCureTend && Rand.Range(0f, 1f - (this.tendQuality - MSRimpepticCureTend)) <= MSRimpepticCureLevel)
								{
									Hediff MSHediffGWToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("GutWorms"), false);
									if (MSHediffGWToGo != null)
									{
										this.MSCure(MSHediffGWToGo, MSCheckRimpeptic);
									}
								}
							}
						}
						if ((Def.defName == "BloodCancer" || Def.defName == "Carcinoma") && (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High"), false)) != null && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
						{
							if (MSDebug && this.tendTicksLeft % 250 == 0)
							{
								Messages.Message(string.Concat(new string[]
								{
									"Debug: ",
                                    Def.label,
									" for ",
                                    Pawn.Label,
									" is INCREASING with tendqual ",
									this.tendQuality.ToString("N2")
								}), Pawn, MessageTypeDefOf.PositiveEvent, true);
							}
							this.tendTicksLeft++;
						}
						if (Def.defName == "HepatitisK")
						{
							Hediff MSCheckDrug = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false);
							Hediff MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
							if ((MSCheckDrug != null || MSCheckDrug2 != null) && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
							{
								if (MSDebug && this.tendTicksLeft % 250 == 0)
								{
									Messages.Message(string.Concat(new string[]
									{
										"Debug: ",
                                        Def.label,
										" for ",
                                        Pawn.Label,
										" is INCREASING with tendqual ",
										this.tendQuality.ToString("N2")
									}), Pawn, MessageTypeDefOf.PositiveEvent, true);
								}
								this.tendTicksLeft++;
							}
						}
						if (Def.defName == "StomachUlcer")
						{
							Hediff MSCheckDrug3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"), false);
							Hediff MSCheckDrug4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false);
							Hediff MSCheckDrug5 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
							if ((MSCheckDrug3 != null || MSCheckDrug4 != null || MSCheckDrug5 != null) && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
							{
								if (MSDebug && this.tendTicksLeft % 250 == 0)
								{
									Messages.Message(string.Concat(new string[]
									{
										"Debug: ",
                                        Def.label,
										" for ",
                                        Pawn.Label,
										" is INCREASING with tendqual ",
										this.tendQuality.ToString("N2")
									}), Pawn, MessageTypeDefOf.PositiveEvent, true);
								}
								this.tendTicksLeft++;
							}
						}
						if (Def.defName == "Tuberculosis" || Def.defName == "KindredDickVirus" || Def.defName == "Sepsis" || Def.defName == "Toothache" || Def.defName == "VoightBernsteinDisease" || Def.defName == "NewReschianFever")
						{
							Hediff MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High"), false);
							Hediff MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"), false);
							Hediff MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"), false);
							if ((MSCheckDrug6 != null || MSCheckDrug7 != null || MSCheckDrug8 != null) && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
							{
								if (MSDebug && this.tendTicksLeft % 250 == 0)
								{
									Messages.Message(string.Concat(new string[]
									{
										"Debug: ",
                                        Def.label,
										" for ",
                                        Pawn.Label,
										" is INCREASING with tendqual ",
										this.tendQuality.ToString("N2")
									}), Pawn, MessageTypeDefOf.PositiveEvent, true);
								}
								this.tendTicksLeft++;
							}
						}
						if (Def.defName == "Migraine")
						{
							Hediff MSCheckDrug9 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimCodamol_High"), false);
							Hediff MSCheckDrug10 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMorphine_High"), false);
							Hediff MSCheckDrug11 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSOpiumPipe_High"), false);
							if ((MSCheckDrug9 != null || MSCheckDrug10 != null || MSCheckDrug11 != null) && Rand.Range(0f, this.tendQuality) >= MSOverComeLevel)
							{
								if (MSDebug && this.tendTicksLeft % 250 == 0)
								{
									Messages.Message(string.Concat(new string[]
									{
										"Debug: ",
                                        Def.label,
										" for ",
                                        Pawn.Label,
										" is INCREASING with tendqual ",
										this.tendQuality.ToString("N2")
									}), Pawn, MessageTypeDefOf.PositiveEvent, true);
								}
								this.tendTicksLeft++;
							}
						}
					}
					if (Def.defName == "FibrousMechanites")
					{
						Hediff MSCheckAntinites = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"), false);
						if (MSCheckAntinites != null)
						{
							Hediff MSHediffFMToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("FibrousMechanites"), false);
							if (MSHediffFMToGo != null)
							{
								this.MSCure(MSHediffFMToGo, MSCheckAntinites);
							}
						}
					}
					if (Def.defName == "SensoryMechanites")
					{
						Hediff MSCheckAntinites2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"), false);
						if (MSCheckAntinites2 != null)
						{
							Hediff MSHediffSMToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("SensoryMechanites"), false);
							if (MSHediffSMToGo != null)
							{
								this.MSCure(MSHediffSMToGo, MSCheckAntinites2);
							}
						}
					}
					if (Def.defName == "LymphaticMechanites")
					{
						Hediff MSCheckAntinites3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"), false);
						if (MSCheckAntinites3 != null)
						{
							Hediff MSHediffSMToGo2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("LymphaticMechanites"), false);
							if (MSHediffSMToGo2 != null)
							{
								this.MSCure(MSHediffSMToGo2, MSCheckAntinites3);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003C90 File Offset: 0x00001E90
		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsTended)
			{
				stringBuilder.AppendLine("tendQuality: " + this.tendQuality.ToStringPercent());
				if (!this.TProps.TendIsPermanent)
				{
					stringBuilder.AppendLine("tendTicksLeft: " + this.tendTicksLeft);
				}
			}
			else
			{
				stringBuilder.AppendLine("untended");
			}
			stringBuilder.AppendLine("severity/day: " + this.SeverityChangePerDay().ToString());
			if (this.TProps.disappearsAtTotalTendQuality >= 0)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"totalTendQuality: ",
					this.totalTendQuality.ToString("F2"),
					" / ",
					this.TProps.disappearsAtTotalTendQuality
				}));
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x04000013 RID: 19
		public int tendTicksLeft = -1;

		// Token: 0x04000014 RID: 20
		public float tendQuality;

		// Token: 0x04000015 RID: 21
		private float totalTendQuality;

		// Token: 0x04000016 RID: 22
		public const float TendQualityRandomVariance = 0.25f;

		// Token: 0x04000017 RID: 23
		private static readonly Color UntendedColor = new ColorInt(116, 101, 72).ToColor;

		// Token: 0x04000018 RID: 24
		private static readonly Texture2D TendedIcon_Need_General = ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedNeed", true);

		// Token: 0x04000019 RID: 25
		private static readonly Texture2D TendedIcon_Well_General = ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedWell", true);

		// Token: 0x0400001A RID: 26
		private static readonly Texture2D TendedIcon_Well_Injury = ContentFinder<Texture2D>.Get("UI/Icons/Medical/BandageWell", true);
	}
}
