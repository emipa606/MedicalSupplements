using System.Collections.Generic;
using Verse;

namespace MSRegen
{
    // Token: 0x0200000E RID: 14
    public class MSRegenUtility
    {
        // Token: 0x0600003D RID: 61 RVA: 0x00004534 File Offset: 0x00002734
        internal static bool ImmuneTo(Pawn pawn, HediffDef def, out List<string> Immunities)
        {
            Immunities = new List<string>();
            var immune = false;
            var hediffs = pawn.health.hediffSet.hediffs;
            for (var i = 0; i < hediffs.Count; i++)
            {
                var curStage = hediffs[i].CurStage;
                if (curStage != null && curStage.makeImmuneTo != null)
                {
                    for (var j = 0; j < curStage.makeImmuneTo.Count; j++)
                    {
                        if (curStage.makeImmuneTo[j] == def)
                        {
                            Immunities.AddDistinct(hediffs[i].def.defName);
                            immune = true;
                        }
                    }
                }
            }

            return immune;
        }
    }
}