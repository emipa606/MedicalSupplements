using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSDrugMix;

public class MSDrugMixUtility
{
    [NoTranslate] public static string ChemIconPath = "Things/Building/Misc/MSDrugMixer/UI/MSDrugsMixerChem_Icon";

    public static List<ThingDef> cachedChunkList = new List<ThingDef>();

    public static Texture2D GetMSMixIcon(ThingDef t)
    {
        if (t == null)
        {
            return ContentFinder<Texture2D>.Get(ChemIconPath);
        }

        var graphicData = t.graphicData;
        if (graphicData?.texPath == null)
        {
            return ContentFinder<Texture2D>.Get(ChemIconPath);
        }

        var texturePath = t.graphicData.texPath;
        if (t.graphicData.graphicClass.Name == "Graphic_StackCount")
        {
            texturePath = $"{texturePath}/{t.defName}_a";
        }

        return ContentFinder<Texture2D>.Get(texturePath);
    }

    public static bool RCPProdValues(ThingDef t, out int ticks, out int minProd, out int maxProd,
        out string research)
    {
        ticks = 0;
        minProd = 0;
        maxProd = 0;
        research = "";
        switch (t.defName)
        {
            case "Neutroamine":
                ticks = 80;
                minProd = 10;
                maxProd = 15;
                research = "MSGlycerol";
                return true;
            case "MSGlycerol":
                ticks = 140;
                minProd = 5;
                maxProd = 5;
                research = "MSGlycerol";
                return true;
            case "MSLithiumSalts":
                ticks = 40;
                minProd = 50;
                maxProd = 50;
                research = "MSLithiumSalts";
                return true;
            case "MSOpiumLatex":
                ticks = 150;
                minProd = 10;
                maxProd = 25;
                research = "MSOpium";
                return true;
            case "MSPhenol":
                ticks = 100;
                minProd = 10;
                maxProd = 50;
                research = "MSPhenol";
                return true;
            case "MSMercurySalts":
                ticks = 300;
                minProd = 5;
                maxProd = 5;
                research = "MSMercury";
                return true;
            case "MSEthylMercury":
                ticks = 1000;
                minProd = 1;
                maxProd = 15;
                research = "MSMercury";
                return true;
            case "MSSulphur":
                ticks = 24;
                minProd = 50;
                maxProd = 50;
                research = "Stonecutting";
                return true;
            case "MSSulphuricAcid":
                ticks = 100;
                minProd = 10;
                maxProd = 25;
                research = "DrugProduction";
                return true;
            case "MSEthanol":
                ticks = 100;
                minProd = 10;
                maxProd = 15;
                research = "BiofuelRefining";
                return true;
            case "MSHydrogenPeroxide":
                ticks = 125;
                minProd = 10;
                maxProd = 30;
                research = "BiofuelRefining";
                return true;
            case "MSVincaAlkaloid":
                ticks = 240;
                minProd = 5;
                maxProd = 15;
                research = "MSVinca";
                return true;
        }

        if (t.defName != "MSHydrochloricAcid")
        {
            return false;
        }

        ticks = 200;
        minProd = 10;
        maxProd = 10;
        research = "DrugProduction";
        return true;
    }

    public static List<ThingDef> ChunkList()
    {
        var list = new List<ThingDef>();
        var allthings = DefDatabase<ThingDef>.AllDefsListForReading;
        if (allthings.Count > 0)
        {
            foreach (var thingdef in allthings)
            {
                var thingCats = thingdef?.thingCategories;
                if (thingCats is not { Count: > 0 })
                {
                    continue;
                }

                using var enumerator2 = thingCats.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current != ThingCategoryDefOf.StoneChunks)
                    {
                        continue;
                    }

                    list.AddDistinct(thingdef);
                    break;
                }
            }
        }

        cachedChunkList = list;
        return list;
    }

    public static List<MSRCPListItem> GetRCPList(ThingDef thingdef)
    {
        var list = new List<MSRCPListItem>();
        list.Clear();
        MSRCPListItem item = default;
        switch (thingdef.defName)
        {
            case "Neutroamine":
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
                break;
            case "MSGlycerol":
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
                break;
            default:
            {
                switch (thingdef.defName)
                {
                    case "MSLithiumSalts":
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
                        var useChunkList = cachedChunkList.Count > 0 ? cachedChunkList : ChunkList();

                        if (useChunkList.Count <= 0)
                        {
                            return list;
                        }

                        using var enumerator = useChunkList.GetEnumerator();
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
                    case "MSOpiumLatex":
                        item.def = DefDatabase<ThingDef>.GetNamed("MSOPSeedPod");
                        item.mixgrp = 1;
                        item.num = 2;
                        item.ratio = 1f;
                        list.Add(item);
                        break;
                    case "MSPhenol":
                        item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel");
                        item.mixgrp = 1;
                        item.num = 3;
                        item.ratio = 1f;
                        list.Add(item);
                        break;
                    default:
                    {
                        switch (thingdef.defName)
                        {
                            case "MSMercurySalts":
                            {
                                _ = new List<ThingDef>();
                                var useChunkList2 = cachedChunkList.Count > 0 ? cachedChunkList : ChunkList();

                                if (useChunkList2.Count <= 0)
                                {
                                    return list;
                                }

                                using var enumerator = useChunkList2.GetEnumerator();
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
                            case "MSEthylMercury":
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
                                break;
                            default:
                            {
                                switch (thingdef.defName)
                                {
                                    case "MSSulphur":
                                    {
                                        _ = new List<ThingDef>();
                                        var useChunkList3 = cachedChunkList.Count > 0 ? cachedChunkList : ChunkList();

                                        if (useChunkList3.Count <= 0)
                                        {
                                            return list;
                                        }

                                        using var enumerator = useChunkList3.GetEnumerator();
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
                                    case "MSSulphuricAcid":
                                        item.def = DefDatabase<ThingDef>.GetNamed("MSSulphur");
                                        item.mixgrp = 1;
                                        item.num = 3;
                                        item.ratio = 1f;
                                        list.Add(item);
                                        break;
                                    case "MSEthanol":
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
                                        break;
                                    case "MSHydrogenPeroxide":
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
                                        break;
                                    case "MSVincaAlkaloid":
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
                                        break;
                                    case "MSHydrochloricAcid":
                                    {
                                        _ = new List<ThingDef>();
                                        var useChunkList4 = cachedChunkList.Count > 0 ? cachedChunkList : ChunkList();

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
                                        break;
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }

                break;
            }
        }

        return list;
    }

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

    public static int StringToInt(string ToConvert)
    {
        if (ToConvert != "No Limit" && int.TryParse(ToConvert, out var Value))
        {
            return Value;
        }

        return 0;
    }

    public struct MSRCPListItem
    {
        internal ThingDef def;

        internal bool mixed;

        internal int mixgrp;

        internal int num;

        internal float ratio;
    }
}