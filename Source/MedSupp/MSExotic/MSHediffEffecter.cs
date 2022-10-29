using Verse;

namespace MSExotic;

internal class MSHediffEffecter
{
    internal static bool HediffEffect(HediffDef hediffdef, float SeverityToApply, Pawn pawn, BodyPartRecord part,
        out bool immune)
    {
        immune = false;
        if (pawn.RaceProps.IsMechanoid || hediffdef == null)
        {
            return false;
        }

        if (!ImmuneTo(pawn, hediffdef))
        {
            if (pawn.health.WouldDieAfterAddingHediff(hediffdef, part, SeverityToApply))
            {
                return false;
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
                return true;
            }

            var addhediff = HediffMaker.MakeHediff(hediffdef, pawn, part);
            addhediff.Severity = SeverityToApply;
            pawn.health.AddHediff(addhediff, part);
            return true;
        }

        immune = true;

        return false;
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