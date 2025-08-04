using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace MSExotic;

public static class MSTraitChanger
{
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
        Messages.Message(key.Translate(arg, trait?.Label.CapitalizeFirst(), doer.CapitalizeFirst()), pawn,
            MsgType);
    }

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

    private static void TraitsUpdated(Pawn pawn)
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