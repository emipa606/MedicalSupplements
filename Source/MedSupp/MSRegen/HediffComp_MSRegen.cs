using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSRegen
{
    // Token: 0x0200000D RID: 13
    public class HediffComp_MSRegen : HediffComp
    {
        // Token: 0x04000022 RID: 34
        private int ticksToHeal;

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000032 RID: 50 RVA: 0x000040AB File Offset: 0x000022AB
        public HediffCompProperties_MSRegen MSProps => (HediffCompProperties_MSRegen) props;

        // Token: 0x06000033 RID: 51 RVA: 0x000040B8 File Offset: 0x000022B8
        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetTicksToHeal();
        }

        // Token: 0x06000034 RID: 52 RVA: 0x000040C8 File Offset: 0x000022C8
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

        // Token: 0x06000035 RID: 53 RVA: 0x0000415F File Offset: 0x0000235F
        public override void CompPostTick(ref float severityAdjustment)
        {
            ticksToHeal--;
            if (ticksToHeal <= 0)
            {
                TryHealRandomOldWound();
                ResetTicksToHeal();
            }
        }

        // Token: 0x06000036 RID: 54 RVA: 0x0000418C File Offset: 0x0000238C
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
            if (hediffs.Count > 0)
            {
                for (var i = 0; i < hediffs.Count; i++)
                {
                    var hediff = hediffs[i];
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

            if (candidates.Count > 0)
            {
                candidates.TryRandomElement(out var hediffToHeal);
                if (hediffToHeal != null)
                {
                    if (hediffToHeal.IsTended())
                    {
                        healAmount = (int) (healAmount * 1.2f);
                        var healfactor = GetHealFactor(hediffToHeal);
                        if (healfactor > 0f)
                        {
                            healAmount = (int) (healAmount * healfactor);
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
            }
        }

        // Token: 0x06000037 RID: 55 RVA: 0x0000437C File Offset: 0x0000257C
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

        // Token: 0x06000038 RID: 56 RVA: 0x00004420 File Offset: 0x00002620
        internal bool MSIsRegenInjury(Hediff h)
        {
            return h.Bleeding || h.def == HediffDefOf.Cut || h.def == HediffDefOf.Burn ||
                   h.def == HediffDefOf.Gunshot || h.def == HediffDefOf.Scratch || h.def == HediffDefOf.Stab ||
                   h.def == HediffDefOf.Bruise || h.def == HediffDefOf.Bite || h.def == HediffDefOf.Shredded ||
                   h.IsPermanent() || h.def == DefDatabase<HediffDef>.GetNamed("Crack") ||
                   h.def == DefDatabase<HediffDef>.GetNamed("Crush") ||
                   h.def == DefDatabase<HediffDef>.GetNamed("Frostbite");
        }

        // Token: 0x06000039 RID: 57 RVA: 0x000044ED File Offset: 0x000026ED
        internal bool MSIsFastRegen(string defName)
        {
            return defName == "MSBattleStim_High";
        }

        // Token: 0x0600003A RID: 58 RVA: 0x000044FF File Offset: 0x000026FF
        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksToHeal, "ticksToHeal");
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00004513 File Offset: 0x00002713
        public override string CompDebugString()
        {
            return "ticksToHeal: " + ticksToHeal;
        }
    }
}