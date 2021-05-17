using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSDrugMix
{
    // Token: 0x02000021 RID: 33
    public class MSDrugMixUtility
    {
        // Token: 0x04000064 RID: 100
        [NoTranslate] public static string ChemIconPath = "Things/Building/Misc/MSDrugMixer/UI/MSDrugsMixerChem_Icon";

        // Token: 0x04000065 RID: 101
        public static List<ThingDef> cachedChunkList = new List<ThingDef>();

        // Token: 0x060000A6 RID: 166 RVA: 0x000081D0 File Offset: 0x000063D0
        public static Texture2D GetMSMixIcon(ThingDef t)
        {
            if (t != null)
            {
                if (t != null)
                {
                    var graphicData = t.graphicData;
                    if (graphicData?.texPath != null)
                    {
                        var texturePath = t.graphicData.texPath;
                        if (t.graphicData.graphicClass.Name == "Graphic_StackCount")
                        {
                            texturePath = texturePath + "/" + t.defName + "_a";
                        }

                        return ContentFinder<Texture2D>.Get(texturePath);
                    }
                }

                return ContentFinder<Texture2D>.Get(ChemIconPath);
            }

            return ContentFinder<Texture2D>.Get(ChemIconPath);
        }

        // Token: 0x060000A7 RID: 167 RVA: 0x00008258 File Offset: 0x00006458
        public static bool RCPProdValues(ThingDef t, out int ticks, out int minProd, out int maxProd,
            out string research)
        {
            ticks = 0;
            minProd = 0;
            maxProd = 0;
            research = "";
            if (t.defName == "Neutroamine")
            {
                ticks = 80;
                minProd = 10;
                maxProd = 15;
                research = "MSGlycerol";
                return true;
            }

            if (t.defName == "MSGlycerol")
            {
                ticks = 140;
                minProd = 5;
                maxProd = 5;
                research = "MSGlycerol";
                return true;
            }

            if (t.defName == "MSLithiumSalts")
            {
                ticks = 40;
                minProd = 50;
                maxProd = 50;
                research = "MSLithiumSalts";
                return true;
            }

            if (t.defName == "MSOpiumLatex")
            {
                ticks = 150;
                minProd = 10;
                maxProd = 25;
                research = "MSOpium";
                return true;
            }

            if (t.defName == "MSPhenol")
            {
                ticks = 100;
                minProd = 10;
                maxProd = 50;
                research = "MSPhenol";
                return true;
            }

            if (t.defName == "MSMercurySalts")
            {
                ticks = 300;
                minProd = 5;
                maxProd = 5;
                research = "MSMercury";
                return true;
            }

            if (t.defName == "MSEthylMercury")
            {
                ticks = 1000;
                minProd = 1;
                maxProd = 15;
                research = "MSMercury";
                return true;
            }

            if (t.defName == "MSSulphur")
            {
                ticks = 24;
                minProd = 50;
                maxProd = 50;
                research = "Stonecutting";
                return true;
            }

            if (t.defName == "MSSulphuricAcid")
            {
                ticks = 100;
                minProd = 10;
                maxProd = 25;
                research = "DrugProduction";
                return true;
            }

            if (t.defName == "MSEthanol")
            {
                ticks = 100;
                minProd = 10;
                maxProd = 15;
                research = "BiofuelRefining";
                return true;
            }

            if (t.defName == "MSHydrogenPeroxide")
            {
                ticks = 125;
                minProd = 10;
                maxProd = 30;
                research = "BiofuelRefining";
                return true;
            }

            if (t.defName == "MSVincaAlkaloid")
            {
                ticks = 240;
                minProd = 5;
                maxProd = 15;
                research = "MSVinca";
                return true;
            }

            if (t.defName == "MSHydrochloricAcid")
            {
                ticks = 200;
                minProd = 10;
                maxProd = 10;
                research = "DrugProduction";
                return true;
            }

            return false;
        }

        // Token: 0x060000A8 RID: 168 RVA: 0x0000848C File Offset: 0x0000668C
        public static List<ThingDef> ChunkList()
        {
            var list = new List<ThingDef>();
            var allthings = DefDatabase<ThingDef>.AllDefsListForReading;
            if (allthings.Count > 0)
            {
                foreach (var thingdef in allthings)
                {
                    var thingCats = thingdef?.thingCategories;
                    if (thingCats != null && thingCats.Count > 0)
                    {
                        using (var enumerator2 = thingCats.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                if (enumerator2.Current == ThingCategoryDefOf.StoneChunks)
                                {
                                    list.AddDistinct(thingdef);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            cachedChunkList = list;
            return list;
        }

        // Token: 0x060000A9 RID: 169 RVA: 0x00008558 File Offset: 0x00006758
        public static List<MSRCPListItem> GetRCPList(ThingDef thingdef)
        {
            var list = new List<MSRCPListItem>();
            list.Clear();
            MSRCPListItem item = default;
            if (thingdef.defName == "Neutroamine")
            {
                item.def = DefDatabase<ThingDef>.GetNamed("MSGlycerol");
                item.mixgrp = 1;
                item.num = 1;
                item.ratio = 1f;
                list.Add(item);
                item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                item.mixgrp = 2;
                item.num = 2;
                item.ratio = 1f;
                list.Add(item);
            }
            else if (thingdef.defName == "MSGlycerol")
            {
                item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                item.mixgrp = 1;
                item.num = 2;
                item.ratio = 1f;
                list.Add(item);
                item.def = DefDatabase<ThingDef>.GetNamed("Beer");
                item.mixgrp = 2;
                item.num = 1;
                item.ratio = 1f;
                list.Add(item);
                item.def = DefDatabase<ThingDef>.GetNamed("MSEthanol");
                item.mixgrp = 2;
                item.num = 1;
                item.ratio = 1f;
                list.Add(item);
            }
            else
            {
                if (thingdef.defName == "MSLithiumSalts")
                {
                    item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                    item.mixgrp = 1;
                    item.num = 5;
                    item.ratio = 0.1f;
                    list.Add(item);
                    item.def = DefDatabase<ThingDef>.GetNamed("MSPhenol");
                    item.mixgrp = 2;
                    item.num = 10;
                    item.ratio = 0.2f;
                    list.Add(item);
                    item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid");
                    item.mixgrp = 2;
                    item.num = 10;
                    item.ratio = 0.2f;
                    list.Add(item);
                    _ = new List<ThingDef>();
                    List<ThingDef> useChunkList;
                    if (cachedChunkList.Count > 0)
                    {
                        useChunkList = cachedChunkList;
                    }
                    else
                    {
                        useChunkList = ChunkList();
                    }

                    if (useChunkList.Count <= 0)
                    {
                        return list;
                    }

                    using (var enumerator = useChunkList.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var ChunkDef = enumerator.Current;
                            item.def = ChunkDef;
                            item.mixed = true;
                            item.mixgrp = 3;
                            item.num = 1;
                            item.ratio = 0.02f;
                            list.Add(item);
                        }

                        return list;
                    }
                }

                if (thingdef.defName == "MSOpiumLatex")
                {
                    item.def = DefDatabase<ThingDef>.GetNamed("MSOPSeedPod");
                    item.mixgrp = 1;
                    item.num = 2;
                    item.ratio = 1f;
                    list.Add(item);
                }
                else if (thingdef.defName == "MSPhenol")
                {
                    item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                    item.mixgrp = 1;
                    item.num = 3;
                    item.ratio = 1f;
                    list.Add(item);
                }
                else
                {
                    if (thingdef.defName == "MSMercurySalts")
                    {
                        _ = new List<ThingDef>();
                        List<ThingDef> useChunkList2;
                        if (cachedChunkList.Count > 0)
                        {
                            useChunkList2 = cachedChunkList;
                        }
                        else
                        {
                            useChunkList2 = ChunkList();
                        }

                        if (useChunkList2.Count <= 0)
                        {
                            return list;
                        }

                        using (var enumerator = useChunkList2.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                var ChunkDef2 = enumerator.Current;
                                item.def = ChunkDef2;
                                item.mixgrp = 1;
                                item.num = 1;
                                item.ratio = 0.2f;
                                list.Add(item);
                            }

                            return list;
                        }
                    }

                    if (thingdef.defName == "MSEthylMercury")
                    {
                        item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                        item.mixgrp = 1;
                        item.num = 10;
                        item.ratio = 1f;
                        list.Add(item);
                        item.def = DefDatabase<ThingDef>.GetNamed("MSMercurySalts");
                        item.mixed = false;
                        item.mixgrp = 2;
                        item.num = 5;
                        item.ratio = 1f;
                        list.Add(item);
                    }
                    else
                    {
                        if (thingdef.defName == "MSSulphur")
                        {
                            _ = new List<ThingDef>();
                            List<ThingDef> useChunkList3;
                            if (cachedChunkList.Count > 0)
                            {
                                useChunkList3 = cachedChunkList;
                            }
                            else
                            {
                                useChunkList3 = ChunkList();
                            }

                            if (useChunkList3.Count <= 0)
                            {
                                return list;
                            }

                            using (var enumerator = useChunkList3.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    var ChunkDef3 = enumerator.Current;
                                    item.def = ChunkDef3;
                                    item.mixgrp = 1;
                                    item.num = 1;
                                    item.ratio = 0.02f;
                                    list.Add(item);
                                }

                                return list;
                            }
                        }

                        if (thingdef.defName == "MSSulphuricAcid")
                        {
                            item.def = DefDatabase<ThingDef>.GetNamed("MSSulphur");
                            item.mixgrp = 1;
                            item.num = 3;
                            item.ratio = 1f;
                            list.Add(item);
                        }
                        else if (thingdef.defName == "MSEthanol")
                        {
                            item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                            item.mixgrp = 1;
                            item.num = 2;
                            item.ratio = 1f;
                            list.Add(item);
                            item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid");
                            item.mixgrp = 2;
                            item.num = 1;
                            item.ratio = 1f;
                            list.Add(item);
                        }
                        else if (thingdef.defName == "MSHydrogenPeroxide")
                        {
                            item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                            item.mixgrp = 1;
                            item.num = 1;
                            item.ratio = 1f;
                            list.Add(item);
                            item.def = DefDatabase<ThingDef>.GetNamed("Neutroamine");
                            item.mixgrp = 2;
                            item.num = 1;
                            item.ratio = 0.2f;
                            list.Add(item);
                            item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid");
                            item.mixgrp = 3;
                            item.num = 1;
                            item.ratio = 0.2f;
                            list.Add(item);
                        }
                        else if (thingdef.defName == "MSVincaAlkaloid")
                        {
                            item.def = DefDatabase<ThingDef>.GetNamed("MSPerrywinkleLeaves");
                            item.mixgrp = 1;
                            item.num = 1;
                            item.ratio = 5f;
                            list.Add(item);
                            item.def = DefDatabase<ThingDef>.GetNamed("MSPhenol");
                            item.mixgrp = 2;
                            item.num = 1;
                            item.ratio = 0.2f;
                            list.Add(item);
                        }
                        else if (thingdef.defName == "MSHydrochloricAcid")
                        {
                            _ = new List<ThingDef>();
                            List<ThingDef> useChunkList4;
                            if (cachedChunkList.Count > 0)
                            {
                                useChunkList4 = cachedChunkList;
                            }
                            else
                            {
                                useChunkList4 = ChunkList();
                            }

                            if (useChunkList4.Count > 0)
                            {
                                foreach (var ChunkDef4 in useChunkList4)
                                {
                                    item.def = ChunkDef4;
                                    item.mixed = true;
                                    item.mixgrp = 1;
                                    item.num = 1;
                                    item.ratio = 0.1f;
                                    list.Add(item);
                                }
                            }

                            item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid");
                            item.mixgrp = 2;
                            item.num = 5;
                            item.ratio = 1f;
                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

        // Token: 0x060000AA RID: 170 RVA: 0x00008D70 File Offset: 0x00006F70
        public static List<string> GetMixList()
        {
            return new List<string>
            {
                "Neutroamine",
                "MSGlycerol",
                "MSPhenol",
                "MSOpiumLatex",
                "MSLithiumSalts",
                "MSMercurySalts",
                "MSEthylMercury",
                "MSSulphur",
                "MSSulphuricAcid",
                "MSEthanol",
                "MSHydrogenPeroxide",
                "MSVincaAlkaloid",
                "MSHydrochloricAcid"
            };
        }

        // Token: 0x060000AB RID: 171 RVA: 0x00008E14 File Offset: 0x00007014
        public static List<int> GetMaxStock()
        {
            return new List<int>
            {
                25,
                50,
                75,
                100,
                150,
                200,
                250,
                300,
                400,
                500,
                750,
                1000,
                0
            };
        }

        // Token: 0x060000AC RID: 172 RVA: 0x00008EA8 File Offset: 0x000070A8
        public static int StringToInt(string ToConvert)
        {
            if (ToConvert != "No Limit" && int.TryParse(ToConvert, out var Value))
            {
                return Value;
            }

            return 0;
        }

        // Token: 0x0200003D RID: 61
        public struct MSRCPListItem
        {
            // Token: 0x04000094 RID: 148
            internal ThingDef def;

            // Token: 0x04000095 RID: 149
            internal bool mixed;

            // Token: 0x04000096 RID: 150
            internal int mixgrp;

            // Token: 0x04000097 RID: 151
            internal int num;

            // Token: 0x04000098 RID: 152
            internal float ratio;
        }
    }
}