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
            foreach (var hediff in hediffs)
            {
                var curStage = hediff.CurStage;
                if (curStage?.makeImmuneTo == null)
                {
                    continue;
                }

                foreach (var hediffDef in curStage.makeImmuneTo)
                {
                    if (hediffDef != def)
                    {
                        continue;
                    }

                    Immunities.AddDistinct(hediff.def.defName);
                    immune = true;
                }
            }

            return immune;
        }
    }
}