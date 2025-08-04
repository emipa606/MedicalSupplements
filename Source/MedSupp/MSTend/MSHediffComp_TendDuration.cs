using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSTend;

[StaticConstructorOnStartup]
public class MSHediffComp_TendDuration : HediffComp_SeverityModifierBase
{
    public const float TendQualityRandomVariance = 0.25f;

    private static readonly Color UntendedColor = new ColorInt(116, 101, 72).ToColor;

    private static readonly Texture2D TendedIcon_Need_General =
        ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedNeed");

    private static readonly Texture2D TendedIcon_Well_General =
        ContentFinder<Texture2D>.Get("UI/Icons/Medical/TendedWell");

    private static readonly Texture2D TendedIcon_Well_Injury =
        ContentFinder<Texture2D>.Get("UI/Icons/Medical/BandageWell");

    private float tendQuality;

    private int tendTicksLeft = -1;

    private float totalTendQuality;

    private MSHediffCompProperties_TendDuration TProps => (MSHediffCompProperties_TendDuration)props;

    public override bool CompShouldRemove => base.CompShouldRemove || TProps.disappearsAtTotalTendQuality >= 0 &&
        totalTendQuality >= TProps.disappearsAtTotalTendQuality;

    private bool IsTended => Current.ProgramState == ProgramState.Playing && tendTicksLeft > 0;

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

                return stringBuilder.ToString().TrimEndNewlines();
            }

            if (TProps.showTendQuality)
            {
                var text =
                    parent.Part != null && parent.Part.def.IsSolid(parent.Part, Pawn.health.hediffSet.hediffs)
                        ? TProps.labelSolidTendedWell
                        : parent.Part is not { depth: BodyPartDepth.Inside }
                            ? TProps.labelTendedWell
                            : TProps.labelTendedWellInner;
                if (text != null)
                {
                    stringBuilder.AppendLine($"{text.CapitalizeFirst()} (" + "Quality".Translate().ToLower() +
                                             " " + tendQuality.ToStringPercent("F0") + ")");
                }
                else
                {
                    stringBuilder.AppendLine($"{"TendQuality".Translate()}: {tendQuality.ToStringPercent()}");
                }
            }

            if (Pawn.Dead || TProps.TendIsPermanent || !parent.TendableNow(true))
            {
                return stringBuilder.ToString().TrimEndNewlines();
            }

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

            return stringBuilder.ToString().TrimEndNewlines();
        }
    }

    public override TextureAndColor CompStateIcon
    {
        get
        {
            if (parent is Hediff_Injury)
            {
                if (!IsTended || parent.IsPermanent())
                {
                    return TextureAndColor.None;
                }

                var color = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(tendQuality));
                return new TextureAndColor(TendedIcon_Well_Injury, color);
            }

            if (parent is Hediff_MissingPart || parent.FullyImmune())
            {
                return TextureAndColor.None;
            }

            if (!IsTended)
            {
                return TendedIcon_Need_General;
            }

            var color2 = Color.Lerp(UntendedColor, Color.white, Mathf.Clamp01(tendQuality));
            return new TextureAndColor(TendedIcon_Well_General, color2);
        }
    }

    private static void MSCure(Hediff MShediffToCure, Hediff MShediffCuredBy)
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
            $"{pawn.Label.CapitalizeFirst()}'s condition of {MShediffToCure.LabelBase.CapitalizeFirst()} has been cured by {MShediffCuredBy.LabelBase.CapitalizeFirst()}",
            pawn,
            MessageTypeDefOf.PositiveEvent);
    }

    public override void CompExposeData()
    {
        Scribe_Values.Look(ref tendTicksLeft, "tendTicksLeft", -1);
        Scribe_Values.Look(ref tendQuality, "tendQuality");
        Scribe_Values.Look(ref totalTendQuality, "totalTendQuality");
    }

    public override float SeverityChangePerDay()
    {
        if (IsTended)
        {
            return TProps.severityPerDayTended * tendQuality;
        }

        return 0f;
    }

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        var MSAddQuality = 0f;
        var MShedSet = parent.pawn.health.hediffSet;
        switch (Def.defName)
        {
            case "WoundInfection" when
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High")) != null:
                MSAddQuality += 0.25f;
                break;
            case "Asthma" when MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSInhaler_High")) != null:
                MSAddQuality += 0.2f;
                break;
            case "Flu" when MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High")) != null:
                MSAddQuality += 0.25f;
                break;
            case "MuscleParasites" when
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh")) != null:
                MSAddQuality += 0.15f;
                break;
            case "GutWorms" when MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh")) != null:
                MSAddQuality += 0.18f;
                break;
            case "Carcinoma" or "BloodCancer" when
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High")) != null:
                MSAddQuality += 0.25f;
                break;
            case "HepatitisK":
            {
                var MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh")) != null || MSCheckDrug2 != null)
                {
                    MSAddQuality += 0.15f;
                }

                break;
            }
            case "StomachUlcer":
            {
                var MSCheckDrug3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug4 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimpepticHigh")) != null || MSCheckDrug3 != null ||
                    MSCheckDrug4 != null)
                {
                    MSAddQuality += 0.2f;
                }

                break;
            }
            case "Tuberculosis" or "KindredDickVirus" or "Sepsis" or "Toothache" or "VoightBernsteinDisease"
                or "NewReschianFever":
            {
                var MSCheckDrug5 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High")) != null ||
                    MSCheckDrug5 != null || MSCheckDrug6 != null)
                {
                    MSAddQuality += 0.15f;
                }

                break;
            }
            case "Migraine":
            {
                var MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMorphine_High"));
                var MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSOpiumPipe_High"));
                if (MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimCodamol_High")) != null)
                {
                    MSAddQuality += 0.25f;
                }

                if (MSCheckDrug7 != null || MSCheckDrug8 != null)
                {
                    MSAddQuality += 0.5f;
                }

                break;
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

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (tendTicksLeft <= 0 || TProps.TendIsPermanent)
        {
            return;
        }

        tendTicksLeft--;
        if (tendTicksLeft <= 0)
        {
            return;
        }

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
                tendTicksLeft++;
            }

            if (Def.defName == "Flu" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSFireThroat_High")) != null &&
                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
            {
                tendTicksLeft++;
            }

            if (Def.defName == "WoundInfection" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimedicrem_High")) != null &&
                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
            {
                tendTicksLeft++;
            }

            if (tendTicksLeft > MSTicksSafeTendTime && Def.defName == "MuscleParasites" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimtarolHigh")) != null &&
                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
            {
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
                        tendTicksLeft--;
                    }

                    if (tendTicksLeft > 0 &&
                        (tendTicksLeft % 1000 == 0 || (tendTicksLeft + 1) % 1000 == 0) &&
                        tendQuality >= MSRimpepticCureTend &&
                        Rand.Range(0f, 1f - (tendQuality - MSRimpepticCureTend)) <= MSRimpepticCureLevel)
                    {
                        var MSHediffGWToGo = MShedSet.GetFirstHediffOfDef(HediffDef.Named("GutWorms"));
                        if (MSHediffGWToGo != null)
                        {
                            MSCure(MSHediffGWToGo, MSCheckRimpeptic);
                        }
                    }
                }
            }

            if (Def.defName is "BloodCancer" or "Carcinoma" &&
                MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSVinacol_High")) != null &&
                Rand.Range(0f, tendQuality) >= MSOverComeLevel)
            {
                tendTicksLeft++;
            }

            if (Def.defName == "HepatitisK")
            {
                var MSCheckDrug = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug2 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if ((MSCheckDrug != null || MSCheckDrug2 != null) &&
                    Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                {
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
                    tendTicksLeft++;
                }
            }

            if (Def.defName is "Tuberculosis" or "KindredDickVirus" or "Sepsis" or "Toothache"
                or "VoightBernsteinDisease" or "NewReschianFever")
            {
                var MSCheckDrug6 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSRimoxicillin_High"));
                var MSCheckDrug7 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSMetasisHigh"));
                var MSCheckDrug8 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSBattleStim_High"));
                if ((MSCheckDrug6 != null || MSCheckDrug7 != null || MSCheckDrug8 != null) &&
                    Rand.Range(0f, tendQuality) >= MSOverComeLevel)
                {
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
                    tendTicksLeft++;
                }
            }
        }

        if (Def.defName == "FibrousMechanites")
        {
            var MSCheckAntinites = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"));
            if (MSCheckAntinites != null)
            {
                var MSHediffFMToGo = MShedSet.GetFirstHediffOfDef(HediffDef.Named("FibrousMechanites"));
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
                var MSHediffSMToGo = MShedSet.GetFirstHediffOfDef(HediffDef.Named("SensoryMechanites"));
                if (MSHediffSMToGo != null)
                {
                    MSCure(MSHediffSMToGo, MSCheckAntinites2);
                }
            }
        }

        if (Def.defName != "LymphaticMechanites")
        {
            return;
        }

        var MSCheckAntinites3 = MShedSet?.GetFirstHediffOfDef(HediffDef.Named("MSAntinitesHigh"));
        if (MSCheckAntinites3 == null)
        {
            return;
        }

        var MSHediffSMToGo2 = MShedSet.GetFirstHediffOfDef(HediffDef.Named("LymphaticMechanites"));
        if (MSHediffSMToGo2 != null)
        {
            MSCure(MSHediffSMToGo2, MSCheckAntinites3);
        }
    }

    public override string CompDebugString()
    {
        var stringBuilder = new StringBuilder();
        if (IsTended)
        {
            stringBuilder.AppendLine($"tendQuality: {tendQuality.ToStringPercent()}");
            if (!TProps.TendIsPermanent)
            {
                stringBuilder.AppendLine($"tendTicksLeft: {tendTicksLeft}");
            }
        }
        else
        {
            stringBuilder.AppendLine("untended");
        }

        stringBuilder.AppendLine($"severity/day: {SeverityChangePerDay()}");
        if (TProps.disappearsAtTotalTendQuality >= 0)
        {
            stringBuilder.AppendLine(
                $"totalTendQuality: {totalTendQuality:F2} / {TProps.disappearsAtTotalTendQuality}");
        }

        return stringBuilder.ToString().Trim();
    }
}