using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSExotic
{
    // Token: 0x02000016 RID: 22
    public class JobDriver_MSWearCondom : JobDriver
    {
        // Token: 0x06000050 RID: 80 RVA: 0x0000563C File Offset: 0x0000383C
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn1 = pawn;
            var job1 = job;
            LocalTargetInfo target = job1.GetTarget(TargetIndex.A).Thing;
            return pawn1.Reserve(target, job1);
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00005676 File Offset: 0x00003876
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
            var prepare = toil;
            prepare.WithProgressBarToilDelay(TargetIndex.A);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.defaultCompleteMode = ToilCompleteMode.Delay;
            prepare.defaultDuration = 300;
            yield return prepare;
            yield return Toils_General.Do(delegate
            {
                JobCondition JC;
                if (TargetA.Thing != null && TargetA.Thing.Spawned)
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
                        var pawn2 = actor;
                        var jobs = pawn2.jobs;
                        jobs?.jobQueue.EnqueueFirst(newLovin);
                    }
                }

                EndJobWith(JC);
            });
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00005688 File Offset: 0x00003888
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
}