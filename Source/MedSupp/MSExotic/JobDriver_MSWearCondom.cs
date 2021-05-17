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
            var pawn = this.pawn;
            var job = this.job;
            LocalTargetInfo target = job.GetTarget(TargetIndex.A).Thing;
            return pawn.Reserve(target, job);
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00005676 File Offset: 0x00003876
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var pawn = GetActor();
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
                    MSExoticUtility.DoMSCondom(pawn, TargetA.Thing.def);
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
                if (pawn == null)
                {
                    thing = null;
                }
                else
                {
                    var ownership = pawn.ownership;
                    thing = ownership?.OwnedBed;
                }

                var LovinBed = thing;
                if (LovinBed != null)
                {
                    var partnerInMyBed = GetCondomPartnerInMyBed(pawn, LovinBed as Building_Bed);
                    if (partnerInMyBed != null && partnerInMyBed.health.capacities.CanBeAwake)
                    {
                        var newLovin = new Job(JobDefOf.Lovin, partnerInMyBed, LovinBed);
                        if (newLovin != null)
                        {
                            var pawn2 = pawn;
                            if (pawn2 != null)
                            {
                                var jobs = pawn2.jobs;
                                if (jobs != null)
                                {
                                    jobs.jobQueue.EnqueueFirst(newLovin);
                                }
                            }
                        }
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