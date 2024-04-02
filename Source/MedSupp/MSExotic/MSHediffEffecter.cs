using Verse;

namespace MSExotic;

internal class MSHediffEffecter
{
    internal static void HediffEffect(HediffDef hediffdef, float SeverityToApply, Pawn pawn, BodyPartRecord part)
    {
        if (pawn.RaceProps.IsMechanoid || hediffdef == null)
        {
            return;
        }

        if (ImmuneTo(pawn, hediffdef))
        {
            return;
        }

        if (pawn.health.WouldDieAfterAddingHediff(hediffdef, part, SeverityToApply))
        {
            return;
        }

        var health = pawn.health;
        Hediff hediff;
        if (health == null)
        {
            hediff = null;
        }
        else
        {
            var hediffSet = health.hediffSet;
            hediff = hediffSet?.GetFirstHediffOfDef(hediffdef);
        }

        var hashediff = hediff;
        if (hashediff != null)
        {
            hashediff.Severity += SeverityToApply;
            return;
        }

        var addhediff = HediffMaker.MakeHediff(hediffdef, pawn, part);
        addhediff.Severity = SeverityToApply;
        pawn.health.AddHediff(addhediff, part);
    }

    internal static bool ImmuneTo(Pawn pawn, HediffDef def)
    {
        var hediffs = pawn.health.hediffSet.hediffs;
        foreach (var hediff in hediffs)
        {
            var curStage = hediff.CurStage;
            if (curStage?.makeImmuneTo == null)
            {
                continue;
            }

            foreach (var hediffDef in curStage.makeImmuneTo)
            {
                if (hediffDef == def)
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static bool HasHediff(Pawn pawn, HediffDef def)
    {
        var health = pawn.health;
        var HS = health?.hediffSet;
        return HS?.GetFirstHediffOfDef(def) != null;
    }
}