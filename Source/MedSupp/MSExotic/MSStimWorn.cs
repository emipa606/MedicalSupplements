using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic;

public class MSStimWorn : Apparel
{
    private int StimUses = 1;

    public CompMSStimWorn StimWornComp => GetComp<CompMSStimWorn>();

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        var wearer = Wearer;
        if (Wearer == null || !wearer.IsColonistPlayerControlled)
        {
            yield break;
        }

        if (Find.Selector.SingleSelectedThing != Wearer)
        {
            yield break;
        }

        string text = "MSExotic.StimUses".Translate(def.label.CapitalizeFirst(), StimUses.ToString());
        string desc = "MSExotic.StimDesc".Translate(def.label.CapitalizeFirst());
        yield return new Command_Action
        {
            defaultLabel = text,
            defaultDesc = desc,
            icon = def.uiIcon,
            action = delegate { MSDoStimSelect(Wearer); }
        };
    }

    public void MSDoStimSelect(Pawn p)
    {
        var list = new List<FloatMenuOption>();
        string text = "MSExotic.StimSelDoNothing".Translate();
        list.Add(new FloatMenuOption(text, delegate { MSUseStim(p, false); }, MenuOptionPriority.Default, null,
            null, 29f));
        text = "MSExotic.StimSelUseStim".Translate(def.label.CapitalizeFirst());
        list.Add(new FloatMenuOption(text, delegate { MSUseStim(p, true); }, MenuOptionPriority.Default, null, null,
            29f, rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), def)));
        Find.WindowStack.Add(new FloatMenu(list));
    }

    public override void PostMake()
    {
        base.PostMake();
        StimUses = GetComp<CompMSStimWorn>().Props.StimUses;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref StimUses, "StimUses", 1);
    }

    public void MSUseStim(Pawn p, bool Using)
    {
        if (!Using || p == null)
        {
            return;
        }

        if (def.defName == "MSBattleStimBelt")
        {
            MSExoticUtility.ChkMSBattleStim(p, out var Reason, out var Passed);
            if (!Passed)
            {
                Messages.Message("MSExotic.ReasonPrefix".Translate() + Reason, p, MessageTypeDefOf.NeutralEvent,
                    false);
                return;
            }

            StimUses--;
            MSExoticUtility.DoMSBattleStim(p, def);
            if (StimUses < 1 && !this.DestroyedOrNull())
            {
                Destroy();
            }
        }
        else
        {
            Log.Message($"ERR: Stim Worn item def not found for {def.label.CapitalizeFirst()} : ({def.defName})");
        }
    }
}