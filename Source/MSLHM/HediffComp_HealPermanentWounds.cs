using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MSLHM;

public class HediffComp_HealPermanentWounds : HediffComp
{
    private readonly HashSet<string> chronicConditions =
    [
        "Blindness",
        "TraumaSavant",
        "Cirrhosis",
        "ChemicalDamageSevere",
        "ChemicalDamageModerate",
        "HepatitisK"
    ];

    private int ticksToHeal;

    public HediffComp_HealPermanentWounds()
    {
        foreach (var hediffGiverSetDef in DefDatabase<HediffGiverSetDef>.AllDefsListForReading)
        {
            hediffGiverSetDef.hediffGivers.FindAll(hg => hg.GetType() == typeof(HediffGiver_Birthday))
                .ForEach(delegate(HediffGiver hg) { chronicConditions.Add(hg.hediff.defName); });
        }

        Log.Message(string.Join(", ", chronicConditions.ToArray()));
    }

    public HediffCompProperties_HealPermanentWounds Props => (HediffCompProperties_HealPermanentWounds)props;

    public override void CompPostMake()
    {
        base.CompPostMake();
        ResetTicksToHeal();
    }

    public void ResetTicksToHeal()
    {
        if (Settings.Get().debugHealingSpeed)
        {
            ticksToHeal = 3000;
            return;
        }

        ticksToHeal = Rand.Range(240000, 360000);
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksToHeal--;
        if (ticksToHeal >= 240000)
        {
            ResetTicksToHeal();
            return;
        }

        if (ticksToHeal > 0)
        {
            return;
        }

        TryHealRandomPermanentWound();
        AffectPawnsAge();
        ResetTicksToHeal();
    }

    public void TryHealRandomPermanentWound()
    {
        var selectHediffsQuery = from hd in Pawn.health.hediffSet.hediffs
            where hd.IsPermanent() || chronicConditions.Contains(hd.def.defName)
            select hd;
        if (!selectHediffsQuery.Any())
        {
            return;
        }

        selectHediffsQuery.TryRandomElement(out var hediff);
        var Hlabel = "condition";
        if (hediff != null)
        {
            Hlabel = hediff.Label;
            var meanHeal = 0.2f;
            var rndHealPercent = meanHeal + (Rand.Gaussian() * meanHeal / 2f);
            var bodyPartMaxHP = 1f;
            if (hediff.Part != null)
            {
                bodyPartMaxHP = hediff.Part.def.GetMaxHealth(hediff.pawn);
            }

            var healAmount = bodyPartMaxHP * rndHealPercent;
            if (healAmount < 0.1f)
            {
                healAmount = 0.1f;
            }

            if (hediff.Severity - healAmount < 0.1f)
            {
                Pawn.health.hediffSet.hediffs.Remove(hediff);
            }
            else
            {
                hediff.Severity -= healAmount;
            }
        }

        if (PawnUtility.ShouldSendNotificationAbout(Pawn))
        {
            Messages.Message($"{Pawn.Label}'s {Hlabel} was healed by Metasis.", Pawn,
                MessageTypeDefOf.PositiveEvent);
        }
    }

    public void AffectPawnsAge()
    {
        if (Pawn.RaceProps.Humanlike)
        {
            if (Pawn.ageTracker.AgeBiologicalYears > 25)
            {
                Pawn.ageTracker.AgeBiologicalTicks.TicksToPeriod(out var biologicalYears,
                    out var biologicalQuadrums, out var biologicalDays, out _);
                var ageBefore = "AgeBiological".Translate(biologicalYears, biologicalQuadrums, biologicalDays);
                var diffFromOptimalAge = Pawn.ageTracker.AgeBiologicalTicks - 90000000L;
                Pawn.ageTracker.AgeBiologicalTicks -= (long)(diffFromOptimalAge * 0.05f);
                Pawn.ageTracker.AgeBiologicalTicks.TicksToPeriod(out biologicalYears, out biologicalQuadrums,
                    out biologicalDays, out _);
                var ageAfter = "AgeBiological".Translate(biologicalYears, biologicalQuadrums, biologicalDays);
                if (!Pawn.IsColonist || !Settings.Get().showAgingMessages)
                {
                    return;
                }

                Messages.Message("MessageAgeReduced".Translate(Pawn.Label, ageBefore, ageAfter),
                    MessageTypeDefOf.PositiveEvent);
                Messages.Message(
                    "MessageAgeReduced".Translate(parent.LabelCap, Pawn.Label, ageBefore, ageAfter), Pawn,
                    MessageTypeDefOf.PositiveEvent);
            }
            else if (Pawn.ageTracker.AgeBiologicalYears < 25)
            {
                Pawn.ageTracker.AgeBiologicalTicks += 900000L;
            }
        }
        else
        {
            var curLifeStageIndex = Pawn.ageTracker.CurLifeStageIndex;
            var startOfThirdStage = (long)(Pawn.RaceProps.lifeStageAges[2].minAge * 60f * 60000f);
            var diffFromOptimalAge2 = Pawn.ageTracker.AgeBiologicalTicks - startOfThirdStage;
            if (curLifeStageIndex >= 3)
            {
                Pawn.ageTracker.AgeBiologicalTicks -= (long)(diffFromOptimalAge2 * 0.05f);
                return;
            }

            Pawn.ageTracker.AgeBiologicalTicks += 300000L;
        }
    }

    public override void CompExposeData()
    {
        Scribe_Values.Look(ref ticksToHeal, "ticksToHeal");
    }

    public override string CompDebugString()
    {
        return $"ticksToHeal: {ticksToHeal}";
    }
}