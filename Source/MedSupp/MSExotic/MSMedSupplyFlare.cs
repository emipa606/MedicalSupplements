using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSExotic
{
    // Token: 0x0200001A RID: 26
    public class MSMedSupplyFlare : Gas
    {
        // Token: 0x06000068 RID: 104 RVA: 0x00006068 File Offset: 0x00004268
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x06000069 RID: 105 RVA: 0x0000609D File Offset: 0x0000429D
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x0600006A RID: 106 RVA: 0x000060B8 File Offset: 0x000042B8
        public override void Tick()
        {
            if (destroyTick <= Find.TickManager.TicksGame)
            {
                Destroy();
            }

            graphicRotation += graphicRotationSpeed;
            if (this.DestroyedOrNull())
            {
                return;
            }

            var TargetMap = Map;
            var TargetCell = Position;
            if (Find.TickManager.TicksGame % 10 == 0)
            {
                FleckMaker.ThrowSmoke(this.TrueCenter() + new Vector3(0f, 0f, 0.1f), TargetMap, 1f);
            }

            if (Find.TickManager.TicksGame % 300 != 0)
            {
                return;
            }

            var DropSpot = GetDropSpot(TargetMap, TargetCell);
            var supplies = GetSupplies(SupplyList());
            DoMSMedSupplies(TargetMap, DropSpot, supplies);
            if (!this.DestroyedOrNull())
            {
                Destroy();
            }
        }

        // Token: 0x0600006B RID: 107 RVA: 0x00006184 File Offset: 0x00004384
        public void DoMSMedSupplies(Map map, IntVec3 DropSpot, List<Thing> things)
        {
            DropPodUtility.DropThingsNear(DropSpot, map, things, 110, false, true);
            Find.LetterStack.ReceiveLetter("MSExotic.MedSupplyLabel".Translate(), "MSExotic.MedSupplyEvent".Translate(),
                LetterDefOf.PositiveEvent, new TargetInfo(DropSpot, map));
        }

        // Token: 0x0600006C RID: 108 RVA: 0x000061D3 File Offset: 0x000043D3
        public IntVec3 GetDropSpot(Map map, IntVec3 root)
        {
            return root;
        }

        // Token: 0x0600006D RID: 109 RVA: 0x000061D8 File Offset: 0x000043D8
        public List<ThingDef> SupplyList()
        {
            var list = new List<ThingDef>();
            ThingDef medicineIndustrial = null;
            for (var count = 0; count < 7; count++)
            {
                switch (count)
                {
                    case 0:
                        medicineIndustrial = ThingDefOf.MedicineIndustrial;
                        break;
                    case 1:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSASBandage", false);
                        break;
                    case 2:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSRimoxicillin", false);
                        break;
                    case 3:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSRimCodamol", false);
                        break;
                    case 4:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSRimzac", false);
                        break;
                    case 5:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSRimedicrem", false);
                        break;
                    case 6:
                        medicineIndustrial = DefDatabase<ThingDef>.GetNamed("MSRimBurnEaze", false);
                        break;
                }

                if (medicineIndustrial != null)
                {
                    list.Add(medicineIndustrial);
                }
            }

            return list;
        }

        // Token: 0x0600006E RID: 110 RVA: 0x00006288 File Offset: 0x00004488
        public List<Thing> GetSupplies(List<ThingDef> list)
        {
            var supplies = new List<Thing>();
            foreach (var thingDef in list)
            {
                var thing = MakeSupplies(thingDef);
                if (thing != null)
                {
                    supplies.Add(thing);
                }
            }

            return supplies;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x000062E8 File Offset: 0x000044E8
        public Thing MakeSupplies(ThingDef thingDef)
        {
            if (thingDef == null)
            {
                return null;
            }

            var supply = ThingMaker.MakeThing(thingDef);
            supply.stackCount = Math.Min(10, thingDef.stackLimit);

            return supply;
        }
    }
}