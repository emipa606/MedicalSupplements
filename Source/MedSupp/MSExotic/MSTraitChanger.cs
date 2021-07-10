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
            var story = pawn.story;
            var remTrait = story != null
                ? (from x in story.traits.allTraits
                    where x.def == trait.def
                    select x).FirstOrDefault()
                : null;
            if (remTrait == null)
            {
                return;
            }

            var story2 = pawn.story;
            story2?.traits.allTraits.Remove(remTrait);

            TraitsUpdated(pawn);
            if (!SendMsg)
            {
                return;
            }

            var key = "MSExotic.TraitRemoved";
            NamedArgument arg = pawn.LabelShort.CapitalizeFirst();
            var trait2 = trait;
            Messages.Message(key.Translate(arg, trait2?.Label.CapitalizeFirst(), doer.CapitalizeFirst()), pawn,
                MsgType);
        }

        // Token: 0x0600007D RID: 125 RVA: 0x000068C8 File Offset: 0x00004AC8
        public static void AddTrait(Pawn pawn, Trait trait, string doer, MessageTypeDef MsgType, bool SendMsg = true)
        {
            var story = pawn?.story;
            story?.traits.GainTrait(trait);

            TraitsUpdated(pawn);
            if (SendMsg)
            {
                Messages.Message(
                    "MSExotic.TraitAdded".Translate(pawn?.LabelShort.CapitalizeFirst(), trait?.Label.CapitalizeFirst(),
                        doer.CapitalizeFirst()), pawn, MsgType);
            }
        }

        // Token: 0x0600007E RID: 126 RVA: 0x0000694C File Offset: 0x00004B4C
        public static void TraitsUpdated(Pawn pawn)
        {
            pawn.workSettings?.Notify_UseWorkPrioritiesChanged();

            typeof(Pawn).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(pawn, null);
            pawn.skills?.Notify_SkillDisablesChanged();

            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }
    }
}