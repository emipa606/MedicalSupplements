using System;
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
			Pawn pawn = this.pawn;
			Job job = this.job;
			LocalTargetInfo target = job.GetTarget(TargetIndex.A).Thing;
			return pawn.Reserve(target, job, 1, -1, null, true);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005676 File Offset: 0x00003876
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Pawn pawn = base.GetActor();
			this.FailOnBurningImmobile(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil toil = new Toil();
			toil.tickAction = delegate()
			{
			};
			Toil prepare = toil;
			prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			prepare.defaultCompleteMode = ToilCompleteMode.Delay;
			prepare.defaultDuration = 300;
			yield return prepare;
			yield return Toils_General.Do(delegate
			{
				JobCondition JC;
				if (this.TargetA.Thing != null && this.TargetA.Thing.Spawned)
				{
					MSExoticUtility.DoMSCondom(pawn, this.TargetA.Thing.def);
					int stack = this.TargetA.Thing.stackCount;
					if (stack > 1)
					{
						stack--;
						this.TargetA.Thing.stackCount = stack;
					}
					else
					{
						this.TargetA.Thing.Destroy(DestroyMode.Vanish);
					}
					JC = JobCondition.Succeeded;
				}
				else
				{
					JC = JobCondition.Incompletable;
				}
				pawn = pawn;
				Thing thing;
				if (pawn == null)
				{
					thing = null;
				}
				else
				{
					Pawn_Ownership ownership = pawn.ownership;
					thing = ((ownership != null) ? ownership.OwnedBed : null);
				}
				Thing LovinBed = thing;
				if (LovinBed != null)
				{
					Pawn partnerInMyBed = JobDriver_MSWearCondom.GetCondomPartnerInMyBed(pawn, LovinBed as Building_Bed);
					if (partnerInMyBed != null && partnerInMyBed.health.capacities.CanBeAwake)
					{
						Job newLovin = new Job(JobDefOf.Lovin, partnerInMyBed, LovinBed);
						if (newLovin != null)
						{
							Pawn pawn2 = pawn;
							if (pawn2 != null)
							{
								Pawn_JobTracker jobs = pawn2.jobs;
								if (jobs != null)
								{
									jobs.jobQueue.EnqueueFirst(newLovin, null);
								}
							}
						}
					}
				}
				this.EndJobWith(JC);
			});
			yield break;
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
			foreach (Pawn curOccupant in LovinBed.CurOccupants)
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
