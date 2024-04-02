using UnityEngine;
using Verse;

namespace MSTend;

public class MSHediffCompProperties_TendDuration : HediffCompProperties
{
    private readonly float baseTendDurationHours = -1f;

    public readonly int disappearsAtTotalTendQuality = -1;

    public readonly bool showTendQuality = true;

    private readonly float tendOverlapHours = 3f;

    [LoadAlias("labelSolidTreatedWell")] public string labelSolidTendedWell;

    [LoadAlias("labelTreatedWell")] public string labelTendedWell;

    [LoadAlias("labelTreatedWellInner")] public string labelTendedWellInner;

    public float severityPerDayTended;

    public bool tendAllAtOnce;

    public MSHediffCompProperties_TendDuration()
    {
        compClass = typeof(MSHediffComp_TendDuration);
    }

    public bool TendIsPermanent => baseTendDurationHours < 0f;

    public int TendTicksFull
    {
        get
        {
            if (TendIsPermanent)
            {
                Log.ErrorOnce("Queried TendTicksFull on permanent-tend Hediff.", 6163263);
            }

            return Mathf.RoundToInt((baseTendDurationHours + tendOverlapHours) * 2500f);
        }
    }

    public int TendTicksBase
    {
        get
        {
            if (TendIsPermanent)
            {
                Log.ErrorOnce("Queried TendTicksBase on permanent-tend Hediff.", 61621263);
            }

            return Mathf.RoundToInt(baseTendDurationHours * 2500f);
        }
    }

    public int TendTicksOverlap
    {
        get
        {
            if (TendIsPermanent)
            {
                Log.ErrorOnce("Queried TendTicksOverlap on permanent-tend Hediff.", 1963263);
            }

            return Mathf.RoundToInt(tendOverlapHours * 2500f);
        }
    }
}