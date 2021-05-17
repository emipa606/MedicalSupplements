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
        // Token: 0x04000016 RID: 22
        public const float TendQualityRandomVariance = 0.25f;

        // Token: 0x04000017 RID: 23
        private static readonly Color UntendedColor = new ColorInt(116, 101, 72).ToColor;

        // Token: 0x04000018 RID: 24
        private static readonly Texture2D TendedIcon_Need_General =
            ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedNeed");

        // Token: 0x04000019 RID: 25
        private static readonly Texture2D TendedIcon_Well_General =
            ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedWell");

        // Token: 0x0400001A RID: 26
        private static readonly Texture2D TendedIcon_Well_Injury =
            ContentFinder<Texture2D>.Get("UI/Icons/Medical/BandageWell");

        // Token: 0x04000014 RID: 20
        public float tendQuality;

        // Token: 0x04000013 RID: 19
        public int tendTicksLeft = -1;

        // Token: 0x04000015 RID: 21
        private float totalTendQuality;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600001A RID: 26 RVA: 0x00002723 File Offset: 0x00000923
        public MSHediffCompProperties_TendDuration TProps => (MSHediffCompProperties_TendDuration) props;

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600001C RID: 28 RVA: 0x000027E5 File Offset: 0x000009E5
        public override bool CompShouldRemove => base.CompShouldRemove || TProps.disappearsAtTotalTendQuality >= 0 &&
            totalTendQuality >= TProps.disappearsAtTotalTendQuality;

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600001D RID: 29 RVA: 0x00002818 File Offset: 0x00000A18
        public bool IsTended => Current.ProgramState == ProgramState.Playing && tendTicksLeft > 0;

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600001E RID: 30 RVA: 0x0000282D File Offset: 0x00000A2D
        public bool AllowTend
        {
            get
            {
                if (TProps.TendIsPermanent)
                {
                    return !IsTended;
                }

                return TProps.TendTicksOverlap > tendTicksLeft;
            }
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x0600001F RID: 31 RVA: 0x0000285C File Offset: 0x00000A5C
        public override string CompTipStringExtra
        {
            get
            {
                if (parent.IsPermanent())
                {
                    return null;
                }

                var stringBuilder = new StringBuilder();
                if (!IsTended)
                {
                    if (!Pawn.Dead && parent.TendableNow())
                    {
                        stringBuilder.AppendLine("NeedsTendingNow".Translate());
                    }
                }
                else
                {
                    if (TProps.showTendQuality)
                    {
                        var text =
                            parent.Part != null && parent.Part.def.IsSolid(parent.Part, Pawn.health.hediffSet.hediffs)
                                ?
                                TProps.labelSolidTendedWell
                                : parent.Part == null || parent.Part.depth != BodyPartDepth.Inside
                                    ? TProps.labelTendedWell
                                    : TProps.labelTendedWellInner;
                        if (text != null)
                        {
                            stringBuilder.AppendLine(text.CapitalizeFirst() + " (" + "Quality".Translate().ToLower() +
                                                     " " + tendQuality.ToStringPercent("F0") + ")");
                        }
                        else
                        {
                            stringBuilder.AppendLine(string.Format("{0}: {1}", "TendQuality".Translate(),
                                tendQuality.ToStringPercent()));
                        }
                    }

                    if (!Pawn.Dead && !TProps.TendIsPermanent && parent.TendableNow(true))
                    {
                        var num = tendTicksLeft - TProps.TendTicksOverlap;
                        if (num < 0)
                        {
                            stringBuilder.AppendLine("CanTendNow".Translate());
                        }
                        else if ("NextTendIn".CanTranslate())
                        {
                            stringBuilder.AppendLine("NextTendIn".Translate(num.ToStringTicksToPeriod()));
                        }
                        else
                        {
                            stringBuilder.AppendLine("NextTreatmentIn".Translate(num.ToStringTicksToPeriod()));
                        }

                        stringBuilder.AppendLine("TreatmentExpiresIn".Translate(tendTicksLeft.ToStringTicksToPeriod()));
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
                if (parent is Hediff_Injury)
                {
                    if (IsTended && !parent.IsPermanent())
                    {
                        var color = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(tendQuality));
                        return new TextureAndColor(TendedIcon_Well_Injury, color);
                    }
                }
                else if (!(parent is Hediff_MissingPart) && !parent.FullyImmune())
                {
                    if (IsTended)
                    {
                        var color2 = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(tendQuality));
                        return new TextureAndColor(TendedIcon_Well_General, color2);
                    }

                    return TendedIcon_Need_General;
                }

                return TextureAndColor.None;
            }
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002730 File Offset: 0x00000930
        private void MSCure(Hediff MShediffToCure, Hediff MShediffCuredBy)
        {
            var pawn = MShediffToCure.pawn;
            pawn.health.RemoveHediff(MShediffToCure);
            if (MShediffToCure.def.cureAllAtOnceIfCuredByItem)
            {
                var num = 0;
                for (;;)
                {
                    num++;
                    if (num > 10000)
                    {
                        break;
                    }

                    var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(MShediffToCure.def);
                    if (firstHediffOfDef == null)
                    {
                        goto IL_62;
                    }

                    pawn.health.RemoveHediff(firstHediffOfDef);
                }

                Log.Error("Too many iterations.");
            }

            IL_62:
            Messages.Message(
                pawn.Label.CapitalizeFirst() + "'s condition of " + MShediffToCure.LabelBase.CapitalizeFirst() +
                " has been cured by " + MShediffCuredBy.LabelBase.CapitalizeFirst(), pawn,
                MessageTypeDefOf.PositiveEvent);
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002B8A File Offset: 0x00000D8A
        public override void CompExposeData()
        {
            Scribe_Values.Look(ref tendTicksLeft, "tendTicksLeft", -1);
            Scribe_Values.Look(ref tendQuality, "tendQuality");
            Scribe_Values.Look(ref totalTendQuality, "totalTendQuality");
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002BCA File Offset: 0x00000DCA
        protected override float SeverityChangePerDay()
        {
            if (IsTended)
            {
                return TProps.severityPerDayTended * tendQuality;
            }

            return 0f;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00002BEC File Offset: 0x00000DEC
        public override void CompTended_NewTemp(float quality, float maxQuality, int batchPosition = 0)
        {
            var MSAddQuality = 0f;
            var MShedSet = parent.pawn.health.hediffSet;
            if (Def.defName == "WoundInfection" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High")) != null)
            {
                MSAddQuality += 0.25f;
            }

            if (Def.defName == "Asthma" && MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSInhaler_High")) != null)
            {
                MSAddQuality += 0.2f;
            }

            if (Def.defName == "Flu" && MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High")) != null)
            {
                MSAddQuality += 0.25f;
            }

            if (Def.defName == "MuscleParasites" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh")) != null)
            {
                MSAddQuality += 0.15f;
            }

            if (Def.defName == "GutWorms" && MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh")) != null)
            {
                MSAddQuality += 0.18f;
            }

            if ((Def.defName == "Carcinoma" || Def.defName == "BloodCancer") &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High")) != null)
            {
                MSAddQuality += 0.25f;
            }

            if (Def.defName == "HepatitisK")
            {
                var flag = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh")) != null;
                var MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (flag || MSCheckDrug2 != null)
                {
                    MSAddQuality += 0.15f;
                }
            }

            if (Def.defName == "StomachUlcer")
            {
                var flag2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh")) != null;
                var MSCheckDrug3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (flag2 || MSCheckDrug3 != null || MSCheckDrug4 != null)
                {
                    MSAddQuality += 0.2f;
                }
            }

            if (Def.defName == "Tuberculosis" || Def.defName == "KindredDickVirus" || Def.defName == "Sepsis" ||
                Def.defName == "Toothache" || Def.defName == "VoightBernsteinDisease" ||
                Def.defName == "NewReschianFever")
            {
                var flag3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High")) != null;
                var MSCheckDrug5 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (flag3 || MSCheckDrug5 != null || MSCheckDrug6 != null)
                {
                    MSAddQuality += 0.15f;
                }
            }

            if (Def.defName == "Migraine")
            {
                var flag4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimCodamol_High")) != null;
                var MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMorphine_High"));
                var MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSOpiumPipe_High"));
                if (flag4)
                {
                    MSAddQuality += 0.25f;
                }

                if (MSCheckDrug7 != null || MSCheckDrug8 != null)
                {
                    MSAddQuality += 0.5f;
                }
            }

            tendQuality = Mathf.Clamp01(quality + Rand.Range(-0.25f + MSAddQuality, 0.25f + MSAddQuality));
            totalTendQuality += tendQuality;
            if (TProps.TendIsPermanent)
            {
                tendTicksLeft = 1;
            }
            else
            {
                tendTicksLeft = Mathf.Max(0, tendTicksLeft) + TProps.TendTicksFull;
            }

            if (batchPosition == 0 && Pawn.Spawned)
            {
                string text = "TextMote_Tended".Translate(parent.Label).CapitalizeFirst() + "\n" +
                              "Quality".Translate() + " " + tendQuality.ToStringPercent();
                MoteMaker.ThrowText(Pawn.DrawPos, Pawn.Map, text, Color.white, 3.65f);
            }

            Pawn.health.Notify_HediffChanged(parent);
        }

        // Token: 0x06000024 RID: 36 RVA: 0x000030B8 File Offset: 0x000012B8
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (tendTicksLeft > 0 && !TProps.TendIsPermanent)
            {
                tendTicksLeft--;
                if (tendTicksLeft > 0)
                {
                    var MSDebug = false;
                    var MShedSet = parent.pawn.health.hediffSet;
                    var MSOverComeLevel = 0.5f;
                    var MSRimpepticCureLevel = 0.01f;
                    var MSRimpepticCureTend = 0.85f;
                    var MSTicksSafeTendTime = 5000;
                    if (MSRimpepticCureTend < MSOverComeLevel)
                    {
                        MSRimpepticCureTend = MSOverComeLevel;
                    }

                    if (tendQuality >= MSOverComeLevel)
                    {
                        if (Def.defName == "Asthma" &&
                            MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSInhaler_High")) != null &&
                            Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                        {
                            if (MSDebug && tendTicksLeft % 250 == 0)
                            {
                                Messages.Message(
                                    string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                        " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                    MessageTypeDefOf.PositiveEvent);
                            }

                            tendTicksLeft++;
                        }

                        if (Def.defName == "Flu" &&
                            MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High")) != null &&
                            Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                        {
                            if (MSDebug && tendTicksLeft % 250 == 0)
                            {
                                Messages.Message(
                                    string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                        " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                    MessageTypeDefOf.PositiveEvent);
                            }

                            tendTicksLeft++;
                        }

                        if (Def.defName == "WoundInfection" &&
                            MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High")) != null &&
                            Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                        {
                            if (MSDebug && tendTicksLeft % 250 == 0)
                            {
                                Messages.Message(
                                    string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                        " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                    MessageTypeDefOf.PositiveEvent);
                            }

                            tendTicksLeft++;
                        }

                        if (tendTicksLeft > MSTicksSafeTendTime && Def.defName == "MuscleParasites" &&
                            MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh")) != null &&
                            Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                        {
                            if (MSDebug && tendTicksLeft % 250 == 0)
                            {
                                Messages.Message(
                                    string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                        " is REDUCING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                    MessageTypeDefOf.PositiveEvent);
                            }

                            tendTicksLeft--;
                        }

                        if (Def.defName == "GutWorms")
                        {
                            var MSCheckRimpeptic = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"));
                            if (MSCheckRimpeptic != null)
                            {
                                if (tendTicksLeft > MSTicksSafeTendTime &&
                                    Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                                {
                                    if (MSDebug && tendTicksLeft % 250 == 0)
                                    {
                                        Messages.Message(
                                            string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                                " is REDUCING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                            MessageTypeDefOf.PositiveEvent);
                                    }

                                    tendTicksLeft--;
                                }

                                if (tendTicksLeft > 0 &&
                                    (tendTicksLeft % 1000 == 0 || (tendTicksLeft + 1) % 1000 == 0) &&
                                    tendQuality >= MSRimpepticCureTend &&
                                    Rand.Range(0f, 1f - (tendQuality - MSRimpepticCureTend)) <= MSRimpepticCureLevel)
                                {
                                    var MSHediffGWToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("GutWorms"));
                                    if (MSHediffGWToGo != null)
                                    {
                                        MSCure(MSHediffGWToGo, MSCheckRimpeptic);
                                    }
                                }
                            }
                        }

                        if ((Def.defName == "BloodCancer" || Def.defName == "Carcinoma") &&
                            MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High")) != null &&
                            Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                        {
                            if (MSDebug && tendTicksLeft % 250 == 0)
                            {
                                Messages.Message(
                                    string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                        " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                    MessageTypeDefOf.PositiveEvent);
                            }

                            tendTicksLeft++;
                        }

                        if (Def.defName == "HepatitisK")
                        {
                            var MSCheckDrug = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                            var MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                            if ((MSCheckDrug != null || MSCheckDrug2 != null) &&
                                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                            {
                                if (MSDebug && tendTicksLeft % 250 == 0)
                                {
                                    Messages.Message(
                                        string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                            " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                        MessageTypeDefOf.PositiveEvent);
                                }

                                tendTicksLeft++;
                            }
                        }

                        if (Def.defName == "StomachUlcer")
                        {
                            var MSCheckDrug3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh"));
                            var MSCheckDrug4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                            var MSCheckDrug5 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                            if ((MSCheckDrug3 != null || MSCheckDrug4 != null || MSCheckDrug5 != null) &&
                                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                            {
                                if (MSDebug && tendTicksLeft % 250 == 0)
                                {
                                    Messages.Message(
                                        string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                            " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                        MessageTypeDefOf.PositiveEvent);
                                }

                                tendTicksLeft++;
                            }
                        }

                        if (Def.defName == "Tuberculosis" || Def.defName == "KindredDickVirus" ||
                            Def.defName == "Sepsis" || Def.defName == "Toothache" ||
                            Def.defName == "VoightBernsteinDisease" || Def.defName == "NewReschianFever")
                        {
                            var MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High"));
                            var MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                            var MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                            if ((MSCheckDrug6 != null || MSCheckDrug7 != null || MSCheckDrug8 != null) &&
                                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                            {
                                if (MSDebug && tendTicksLeft % 250 == 0)
                                {
                                    Messages.Message(
                                        string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                            " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                        MessageTypeDefOf.PositiveEvent);
                                }

                                tendTicksLeft++;
                            }
                        }

                        if (Def.defName == "Migraine")
                        {
                            var MSCheckDrug9 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimCodamol_High"));
                            var MSCheckDrug10 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMorphine_High"));
                            var MSCheckDrug11 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSOpiumPipe_High"));
                            if ((MSCheckDrug9 != null || MSCheckDrug10 != null || MSCheckDrug11 != null) &&
                                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                            {
                                if (MSDebug && tendTicksLeft % 250 == 0)
                                {
                                    Messages.Message(
                                        string.Concat("Debug: ", Def.label, " for ", Pawn.Label,
                                            " is INCREASING with tendqual ", tendQuality.ToString("N2")), Pawn,
                                        MessageTypeDefOf.PositiveEvent);
                                }

                                tendTicksLeft++;
                            }
                        }
                    }

                    if (Def.defName == "FibrousMechanites")
                    {
                        var MSCheckAntinites = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"));
                        if (MSCheckAntinites != null)
                        {
                            var MSHediffFMToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("FibrousMechanites"));
                            if (MSHediffFMToGo != null)
                            {
                                MSCure(MSHediffFMToGo, MSCheckAntinites);
                            }
                        }
                    }

                    if (Def.defName == "SensoryMechanites")
                    {
                        var MSCheckAntinites2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"));
                        if (MSCheckAntinites2 != null)
                        {
                            var MSHediffSMToGo = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("SensoryMechanites"));
                            if (MSHediffSMToGo != null)
                            {
                                MSCure(MSHediffSMToGo, MSCheckAntinites2);
                            }
                        }
                    }

                    if (Def.defName == "LymphaticMechanites")
                    {
                        var MSCheckAntinites3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"));
                        if (MSCheckAntinites3 != null)
                        {
                            var MSHediffSMToGo2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("LymphaticMechanites"));
                            if (MSHediffSMToGo2 != null)
                            {
                                MSCure(MSHediffSMToGo2, MSCheckAntinites3);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00003C90 File Offset: 0x00001E90
        public override string CompDebugString()
        {
            var stringBuilder = new StringBuilder();
            if (IsTended)
            {
                stringBuilder.AppendLine("tendQuality: " + tendQuality.ToStringPercent());
                if (!TProps.TendIsPermanent)
                {
                    stringBuilder.AppendLine("tendTicksLeft: " + tendTicksLeft);
                }
            }
            else
            {
                stringBuilder.AppendLine("untended");
            }

            stringBuilder.AppendLine("severity/day: " + SeverityChangePerDay());
            if (TProps.disappearsAtTotalTendQuality >= 0)
            {
                stringBuilder.AppendLine(string.Concat("totalTendQuality: ", totalTendQuality.ToString("F2"), " / ",
                    TProps.disappearsAtTotalTendQuality));
            }

            return stringBuilder.ToString().Trim();
        }
    }
}