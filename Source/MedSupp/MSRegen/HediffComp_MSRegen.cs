using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSRegen;

public class HediffComp_MSRegen : HediffComp
{
    private static readonly HediffDef burn = HediffDef.Named("Burn");
    private static readonly HediffDef scratch = HediffDef.Named("Scratch");
    private static readonly HediffDef bruise = HediffDef.Named("Bruise");
    private static readonly HediffDef gunshot = HediffDef.Named("Gunshot");
    private static readonly HediffDef stab = HediffDef.Named("Stab");
    private static readonly HediffDef shredded = HediffDef.Named("Shredded");
    private static readonly HediffDef crack = HediffDef.Named("Crack");
    private static readonly HediffDef crush = HediffDef.Named("Crush");
    private static readonly HediffDef frostbite = HediffDef.Named("Frostbite");
    private int ticksToHeal;

    private HediffCompProperties_MSRegen MSProps => (HediffCompProperties_MSRegen)props;

    public override void CompPostMake()
    {
        base.CompPostMake();
        ResetTicksToHeal();
    }

    public void ResetTicksToHeal()
    {
        var period = 2500;
        if (Def.defName == "MSBattleStim_High")
        {
            period /= 2;
        }

        if (MSProps.RegenHoursMin > 0 && MSProps.RegenHoursMax > 0 &&
            MSProps.RegenHoursMax >= MSProps.RegenHoursMin)
        {
            ticksToHeal = Rand.Range(MSProps.RegenHoursMin, MSProps.RegenHoursMax) * period;
            return;
        }

        ticksToHeal = Rand.Range(12, 24) * period;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksToHeal--;
        if (ticksToHeal > 0)
        {
            return;
        }

        TryHealRandomOldWound();
        ResetTicksToHeal();
    }

    public void TryHealRandomOldWound()
    {
        var healAmount = MSProps.RegenHealVal;
        var candidates = new List<Hediff>();
        var pawn = Pawn;
        List<Hediff> list;
        if (pawn == null)
        {
            list = null;
        }
        else
        {
            var health = pawn.health;
            list = health?.hediffSet.hediffs;
        }

        var hediffs = list;
        if (hediffs is { Count: > 0 })
        {
            foreach (var hediff in hediffs)
            {
                if (Def.defName == "MSRimBurnEazeHigh")
                {
                    if (hediff.def == burn)
                    {
                        candidates.Add(hediff);
                    }
                }
                else if (Def.defName == "MSBattleStim_High")
                {
                    if (MSIsRegenInjury(hediff))
                    {
                        candidates.Add(hediff);
                    }
                }
                else if (hediff.IsPermanent())
                {
                    candidates.Add(hediff);
                }
            }
        }

        if (candidates.Count <= 0)
        {
            return;
        }

        candidates.TryRandomElement(out var hediffToHeal);
        if (hediffToHeal == null)
        {
            return;
        }

        if (hediffToHeal.IsTended())
        {
            healAmount = (int)(healAmount * 1.2f);
            var healfactor = GetHealFactor(hediffToHeal);
            if (healfactor > 0f)
            {
                healAmount = (int)(healAmount * healfactor);
                if (healAmount < 1)
                {
                    healAmount = 1;
                }
            }
        }

        if (hediffToHeal.Severity - healAmount <= 0f && PawnUtility.ShouldSendNotificationAbout(Pawn) &&
            !MSIsFastRegen(Def.defName))
        {
            Messages.Message(
                "MSRegen.WoundHealed".Translate(parent.LabelCap, Pawn.LabelShort, hediffToHeal.Label,
                    Pawn.Named("PAWN")), Pawn, MessageTypeDefOf.PositiveEvent);
        }

        if (hediffToHeal.Severity - healAmount > 0f)
        {
            hediffToHeal.Severity -= healAmount;
            return;
        }

        hediffToHeal.Severity = 0f;
    }

    private static float GetHealFactor(Hediff h)
    {
        var hf = 1f;
        if (h.def == scratch)
        {
            hf = 1.2f;
        }
        else if (h.def == bruise)
        {
            hf = 1.5f;
        }
        else if (h.def == burn)
        {
            hf = 1.2f;
        }
        else if (h.def == crack || h.def == crush || h.def == frostbite)
        {
            hf = 0.8f;
        }

        return hf;
    }

    private static bool MSIsRegenInjury(Hediff h)
    {
        return h.Bleeding || h.def == HediffDefOf.Cut || h.def == burn ||
               h.def == gunshot || h.def == scratch || h.def == stab ||
               h.def == bruise || h.def == HediffDefOf.Bite || h.def == shredded ||
               h.IsPermanent() || h.def == crack || h.def == crush || h.def == frostbite;
    }

    private static bool MSIsFastRegen(string defName)
    {
        return defName == "MSBattleStim_High";
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