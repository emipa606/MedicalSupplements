using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSExotic;

public static class MSSkillChanger
{
    public static string GetTranscendenceQuality(ThingDef t)
    {
        var length = "MSTranscendence_".Length;
        return t.defName.Substring(length, t.defName.Length - length);
    }

    public static void CheckSkills(Pawn p, ThingDef t, out string reason, out bool passed)
    {
        reason = "";
        passed = true;
        var chkskills = GetSkillList(GetTranscendenceQuality(t));
        if (chkskills.Count > 0)
        {
            var numCants = 0;
            foreach (var skillDef in chkskills)
            {
                if (p == null)
                {
                    continue;
                }

                var skills = p.skills;
                int? num;
                if (skills == null)
                {
                    num = null;
                }
                else
                {
                    var skill = skills.GetSkill(skillDef);
                    num = skill != null ? new int?(skill.Level) : null;
                }

                var num2 = num;
                var num3 = 20;
                if ((num2.GetValueOrDefault() >= num3) & (num2 != null))
                {
                    numCants++;
                }
            }

            if (numCants != chkskills.Count)
            {
                return;
            }

            if (p != null)
            {
                reason = "MSExotic.WontLearnUsing".Translate(p.LabelShort, t.label.CapitalizeFirst());
            }

            passed = false;
        }
        else
        {
            reason = "MSExotic.NoTSkillList".Translate(p.LabelShort, t.defName);
            passed = false;
        }
    }

    public static List<SkillDef> GetSkillList(string selector)
    {
        var skills = new List<SkillDef>();
        switch (selector)
        {
            case "All":
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
                break;
            case "Receptive":
                skills.Add(SkillDefOf.Animals);
                skills.Add(SkillDefOf.Social);
                break;
            case "Inventive":
                skills.Add(SkillDefOf.Artistic);
                skills.Add(SkillDefOf.Cooking);
                skills.Add(SkillDefOf.Plants);
                break;
            case "Tactical":
                skills.Add(SkillDefOf.Shooting);
                skills.Add(SkillDefOf.Melee);
                break;
            case "Practical":
                skills.Add(SkillDefOf.Construction);
                skills.Add(SkillDefOf.Crafting);
                skills.Add(SkillDefOf.Mining);
                break;
            default:
            {
                if (selector != "Analytical")
                {
                    return skills;
                }

                skills.Add(SkillDefOf.Intellectual);
                skills.Add(SkillDefOf.Medicine);
                break;
            }
        }

        return skills;
    }
}