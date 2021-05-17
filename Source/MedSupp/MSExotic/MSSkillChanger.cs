using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic
{
    // Token: 0x0200001B RID: 27
    public static class MSSkillChanger
    {
        // Token: 0x06000071 RID: 113 RVA: 0x00006320 File Offset: 0x00004520
        public static string GetTranscendenceQuality(ThingDef t)
        {
            var length = "MSTranscendence_".Length;
            return t.defName.Substring(length, t.defName.Length - length);
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00006354 File Offset: 0x00004554
        public static void CheckSkills(Pawn p, ThingDef t, out string reason, out bool passed)
        {
            reason = "";
            passed = true;
            var chkskills = GetSkillList(GetTranscendenceQuality(t));
            if (chkskills.Count > 0)
            {
                var numCants = 0;
                for (var i = 0; i < chkskills.Count; i++)
                {
                    if (p != null)
                    {
                        var skills = p.skills;
                        int? num;
                        if (skills == null)
                        {
                            num = null;
                        }
                        else
                        {
                            var skill = skills.GetSkill(chkskills[i]);
                            num = skill != null ? new int?(skill.Level) : null;
                        }

                        var num2 = num;
                        var num3 = 20;
                        if ((num2.GetValueOrDefault() >= num3) & (num2 != null))
                        {
                            numCants++;
                        }
                    }
                }

                if (numCants == chkskills.Count)
                {
                    reason = "MSExotic.WontLearnUsing".Translate(p.LabelShort, t.label.CapitalizeFirst());
                    passed = false;
                }
            }
            else
            {
                reason = "MSExotic.NoTSkillList".Translate(p.LabelShort, t.defName);
                passed = false;
            }
        }

        // Token: 0x06000073 RID: 115 RVA: 0x0000645C File Offset: 0x0000465C
        public static List<SkillDef> GetSkillList(string selector)
        {
            var skills = new List<SkillDef>();
            if (!(selector == "All"))
            {
                if (!(selector == "Receptive"))
                {
                    if (!(selector == "Inventive"))
                    {
                        if (!(selector == "Tactical"))
                        {
                            if (!(selector == "Practical"))
                            {
                                if (selector == "Analytical")
                                {
                                    skills.Add(SkillDefOf.Intellectual);
                                    skills.Add(SkillDefOf.Medicine);
                                }
                            }
                            else
                            {
                                skills.Add(SkillDefOf.Construction);
                                skills.Add(SkillDefOf.Crafting);
                                skills.Add(SkillDefOf.Mining);
                            }
                        }
                        else
                        {
                            skills.Add(SkillDefOf.Shooting);
                            skills.Add(SkillDefOf.Melee);
                        }
                    }
                    else
                    {
                        skills.Add(SkillDefOf.Artistic);
                        skills.Add(SkillDefOf.Cooking);
                        skills.Add(SkillDefOf.Plants);
                    }
                }
                else
                {
                    skills.Add(SkillDefOf.Animals);
                    skills.Add(SkillDefOf.Social);
                }
            }
            else
            {
                skills.Add(SkillDefOf.Animals);
                skills.Add(SkillDefOf.Artistic);
                skills.Add(SkillDefOf.Construction);
                skills.Add(SkillDefOf.Cooking);
                skills.Add(SkillDefOf.Crafting);
                skills.Add(SkillDefOf.Intellectual);
                skills.Add(SkillDefOf.Medicine);
                skills.Add(SkillDefOf.Melee);
                skills.Add(SkillDefOf.Mining);
                skills.Add(SkillDefOf.Plants);
                skills.Add(SkillDefOf.Shooting);
                skills.Add(SkillDefOf.Social);
            }

            return skills;
        }
    }
}