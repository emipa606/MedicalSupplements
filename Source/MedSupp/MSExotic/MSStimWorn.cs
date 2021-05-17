using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic
{
    // Token: 0x0200001C RID: 28
    public class MSStimWorn : Apparel
    {
        // Token: 0x04000052 RID: 82
        private int StimUses = 1;

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000076 RID: 118 RVA: 0x000066B3 File Offset: 0x000048B3
        public CompMSStimWorn StimWornComp => GetComp<CompMSStimWorn>();

        // Token: 0x06000074 RID: 116 RVA: 0x000065E7 File Offset: 0x000047E7
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            var wearer = Wearer;
            if (Wearer != null && wearer.IsColonistPlayerControlled)
            {
                if (Find.Selector.SingleSelectedThing == Wearer)
                {
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
            }
        }

        // Token: 0x06000075 RID: 117 RVA: 0x000065F8 File Offset: 0x000047F8
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

        // Token: 0x06000077 RID: 119 RVA: 0x000066BB File Offset: 0x000048BB
        public override void PostMake()
        {
            base.PostMake();
            StimUses = GetComp<CompMSStimWorn>().Props.StimUses;
        }

        // Token: 0x06000078 RID: 120 RVA: 0x000066D9 File Offset: 0x000048D9
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref StimUses, "StimUses", 1);
        }

        // Token: 0x06000079 RID: 121 RVA: 0x000066F4 File Offset: 0x000048F4
        public void MSUseStim(Pawn p, bool Using)
        {
            if (Using && p != null)
            {
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
                    Log.Message(string.Concat("ERR: Stim Worn item def not found for ", def.label.CapitalizeFirst(),
                        " : (", def.defName, ")"));
                }
            }
        }
    }
}