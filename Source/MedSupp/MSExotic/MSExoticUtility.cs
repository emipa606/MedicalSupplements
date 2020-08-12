using System;
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
			bool Sickly = false;
			MessageTypeDef MsgType = MessageTypeDefOf.PositiveEvent;
			TraitDef Immunity = DefDatabase<TraitDef>.GetNamed("Immunity", true);
			if (p.story.traits.HasTrait(Immunity) && p != null)
			{
				Pawn_StoryTracker story = p.story;
				int? num;
				if (story == null)
				{
					num = null;
				}
				else
				{
					TraitSet traits = story.traits;
					num = ((traits != null) ? new int?(traits.GetTrait(Immunity).Degree) : null);
				}
				int? num2 = num;
				int num3 = -1;
				if (num2.GetValueOrDefault() == num3 & num2 != null)
				{
					Sickly = true;
				}
			}
			if (Sickly)
			{
				Pawn_StoryTracker story2 = p.story;
				Trait trait;
				if (story2 == null)
				{
					trait = null;
				}
				else
				{
					TraitSet traits2 = story2.traits;
					trait = ((traits2 != null) ? traits2.GetTrait(Immunity) : null);
				}
				Trait ToGo = trait;
				MSTraitChanger.RemoveTrait(p, ToGo, t.label, MsgType, true);
				return;
			}
			Trait ToAdd = new Trait(Immunity, 1, false);
			MSTraitChanger.AddTrait(p, ToAdd, t.label, MsgType, true);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000057F8 File Offset: 0x000039F8
		public static void ChkMSImmunisation(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			TraitDef Immunity = DefDatabase<TraitDef>.GetNamed("Immunity", true);
			if (p.story.traits.HasTrait(Immunity) && p != null)
			{
				Pawn_StoryTracker story = p.story;
				int? num;
				if (story == null)
				{
					num = null;
				}
				else
				{
					TraitSet traits = story.traits;
					num = ((traits != null) ? new int?(traits.GetTrait(Immunity).Degree) : null);
				}
				int? num2 = num;
				int num3 = 1;
				if (num2.GetValueOrDefault() == num3 & num2 != null)
				{
					Passed = false;
					Reason = "MSExotic.AlreadyHasImmunity".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
					return;
				}
			}
			Passed = true;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000058E0 File Offset: 0x00003AE0
		public static void DoMSCerebrax(Pawn p, ThingDef t)
		{
			int degree = 0;
			MessageTypeDef MsgType = MessageTypeDefOf.PositiveEvent;
			TraitDef Psych = DefDatabase<TraitDef>.GetNamed("PsychicSensitivity", true);
			if (p.story.traits.HasTrait(Psych))
			{
				degree = p.story.traits.GetTrait(Psych).Degree;
			}
			if (degree == 2)
			{
				Pawn_StoryTracker story = p.story;
				Trait trait;
				if (story == null)
				{
					trait = null;
				}
				else
				{
					TraitSet traits = story.traits;
					trait = ((traits != null) ? traits.GetTrait(Psych) : null);
				}
				Trait ToGo = trait;
				MSTraitChanger.RemoveTrait(p, ToGo, t.label, MsgType, true);
				Trait ToAdd = new Trait(Psych, 1, false);
				MSTraitChanger.AddTrait(p, ToAdd, t.label, MsgType, false);
				return;
			}
			if (degree == 1)
			{
				Pawn_StoryTracker story2 = p.story;
				Trait trait2;
				if (story2 == null)
				{
					trait2 = null;
				}
				else
				{
					TraitSet traits2 = story2.traits;
					trait2 = ((traits2 != null) ? traits2.GetTrait(Psych) : null);
				}
				Trait ToGo2 = trait2;
				MSTraitChanger.RemoveTrait(p, ToGo2, t.label, MsgType, true);
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000059B0 File Offset: 0x00003BB0
		public static void ChkMSCerebrax(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			TraitDef Psych = DefDatabase<TraitDef>.GetNamed("PsychicSensitivity", true);
			if (p.story.traits.HasTrait(Psych))
			{
				if (p != null)
				{
					Pawn_StoryTracker story = p.story;
					int? num;
					if (story == null)
					{
						num = null;
					}
					else
					{
						TraitSet traits = story.traits;
						num = ((traits != null) ? new int?(traits.GetTrait(Psych).Degree) : null);
					}
					int? num2 = num;
					int num3 = 0;
					if (num2.GetValueOrDefault() <= num3 & num2 != null)
					{
						Passed = false;
						Reason = "MSExotic.HasNoPsychSensitivity".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
						return;
					}
				}
				Passed = true;
				return;
			}
			Passed = false;
			Reason = "MSExotic.HasNoPsychSensitivity".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00005ACC File Offset: 0x00003CCC
		public static void DoMSBattleStim(Pawn p, ThingDef t)
		{
			HediffDef named = DefDatabase<HediffDef>.GetNamed("MSBattleStim_High", false);
			float SeverityToApply = 0.5f;
			bool immune;
			MSHediffEffecter.HediffEffect(named, SeverityToApply, p, null, out immune);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005AF8 File Offset: 0x00003CF8
		public static void ChkMSBattleStim(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			if (MSHediffEffecter.HasHediff(p, DefDatabase<HediffDef>.GetNamed("MSBattleStim_High", true)))
			{
				Passed = false;
				Reason = "MSExoctic.AlreadyStimmed".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
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
			List<SkillDef> skills = MSSkillChanger.GetSkillList(MSSkillChanger.GetTranscendenceQuality(t));
			List<SkillDef> candidates = new List<SkillDef>();
			if (skills.Count > 0)
			{
				for (int i = 0; i < skills.Count; i++)
				{
					if (p != null)
					{
						Pawn_SkillTracker skills2 = p.skills;
						int? num;
						if (skills2 == null)
						{
							num = null;
						}
						else
						{
							SkillRecord skill2 = skills2.GetSkill(skills[i]);
							num = ((skill2 != null) ? new int?(skill2.Level) : null);
						}
						int? num2 = num;
						int num3 = 20;
						if ((num2.GetValueOrDefault() < num3 & num2 != null) && p != null)
						{
							Pawn_SkillTracker skills3 = p.skills;
							num2 = ((skills3 != null) ? new int?(skills3.GetSkill(skills[i]).Level) : null);
							num3 = 0;
							if (num2.GetValueOrDefault() > num3 & num2 != null)
							{
								candidates.Add(skills[i]);
							}
						}
					}
				}
			}
			if (candidates.Count > 0)
			{
				SkillDef skill = candidates.RandomElement<SkillDef>();
				int before = p.skills.GetSkill(skill).Level;
				float Rnd = Rand.Range(1f, 3f);
				p.skills.Learn(skill, 32000f * Rnd, true);
				int after = p.skills.GetSkill(skill).Level;
				Messages.Message("MSExotic.TSkillBoost".Translate(p.LabelShort, skill.label.CapitalizeFirst(), before.ToString(), after.ToString()), p, MessageTypeDefOf.PositiveEvent, true);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005D44 File Offset: 0x00003F44
		public static void ChkMSTranscendence(Pawn p, ThingDef t, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			string skillsReason;
			bool skillsPassed;
			MSSkillChanger.CheckSkills(p, t, out skillsReason, out skillsPassed);
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
			float grav = 27f;
			int TicksPerYear = 3600000;
			float age = p.ageTracker.AgeBiologicalYearsFloat;
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
			float Rnd = (float)Rand.Range(1, 3);
			long newage = (long)((age + (Rnd + offset) * adj) * (float)TicksPerYear);
			p.ageTracker.AgeBiologicalTicks = newage;
			float dispnewage = p.ageTracker.AgeBiologicalYearsFloat;
			Messages.Message("MSExotic.PerpetuityDone".Translate(p.LabelShort, ((int)age).ToString(), ((int)dispnewage).ToString()), p, MessageTypeDefOf.PositiveEvent, true);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005E70 File Offset: 0x00004070
		public static void ChkMSPerpetuity(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			Passed = true;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00005EB0 File Offset: 0x000040B0
		public static void DoMSCondom(Pawn p, ThingDef t)
		{
			HediffDef named = DefDatabase<HediffDef>.GetNamed("MSCondom_High", false);
			float SeverityToApply = 0.5f;
			bool immune;
			MSHediffEffecter.HediffEffect(named, SeverityToApply, p, null, out immune);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005ED9 File Offset: 0x000040D9
		public static void ChkMSCondom(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "MSExotic.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			Passed = true;
		}
	}
}
