using RimWorld;
using Verse;

namespace MSExotic;

public class CompUseEffect_MSExotic : CompUseEffect
{
    public override void DoEffect(Pawn usedBy)
    {
        base.DoEffect(usedBy);
        if (parent.def == MSExoticDefOf.ThingDefOf.MSImmunisation)
        {
            MSExoticUtility.DoMSImmunisation(usedBy, parent.def);
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSCerebrax)
        {
            MSExoticUtility.DoMSCerebrax(usedBy, parent.def);
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSBattleStim)
        {
            MSExoticUtility.DoMSBattleStim(usedBy, parent.def);
        }

        if (MSExoticUtility.GetIsTranscendence(parent.def))
        {
            MSExoticUtility.DoMSTranscendence(usedBy, parent.def);
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSPerpetuity)
        {
            MSExoticUtility.DoMSPerpetuity(usedBy, parent.def);
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSCondom)
        {
            MSExoticUtility.DoMSCondom(usedBy, parent.def);
        }
    }

    public override bool CanBeUsedBy(Pawn p, out string failReason)
    {
        failReason = null;
        if (parent.def == MSExoticDefOf.ThingDefOf.MSImmunisation)
        {
            MSExoticUtility.ChkMSImmunisation(p, out var Reason, out var Passed);
            if (!Passed)
            {
                failReason = Reason;
                return false;
            }
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSCerebrax)
        {
            MSExoticUtility.ChkMSCerebrax(p, out var Reason2, out var Passed2);
            if (!Passed2)
            {
                failReason = Reason2;
                return false;
            }
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSBattleStim)
        {
            MSExoticUtility.ChkMSBattleStim(p, out var Reason3, out var Passed3);
            if (!Passed3)
            {
                failReason = Reason3;
                return false;
            }
        }

        if (MSExoticUtility.GetIsTranscendence(parent.def))
        {
            MSExoticUtility.ChkMSTranscendence(p, parent.def, out var Reason4, out var Passed4);
            if (!Passed4)
            {
                failReason = Reason4;
                return false;
            }
        }

        if (parent.def == MSExoticDefOf.ThingDefOf.MSPerpetuity)
        {
            MSExoticUtility.ChkMSPerpetuity(p, out var Reason5, out var Passed5);
            if (!Passed5)
            {
                failReason = Reason5;
                return false;
            }
        }

        if (parent.def != MSExoticDefOf.ThingDefOf.MSCondom)
        {
            return true;
        }

        MSExoticUtility.ChkMSCondom(p, out var Reason6, out var Passed6);
        if (Passed6)
        {
            return true;
        }

        failReason = Reason6;
        return false;
    }
}