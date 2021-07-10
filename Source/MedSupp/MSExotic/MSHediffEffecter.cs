using Verse;

namespace MSExotic
{
    // Token: 0x02000019 RID: 25
    internal class MSHediffEffecter
    {
        // Token: 0x06000064 RID: 100 RVA: 0x00005F24 File Offset: 0x00004124
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

        // Token: 0x06000065 RID: 101 RVA: 0x00005FC0 File Offset: 0x000041C0
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

        // Token: 0x06000066 RID: 102 RVA: 0x00006030 File Offset: 0x00004230
        internal static bool HasHediff(Pawn pawn, HediffDef def)
        {
            var health = pawn.health;
            var HS = health?.hediffSet;
            return HS?.GetFirstHediffOfDef(def) != null;
        }
    }
}