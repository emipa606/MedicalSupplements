using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSRegen;

public class HediffComp_MSRegen : HediffComp
{
    private int ticksToHeal;

    public HediffCompProperties_MSRegen MSProps => (HediffCompProperties_MSRegen)props;

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
                    if (hediff.def == HediffDefOf.Burn)
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

    internal float GetHealFactor(Hediff h)
    {
        var hf = 1f;
        if (h.def == HediffDefOf.Scratch)
        {
            hf = 1.2f;
        }
        else if (h.def == HediffDefOf.Bruise)
        {
            hf = 1.5f;
        }
        else if (h.def == HediffDefOf.Burn)
        {
            hf = 1.2f;
        }
        else if (h.def == DefDatabase<HediffDef>.GetNamed("Crack"))
        {
            hf = 0.8f;
        }
        else if (h.def == DefDatabase<HediffDef>.GetNamed("Crush"))
        {
            hf = 0.8f;
        }
        else if (h.def == DefDatabase<HediffDef>.GetNamed("Frostbite"))
        {
            hf = 0.8f;
        }

        return hf;
    }

    internal bool MSIsRegenInjury(Hediff h)
    {
        return h.Bleeding || h.def == HediffDefOf.Cut || h.def == HediffDefOf.Burn ||
               h.def == HediffDefOf.Gunshot || h.def == HediffDefOf.Scratch || h.def == HediffDefOf.Stab ||
               h.def == HediffDefOf.Bruise || h.def == HediffDefOf.Bite || h.def == HediffDefOf.Shredded ||
               h.IsPermanent() || h.def == DefDatabase<HediffDef>.GetNamed("Crack") ||
               h.def == DefDatabase<HediffDef>.GetNamed("Crush") ||
               h.def == DefDatabase<HediffDef>.GetNamed("Frostbite");
    }

    internal bool MSIsFastRegen(string defName)
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