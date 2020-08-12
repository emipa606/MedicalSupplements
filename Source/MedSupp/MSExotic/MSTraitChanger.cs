using System;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace MSExotic
{
	// Token: 0x0200001D RID: 29
	public static class MSTraitChanger
	{
		// Token: 0x0600007C RID: 124 RVA: 0x000067F8 File Offset: 0x000049F8
		public static void RemoveTrait(Pawn pawn, Trait trait, string doer, MessageTypeDef MsgType, bool SendMsg = true)
		{
			Pawn_StoryTracker story = pawn.story;
			Trait remTrait = (story != null) ? (from x in story.traits.allTraits
			where x.def == trait.def
			select x).FirstOrDefault() : null;
			if (remTrait == null)
			{
				return;
			}
			if (pawn != null)
			{
				Pawn_StoryTracker story2 = pawn.story;
				if (story2 != null)
				{
					story2.traits.allTraits.Remove(remTrait);
				}
			}
			MSTraitChanger.TraitsUpdated(pawn);
			if (SendMsg)
			{
				string key = "MSExotic.TraitRemoved";
				NamedArgument arg = pawn?.LabelShort.CapitalizeFirst();
				Trait trait2 = trait;
				Messages.Message(key.Translate(arg, trait2?.Label.CapitalizeFirst(), doer.CapitalizeFirst()), pawn, MsgType, true);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000068C8 File Offset: 0x00004AC8
		public static void AddTrait(Pawn pawn, Trait trait, string doer, MessageTypeDef MsgType, bool SendMsg = true)
		{
			if (pawn != null)
			{
				Pawn_StoryTracker story = pawn.story;
				if (story != null)
				{
					story.traits.GainTrait(trait);
				}
			}
			MSTraitChanger.TraitsUpdated(pawn);
			if (SendMsg)
			{
				Messages.Message("MSExotic.TraitAdded".Translate(pawn?.LabelShort.CapitalizeFirst(), trait?.Label.CapitalizeFirst(), doer.CapitalizeFirst()), pawn, MsgType, true);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000694C File Offset: 0x00004B4C
		public static void TraitsUpdated(Pawn pawn)
		{
			if (pawn.workSettings != null)
			{
				pawn.workSettings.Notify_UseWorkPrioritiesChanged();
			}
			typeof(Pawn).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn, null);
			if (pawn.skills != null)
			{
				pawn.skills.Notify_SkillDisablesChanged();
			}
			if (!pawn.Dead && pawn.RaceProps.Humanlike)
			{
				pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
			}
		}
	}
}
