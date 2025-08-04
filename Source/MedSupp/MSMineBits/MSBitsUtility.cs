using Verse;

namespace MSMineBits;

public class MSBitsUtility
{
    public const string ChunkGranite = "ChunkGranite";

    public const string ChunkMarble = "ChunkMarble";

    public const string ChunkLimestone = "ChunkLimestone";

    public const string ChunkSlate = "ChunkSlate";

    public const string ChunkSandstone = "ChunkSandstone";

    public const string CollapsedRocks = "CollapsedRocks";

    public const string rxCollapsedRoofRocks = "rxCollapsedRoofRocks";

    internal const string ChunkEmperadordark = "ChunkEmperadordark";

    internal const string ChunkBlueschist = "ChunkBlueschist";

    internal const string ChunkSerpentinite = "ChunkSerpentinite";

    internal const string ChunkDacite = "ChunkDacite";

    internal const string ChunkSovite = "ChunkSovite";

    internal const string ChunkChalk = "ChunkChalk";

    internal const string ChunkCreoleMarble = "ChunkCreoleMarble";

    internal const string ChunkEtowahMarble = "ChunkEtowahMarble";

    internal const string ChunkGreenSchist = "ChunkGreenSchist";

    internal const string ChunkVibrantDunite = "ChunkVibrantDunite";

    internal const string ChunkDarkAndesite = "ChunkDarkAndesite";

    internal const string ChunkAnorthosite = "ChunkAnorthosite";

    internal const string ChunkBasalt = "ChunkBasalt";

    internal const string ChunkCharnockite = "ChunkCharnockite";

    internal const string ChunkGreenGabbro = "ChunkGreenGabbro";

    internal const string ChunkLherzolite = "ChunkLherzolite";

    internal const string ChunkMonzonite = "ChunkMonzonite";

    internal const string ChunkRhyolite = "ChunkRhyolite";

    internal const string ChunkScoria = "ChunkScoria";

    internal const string ChunkJaspillite = "ChunkJaspillite";

    internal const string ChunkLignite = "ChunkLignite";

    internal const string ChunkSiltstone = "ChunkSiltstone";

    internal const string ChunkMigmatite = "ChunkMigmatite";

    internal const string ChunkThometzekite = "ChunkThometzekite";

    internal const string ChunkLepidolite = "ChunkLepidolite";

    internal const string ChunkClaystone = "ChunkClayStone";

    internal const string ChunkAndesite = "ChunkAndesite";

    internal const string ChunkSyenite = "ChunkSyenite";

    internal const string ChunkGneiss = "ChunkGneiss";

    internal const string ChunkQuartzite = "ChunkQuartzite";

    internal const string ChunkSchist = "ChunkSchist";

    internal const string ChunkGabbro = "ChunkGabbro";

    internal const string ChunkDiorite = "ChunkDiorite";

    internal const string ChunkDunite = "ChunkDunite";

    internal const string ChunkPegmatite = "ChunkPegmatite";

    public const string MSLithiumSalts = "MSLithiumSalts";

    public const string MSMercurySalts = "MSMercurySalts";

    public const string MSSulphur = "MSSulphur";

    private static readonly Pair<string, float>[] bitschance =
    [
        new("MSLithiumSalts", 0.85f),
        new("MSMercurySalts", 0.1f),
        new("MSSulphur", 1f)
    ];

    public static bool GetIsBitsSource(ThingDef defSource, bool isSource, Pawn pawn, out ThingDef bitsdef,
        out int bitsyield)
    {
        bitsdef = null;
        bitsyield = 0;
        if (!GetBitsSource(defSource) && !isSource)
        {
            return false;
        }

        bitsdef = DefDatabase<ThingDef>.GetNamed(bitschance.RandomElementByWeight(x => x.Second).First, false);
        if (bitsdef == null)
        {
            return false;
        }

        bitsyield = GetBitsYield(defSource, bitsdef);
        return bitsyield > 0;
    }

    public static int GetBitsYield(ThingDef defSource, ThingDef def)
    {
        if (defSource != null)
        {
            switch (defSource.defName)
            {
                case "ChunkLepidolite":
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(1, 4);
                            }
                        }
                        else
                        {
                            return Rand.Range(1, 1);
                        }
                    }
                    else
                    {
                        return Rand.Range(10, 15);
                    }

                    break;
                }
                case "ChunkSerpentinite":
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(10, 15);
                            }
                        }
                        else
                        {
                            return Rand.Range(1, 1);
                        }
                    }
                    else
                    {
                        return Rand.Range(1, 3);
                    }

                    break;
                }
                case "ChunkDunite":
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(3, 7);
                            }
                        }
                        else
                        {
                            return Rand.Range(2, 5);
                        }
                    }
                    else
                    {
                        return Rand.Range(1, 2);
                    }

                    break;
                }
                case "ChunkPegmatite":
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(1, 2);
                            }
                        }
                        else
                        {
                            return Rand.Range(1, 1);
                        }
                    }
                    else
                    {
                        return Rand.Range(5, 20);
                    }

                    break;
                }
                case "ChunkMarble" or "ChunkLimestone" or "ChunkEmperadordark"
                    or "ChunkBlueschist" or "ChunkGreenSchist" or "ChunkDacite" or "ChunkSovite"
                    or "ChunkChalk" or "ChunkCreoleMarble" or "ChunkEtowahMarble" or "ChunkDiorite":
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(1, 3);
                            }
                        }
                        else
                        {
                            return Rand.Range(1, 1);
                        }
                    }
                    else
                    {
                        return Rand.Range(3, 7);
                    }

                    break;
                }
                default:
                {
                    var defName = def.defName;
                    if (defName != "MSLithiumSalts")
                    {
                        if (defName != "MSMercurySalts")
                        {
                            if (defName == "MSSulphur")
                            {
                                return Rand.Range(5, 10);
                            }
                        }
                        else
                        {
                            return Rand.Range(1, 1);
                        }
                    }
                    else
                    {
                        return Rand.Range(5, 10);
                    }

                    break;
                }
            }
        }
        else
        {
            var defName = def.defName;
            if (defName != "MSLithiumSalts")
            {
                if (defName != "MSMercurySalts")
                {
                    if (defName == "MSSulphur")
                    {
                        return Rand.Range(5, 10);
                    }
                }
                else
                {
                    return Rand.Range(1, 1);
                }
            }
            else
            {
                return Rand.Range(5, 10);
            }
        }

        return 0;
    }

    private static uint ComputeStringHash(string s)
    {
        uint num = 0;
        if (s == null)
        {
            return num;
        }

        num = 2166136261U;
        foreach (var c in s)
        {
            num = (c ^ num) * 16777619U;
        }

        return num;
    }

    private static bool GetBitsSource(ThingDef def)
    {
        if (def == null)
        {
            return false;
        }

        var defName = def.defName;
        var num = ComputeStringHash(defName);
        switch (num)
        {
            case <= 1154306412U and <= 702995605U:
            {
                switch (num)
                {
                    case <= 450668030U and <= 135430043U:
                    {
                        if (num != 130326325U)
                        {
                            if (num != 135430043U)
                            {
                                return false;
                            }

                            if (defName == "ChunkBasalt")
                            {
                                return true;
                            }
                        }
                        else if (defName == "ChunkCreoleMarble")
                        {
                            return true;
                        }

                        break;
                    }
                    case <= 450668030U when num != 237089535U:
                    {
                        if (num != 334370396U)
                        {
                            if (num != 450668030U)
                            {
                                return false;
                            }

                            if (defName == "ChunkAnorthosite")
                            {
                                return true;
                            }
                        }
                        else if (defName == "ChunkBlueschist")
                        {
                            return true;
                        }

                        break;
                    }
                    case <= 450668030U when defName == "ChunkGreenSchist":
                        return true;
                    case <= 450668030U:
                        break;
                    case <= 468849279U when num != 459180747U:
                    {
                        if (num != 468849279U)
                        {
                            return false;
                        }

                        if (defName == "ChunkGneiss")
                        {
                            return true;
                        }

                        break;
                    }
                    case <= 468849279U when defName == "ChunkLepidolite":
                        return true;
                    case <= 468849279U:
                        break;
                    default:
                    {
                        if (num != 497423552U)
                        {
                            if (num != 547426595U)
                            {
                                if (num != 702995605U)
                                {
                                    return false;
                                }

                                if (defName == "ChunkDarkAndesite")
                                {
                                    return true;
                                }
                            }
                            else if (defName == "ChunkDunite")
                            {
                                return true;
                            }
                        }
                        else if (defName == "ChunkDiorite")
                        {
                            return true;
                        }

                        break;
                    }
                }

                break;
            }
            case <= 990094961U:
            {
                if (num <= 803651291U)
                {
                    if (num != 754271499U)
                    {
                        if (num != 803651291U)
                        {
                            return false;
                        }

                        if (defName == "ChunkQuartzite")
                        {
                            return true;
                        }
                    }
                    else if (defName == "ChunkSiltstone")
                    {
                        return true;
                    }
                }
                else if (num != 840754687U)
                {
                    if (num != 930377356U)
                    {
                        if (num != 990094961U)
                        {
                            return false;
                        }

                        if (defName == "ChunkGabbro")
                        {
                            return true;
                        }
                    }
                    else if (defName == "ChunkLignite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkSlate")
                {
                    return true;
                }

                break;
            }
            case <= 1113897122U:
            {
                if (num != 1103574401U)
                {
                    if (num != 1113897122U)
                    {
                        return false;
                    }

                    if (defName == "ChunkDacite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkEtowahMarble")
                {
                    return true;
                }

                break;
            }
            case <= 1154306412U when num != 1117948611U:
            {
                if (num != 1138947383U)
                {
                    if (num != 1154306412U)
                    {
                        return false;
                    }

                    if (defName == "ChunkSchist")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkSyenite")
                {
                    return true;
                }

                break;
            }
            case <= 1154306412U when defName == "ChunkMarble":
                return true;
            case <= 1154306412U:
                break;
            case <= 3134642644U and <= 2223887258U:
            {
                if (num <= 1807593693U)
                {
                    if (num != 1789891285U)
                    {
                        if (num != 1807593693U)
                        {
                            return false;
                        }

                        if (defName == "ChunkChalk")
                        {
                            return true;
                        }
                    }
                    else if (defName == "ChunkMonzonite")
                    {
                        return true;
                    }
                }
                else if (num != 2078373069U)
                {
                    if (num != 2104149328U)
                    {
                        if (num != 2223887258U)
                        {
                            return false;
                        }

                        if (defName == "ChunkSovite")
                        {
                            return true;
                        }
                    }
                    else if (defName == "ChunkRhyolite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkVibrantDunite")
                {
                    return true;
                }

                break;
            }
            case <= 2723944204U:
            {
                if (num != 2277173191U)
                {
                    if (num != 2723944204U)
                    {
                        return false;
                    }

                    if (defName == "ChunkClayStone")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkScoria")
                {
                    return true;
                }

                break;
            }
            case <= 3134642644U when num != 2981327554U:
            {
                if (num != 3027871598U)
                {
                    if (num != 3134642644U)
                    {
                        return false;
                    }

                    if (defName == "ChunkGranite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkGreenGabbro")
                {
                    return true;
                }

                break;
            }
            case <= 3134642644U when defName == "ChunkSerpentinite":
                return true;
            case <= 3134642644U:
                break;
            case <= 3386876289U and <= 3317400192U:
            {
                if (num != 3242717934U)
                {
                    if (num != 3317400192U)
                    {
                        return false;
                    }

                    if (defName == "ChunkPegmatite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkLherzolite")
                {
                    return true;
                }

                break;
            }
            case <= 3386876289U when num != 3349364057U:
            {
                if (num != 3354811171U)
                {
                    if (num != 3386876289U)
                    {
                        return false;
                    }

                    if (defName == "ChunkSandstone")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkMigmatite")
                {
                    return true;
                }

                break;
            }
            case <= 3386876289U when defName == "ChunkCharnockite":
                return true;
            case <= 3386876289U:
                break;
            case <= 3660010543U when num != 3604908289U:
            {
                if (num != 3660010543U)
                {
                    return false;
                }

                if (defName == "ChunkEmperadordark")
                {
                    return true;
                }

                break;
            }
            case <= 3660010543U when defName == "ChunkJaspillite":
                return true;
            case <= 3660010543U:
                break;
            default:
            {
                if (num != 3782560473U)
                {
                    if (num != 4006516401U)
                    {
                        if (num != 4282638488U)
                        {
                            return false;
                        }

                        if (defName == "ChunkLimestone")
                        {
                            return true;
                        }
                    }
                    else if (defName == "ChunkAndesite")
                    {
                        return true;
                    }
                }
                else if (defName == "ChunkThometzekite")
                {
                    return true;
                }

                break;
            }
        }

        return false;
    }
}