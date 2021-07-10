using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic
{
    // Token: 0x02000018 RID: 24
    public class MSExoticUtility
    {
        // Token: 0x06000056 RID: 86 RVA: 0x0000571C File Offset: 0x0000391C
        public static void DoMSImmunisation(Pawn p, ThingDef t)
        {
            var Sickly = false;
            var MsgType = MessageTypeDefOf.PositiveEvent;
            var Immunity = DefDatabase<TraitDef>.GetNamed("Immunity");
            if (p.story.traits.HasTrait(Immunity))
            {
                var story = p.story;
                int? num;
                if (story == null)
                {
                    num = null;
                }
                else
                {
                    var traits = story.traits;
                    num = traits != null ? new int?(traits.GetTrait(Immunity).Degree) : null;
                }

                var num2 = num;
                var num3 = -1;
                if ((num2.GetValueOrDefault() == num3) & (num2 != null))
                {
                    Sickly = true;
                }
            }

            if (Sickly)
            {
                var story2 = p.story;
                Trait trait;
                if (story2 == null)
                {
                    trait = null;
                }
                else
                {
                    var traits2 = story2.traits;
                    trait = traits2?.GetTrait(Immunity);
                }

                var ToGo = trait;
                MSTraitChanger.RemoveTrait(p, ToGo, t.label, MsgType);
                return;
            }

            var ToAdd = new Trait(Immunity, 1);
            MSTraitChanger.AddTrait(p, ToAdd, t.label, MsgType);
        }

        // Token: 0x06000057 RID: 87 RVA: 0x000057F8 File Offset: 0x000039F8
        public static void ChkMSImmunisation(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            var Immunity = DefDatabase<TraitDef>.GetNamed("Immunity");
            if (p.story.traits.HasTrait(Immunity))
            {
                var story = p.story;
                int? num;
                if (story == null)
                {
                    num = null;
                }
                else
                {
                    var traits = story.traits;
                    num = traits != null ? new int?(traits.GetTrait(Immunity).Degree) : null;
                }

                var num2 = num;
                var num3 = 1;
                if ((num2.GetValueOrDefault() == num3) & (num2 != null))
                {
                    Passed = false;
                    Reason = "MSExotic.AlreadyHasImmunity".Translate(p.LabelShort.CapitalizeFirst());
                    return;
                }
            }

            Passed = true;
        }

        // Token: 0x06000058 RID: 88 RVA: 0x000058E0 File Offset: 0x00003AE0
        public static void DoMSCerebrax(Pawn p, ThingDef t)
        {
            var degree = 0;
            var MsgType = MessageTypeDefOf.PositiveEvent;
            var Psych = DefDatabase<TraitDef>.GetNamed("PsychicSensitivity");
            if (p.story.traits.HasTrait(Psych))
            {
                degree = p.story.traits.GetTrait(Psych).Degree;
            }

            if (degree == 2)
            {
                var story = p.story;
                Trait trait;
                if (story == null)
                {
                    trait = null;
                }
                else
                {
                    var traits = story.traits;
                    trait = traits?.GetTrait(Psych);
                }

                var ToGo = trait;
                MSTraitChanger.RemoveTrait(p, ToGo, t.label, MsgType);
                var ToAdd = new Trait(Psych, 1);
                MSTraitChanger.AddTrait(p, ToAdd, t.label, MsgType, false);
                return;
            }

            if (degree != 1)
            {
                return;
            }

            var story2 = p.story;
            Trait trait2;
            if (story2 == null)
            {
                trait2 = null;
            }
            else
            {
                var traits2 = story2.traits;
                trait2 = traits2?.GetTrait(Psych);
            }

            var ToGo2 = trait2;
            MSTraitChanger.RemoveTrait(p, ToGo2, t.label, MsgType);
        }

        // Token: 0x06000059 RID: 89 RVA: 0x000059B0 File Offset: 0x00003BB0
        public static void ChkMSCerebrax(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            var Psych = DefDatabase<TraitDef>.GetNamed("PsychicSensitivity");
            if (p.story.traits.HasTrait(Psych))
            {
                var story = p.story;
                int? num;
                if (story == null)
                {
                    num = null;
                }
                else
                {
                    var traits = story.traits;
                    num = traits != null ? new int?(traits.GetTrait(Psych).Degree) : null;
                }

                var num2 = num;
                var num3 = 0;
                if ((num2.GetValueOrDefault() <= num3) & (num2 != null))
                {
                    Passed = false;
                    Reason = "MSExotic.HasNoPsychSensitivity".Translate(p.LabelShort.CapitalizeFirst());
                    return;
                }

                Passed = true;
                return;
            }

            Passed = false;
            Reason = "MSExotic.HasNoPsychSensitivity".Translate(p.LabelShort.CapitalizeFirst());
        }

        // Token: 0x0600005A RID: 90 RVA: 0x00005ACC File Offset: 0x00003CCC
        public static void DoMSBattleStim(Pawn p, ThingDef t)
        {
            var named = DefDatabase<HediffDef>.GetNamed("MSBattleStim_High", false);
            var SeverityToApply = 0.5f;
            MSHediffEffecter.HediffEffect(named, SeverityToApply, p, null, out _);
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00005AF8 File Offset: 0x00003CF8
        public static void ChkMSBattleStim(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            if (MSHediffEffecter.HasHediff(p, DefDatabase<HediffDef>.GetNamed("MSBattleStim_High")))
            {
                Passed = false;
                Reason = "MSExoctic.AlreadyStimmed".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            Passed = true;
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00005B81 File Offset: 0x00003D81
        public static bool GetIsTranscendence(ThingDef def)
        {
            return def.defName.StartsWith("MSTranscendence");
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00005B98 File Offset: 0x00003D98
        public static void DoMSTranscendence(Pawn p, ThingDef t)
        {
            var skills = MSSkillChanger.GetSkillList(MSSkillChanger.GetTranscendenceQuality(t));
            var candidates = new List<SkillDef>();
            if (skills.Count > 0)
            {
                foreach (var skillDef in skills)
                {
                    if (p == null)
                    {
                        continue;
                    }

                    var skills2 = p.skills;
                    int? num;
                    if (skills2 == null)
                    {
                        num = null;
                    }
                    else
                    {
                        var skill2 = skills2.GetSkill(skillDef);
                        num = skill2 != null ? new int?(skill2.Level) : null;
                    }

                    var num2 = num;
                    var num3 = 20;
                    if (!((num2.GetValueOrDefault() < num3) & (num2 != null)))
                    {
                        continue;
                    }

                    var skills3 = p.skills;
                    num2 = skills3 != null ? new int?(skills3.GetSkill(skillDef).Level) : null;
                    num3 = 0;
                    if ((num2.GetValueOrDefault() > num3) & (num2 != null))
                    {
                        candidates.Add(skillDef);
                    }
                }
            }

            if (candidates.Count <= 0)
            {
                return;
            }

            var skill = candidates.RandomElement();
            if (p == null)
            {
                return;
            }

            var before = p.skills.GetSkill(skill).Level;
            var Rnd = Rand.Range(1f, 3f);
            p.skills.Learn(skill, 32000f * Rnd, true);
            var after = p.skills.GetSkill(skill).Level;
            Messages.Message(
                "MSExotic.TSkillBoost".Translate(p.LabelShort, skill.label.CapitalizeFirst(), before.ToString(),
                    after.ToString()), p, MessageTypeDefOf.PositiveEvent);
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00005D44 File Offset: 0x00003F44
        public static void ChkMSTranscendence(Pawn p, ThingDef t, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            MSSkillChanger.CheckSkills(p, t, out var skillsReason, out var skillsPassed);
            if (!skillsPassed)
            {
                Passed = false;
                Reason = skillsReason;
                return;
            }

            Passed = true;
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00005DA4 File Offset: 0x00003FA4
        public static void DoMSPerpetuity(Pawn p, ThingDef t)
        {
            var grav = 27f;
            var TicksPerYear = 3600000;
            var age = p.ageTracker.AgeBiologicalYearsFloat;
            float adj;
            float offset;
            if (age >= grav)
            {
                adj = -1f;
                offset = (age - grav) / 10f;
            }
            else
            {
                adj = 1f;
                offset = (grav - age) / 10f;
            }

            float Rnd = Rand.Range(1, 3);
            var newage = (long) ((age + ((Rnd + offset) * adj)) * TicksPerYear);
            p.ageTracker.AgeBiologicalTicks = newage;
            var dispnewage = p.ageTracker.AgeBiologicalYearsFloat;
            Messages.Message(
                "MSExotic.PerpetuityDone".Translate(p.LabelShort, ((int) age).ToString(),
                    ((int) dispnewage).ToString()), p, MessageTypeDefOf.PositiveEvent);
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00005E70 File Offset: 0x00004070
        public static void ChkMSPerpetuity(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            Passed = true;
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00005EB0 File Offset: 0x000040B0
        public static void DoMSCondom(Pawn p, ThingDef t)
        {
            var named = DefDatabase<HediffDef>.GetNamed("MSCondom_High", false);
            var SeverityToApply = 0.5f;
            MSHediffEffecter.HediffEffect(named, SeverityToApply, p, null, out _);
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00005ED9 File Offset: 0x000040D9
        public static void ChkMSCondom(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "MSExotic.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            Passed = true;
        }
    }
}