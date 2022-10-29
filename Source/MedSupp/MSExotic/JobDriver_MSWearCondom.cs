using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSExotic;

public class JobDriver_MSWearCondom : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var pawn1 = pawn;
        var job1 = job;
        LocalTargetInfo target = job1.GetTarget(TargetIndex.A).Thing;
        return pawn1.Reserve(target, job1);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var actor = GetActor();
        this.FailOnBurningImmobile(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);
        var toil = new Toil
        {
            tickAction = delegate { }
        };
        toil.WithProgressBarToilDelay(TargetIndex.A);
        toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        toil.defaultCompleteMode = ToilCompleteMode.Delay;
        toil.defaultDuration = 300;
        yield return toil;
        yield return Toils_General.Do(delegate
        {
            JobCondition JC;
            if (TargetA.Thing is { Spawned: true })
            {
                MSExoticUtility.DoMSCondom(actor, TargetA.Thing.def);
                var stack = TargetA.Thing.stackCount;
                if (stack > 1)
                {
                    stack--;
                    TargetA.Thing.stackCount = stack;
                }
                else
                {
                    TargetA.Thing.Destroy();
                }

                JC = JobCondition.Succeeded;
            }
            else
            {
                JC = JobCondition.Incompletable;
            }

            Thing thing;
            if (actor == null)
            {
                thing = null;
            }
            else
            {
                var ownership = actor.ownership;
                thing = ownership?.OwnedBed;
            }

            var LovinBed = thing;
            if (LovinBed != null)
            {
                var partnerInMyBed = GetCondomPartnerInMyBed(actor, LovinBed as Building_Bed);
                if (partnerInMyBed != null && partnerInMyBed.health.capacities.CanBeAwake)
                {
                    var newLovin = new Job(JobDefOf.Lovin, partnerInMyBed, LovinBed);
                    var jobs = actor.jobs;
                    jobs?.jobQueue.EnqueueFirst(newLovin);
                }
            }

            EndJobWith(JC);
        });
    }

    public static Pawn GetCondomPartnerInMyBed(Pawn pawn, Building_Bed LovinBed)
    {
        if (LovinBed.SleepingSlotsCount <= 1)
        {
            return null;
        }

        if (!LovePartnerRelationUtility.HasAnyLovePartner(pawn))
        {
            return null;
        }

        foreach (var curOccupant in LovinBed.CurOccupants)
        {
            if (curOccupant != pawn && LovePartnerRelationUtility.LovePartnerRelationExists(pawn, curOccupant))
            {
                return curOccupant;
            }
        }

        return null;
    }
}