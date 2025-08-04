using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MSDrugMix;

public class Building_MSDrugMix : Building
{
    private static readonly string UITexPath = "Things/Building/Misc/MSDrugMixer/UI/";

    [NoTranslate] private readonly string chemicalTexPath = $"{UITexPath}MSDrugsMixerChem_Icon";

    [NoTranslate] private readonly string debugTexPath = $"{UITexPath}MSDrugsMixerDebug_Icon";

    private readonly float effeciencyFactor = 0.95f;

    [NoTranslate] private readonly string EndLimitPath = "Limit_icon";

    [NoTranslate] private readonly string FrontLimitPath = $"{UITexPath}StockLimits/MSDrugMixerStock";

    [NoTranslate] private readonly string produceTexPath = $"{UITexPath}MSDrugsMixerProduce_Icon";

    private List<IntVec3> cachedAdjCellsCardinal;

    private bool debug;

    private bool isProducing;

    private ThingDef MixerThingDef;

    private Sustainer mixSustainer;

    private int NumProd;

    private CompPowerTrader powerComp;

    private int ProdWorkTicks;

    private int StockLimit;

    private int TotalProdWorkTicks;

    private List<IntVec3> AdjCellsCardinalInBounds
    {
        get
        {
            cachedAdjCellsCardinal ??= (from c in GenAdj.CellsAdjacentCardinal(this)
                where c.InBounds(Map)
                select c).ToList();

            return cachedAdjCellsCardinal;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref MixerThingDef, "MixerThingDef");
        Scribe_Values.Look(ref isProducing, "isProducing");
        Scribe_Values.Look(ref NumProd, "NumProd");
        Scribe_Values.Look(ref ProdWorkTicks, "ProdWorkTicks");
        Scribe_Values.Look(ref TotalProdWorkTicks, "TotalProdWorkTicks");
        Scribe_Values.Look(ref StockLimit, "StockLimit");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        powerComp = GetComp<CompPowerTrader>();
        cachedAdjCellsCardinal = AdjCellsCardinalInBounds;
    }

    private void StartMixSustainer()
    {
        var info = SoundInfo.InMap(this, MaintenanceType.PerTick);
        mixSustainer = SoundDef.Named("MSDrugMixer").TrySpawnSustainer(info);
    }

    protected override void Tick()
    {
        base.Tick();
        if (debug && Find.TickManager.TicksGame % 100 == 0)
        {
            var debugMsg = $"At Tick: {Find.TickManager.TicksGame}";
            debugMsg =
                $"{debugMsg} : ({(MixerThingDef != null ? MixerThingDef.defName : "Null")}) : Prod: {(isProducing ? "True" : "false")} : Num: {NumProd} : PWT: {ProdWorkTicks}";
            Log.Message(debugMsg);
        }

        if (!IsWorking(this) || MixerThingDef == null || StockLimitReached(this, MixerThingDef, StockLimit, out _))
        {
            return;
        }

        if (ProdWorkTicks > 0 && isProducing)
        {
            ProdWorkTicks--;
            if (mixSustainer == null || mixSustainer.Ended)
            {
                StartMixSustainer();
                return;
            }

            mixSustainer.Maintain();
        }
        else if (isProducing && NumProd > 0 && MixerThingDef != null)
        {
            if (debug)
            {
                Log.Message($"Production point: {MixerThingDef.defName} : {ProdWorkTicks}");
            }

            if (ValidateOutput(MixerThingDef, out var hasSpace, out var candidatesOut) && hasSpace > 0)
            {
                if (hasSpace >= NumProd)
                {
                    if (debug)
                    {
                        Log.Message($"Ejecting: {MixerThingDef.defName} : {NumProd}");
                    }

                    MixerEject(MixerThingDef, NumProd, candidatesOut, out var Surplus);
                    NumProd = Surplus;
                }
                else
                {
                    if (debug)
                    {
                        Log.Message($"Ejecting: {MixerThingDef.defName} : {hasSpace}");
                    }

                    MixerEject(MixerThingDef, hasSpace, candidatesOut, out var Surplus2);
                    NumProd -= hasSpace - Surplus2;
                }
            }

            if (NumProd == 0)
            {
                TotalProdWorkTicks = 0;
            }
        }
        else if (isProducing && MixerThingDef != null && ValidateRecipe(MixerThingDef, out var UseMax,
                     out var RecipeList, out var minProd, out var maxProd, out var ticks))
        {
            if (debug)
            {
                Log.Message($"StartProduction: {MixerThingDef.defName} :  RCP Items: {RecipeList.Count}");
            }

            if (RecipeList.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < RecipeList.Count; i++)
            {
                var recipeThingDef = RecipeList[i].def;
                var num = UseMax ? RecipeList[i].Max : RecipeList[i].Min;

                if (debug)
                {
                    Log.Message($"Removing: {(UseMax ? "Max" : "Min")}: {num} ({recipeThingDef.defName})");
                }

                RemoveRecipeItems(recipeThingDef, num);
            }

            NumProd = minProd;
            if (UseMax)
            {
                NumProd = maxProd;
            }

            ProdWorkTicks = (int)(ticks * effeciencyFactor * NumProd);
            TotalProdWorkTicks = ProdWorkTicks;
        }
    }


    private void MixerEject(ThingDef t, int numProducts, List<Building> candidatesout, out int remaining)
    {
        remaining = numProducts;
        if (candidatesout.Count <= 0)
        {
            return;
        }

        for (var i = 0; i < candidatesout.Count; i++)
        {
            if (i == 0)
            {
                _ = candidatesout[i];
            }

            if (numProducts <= 0)
            {
                continue;
            }

            var thingList = candidatesout[i].Position.GetThingList(candidatesout[i].Map);
            if (thingList.Count <= 0)
            {
                continue;
            }

            var founditem = false;
            var blocked = false;
            foreach (var thing in thingList)
            {
                if (thing.def == t)
                {
                    founditem = true;
                    var canPlace = thing.def.stackLimit - thing.stackCount;
                    if (canPlace <= 0)
                    {
                        continue;
                    }

                    if (canPlace >= numProducts)
                    {
                        thing.stackCount += numProducts;
                        remaining -= numProducts;
                        numProducts = 0;
                    }
                    else
                    {
                        thing.stackCount += canPlace;
                        numProducts -= canPlace;
                        remaining -= canPlace;
                    }
                }
                else if (thing is not Building)
                {
                    blocked = true;
                }
            }

            if (founditem || blocked)
            {
                continue;
            }

            {
                var canPlace = t.stackLimit;
                var newProduct = ThingMaker.MakeThing(t);
                if (!candidatesout[i].Position.IsValidStorageFor(candidatesout[i].Map, newProduct))
                {
                    continue;
                }

                if (canPlace >= numProducts)
                {
                    newProduct.stackCount = numProducts;
                    remaining -= numProducts;
                    numProducts = 0;
                }
                else
                {
                    newProduct.stackCount = canPlace;
                    numProducts -= canPlace;
                    remaining -= canPlace;
                }

                GenPlace.TryPlaceThing(newProduct, candidatesout[i].Position, candidatesout[i].Map,
                    ThingPlaceMode.Direct, out _);
            }
        }
    }

    private void RemoveRecipeItems(ThingDef t, int numToRemove)
    {
        var AdjCells = AdjCellsCardinalInBounds;
        if (AdjCells.Count <= 0)
        {
            return;
        }

        var TotalRemoved = 0;
        for (var i = 0; i < AdjCells.Count; i++)
        {
            if (numToRemove <= 0)
            {
                continue;
            }

            var isInputCell = false;
            var has = 0;
            var candidates = new List<Thing>();
            var thingList = AdjCells[i].GetThingList(Map);
            if (thingList.Count > 0)
            {
                foreach (var thing in thingList)
                {
                    if (thing.def == t)
                    {
                        has += thing.stackCount;
                        candidates.Add(thing);
                    }

                    if (thing is Building && thing.def?.defName == "MSDrugMixInput")
                    {
                        isInputCell = true;
                    }
                }
            }

            if (!isInputCell || has <= 0 || candidates.Count <= 0)
            {
                continue;
            }

            foreach (var thing in candidates)
            {
                if (thing.def != t)
                {
                    continue;
                }

                if (numToRemove - thing.stackCount >= 0)
                {
                    numToRemove -= thing.stackCount;
                    TotalRemoved += thing.stackCount;
                    thing.Destroy();
                }
                else
                {
                    thing.stackCount -= numToRemove;
                    TotalRemoved += numToRemove;
                    numToRemove = 0;
                }
            }
        }

        if (debug)
        {
            Log.Message($"Total Removed: ({t.defName}) = {TotalRemoved}");
        }
    }

    private bool ValidateOutput(ThingDef t, out int hasSpace, out List<Building> candidatesOut)
    {
        hasSpace = 0;
        candidatesOut = [];
        var AdjCells = AdjCellsCardinalInBounds;
        if (AdjCells.Count > 0)
        {
            for (var i = 0; i < AdjCells.Count; i++)
            {
                var isOutputCell = false;
                var has = 0;
                var thingList = AdjCells[i].GetThingList(Map);
                if (thingList.Count > 0)
                {
                    foreach (var thing in thingList)
                    {
                        if (thing.def == t)
                        {
                            has += thing.stackCount;
                        }

                        if (thing is not Building building || building.def.defName != "MSDrugMixOutput")
                        {
                            continue;
                        }

                        isOutputCell = true;
                        hasSpace += t.stackLimit;
                        candidatesOut.Add(building);
                    }
                }

                if (!isOutputCell)
                {
                    continue;
                }

                hasSpace -= has;
            }
        }

        if (debug)
        {
            Log.Message($"{hasSpace} item space on {candidatesOut.Count} points");
        }

        return hasSpace > 0;
    }

    private bool ValidateRecipe(ThingDef t, out bool CanUseMax, out List<RCPItemCanUse> FinalList, out int MinProd,
        out int MaxProd, out int Ticks)
    {
        CanUseMax = true;
        FinalList = null;
        MinProd = 0;
        MaxProd = 0;
        Ticks = 0;
        if (debug && Find.TickManager.TicksGame % 100 == 0)
        {
            Log.Message($"ValRep: {t.defName}");
        }

        if (!MSDrugMixUtility.RCPProdValues(t, out var ticks, out var minProd, out var maxProd, out var Res))
        {
            return false;
        }

        Ticks = ticks;
        MinProd = minProd;
        MaxProd = maxProd;
        if (debug)
        {
            Log.Message($"RCPVals: Ticks: {ticks} minProd: {minProd} maxProd: {maxProd} Res: {Res}");
        }

        if (!ResearchProjectDef.Named(Res).IsFinished || minProd <= 0 || maxProd <= 0 || ticks <= 0)
        {
            if (!ResearchProjectDef.Named(Res).IsFinished)
            {
                Log.Message("MSDrugMix.ErrorRes".Translate(MixerThingDef.label));
                isProducing = false;
                NumProd = 0;
                ProdWorkTicks = 0;
                TotalProdWorkTicks = 0;
            }
            else
            {
                Log.Message("MSDrugMix.ErrorRCP".Translate(MixerThingDef.label, ticks.ToString(),
                    minProd.ToString(), maxProd.ToString()));
                isProducing = false;
                NumProd = 0;
                ProdWorkTicks = 0;
                TotalProdWorkTicks = 0;
            }

            return false;
        }

        var listRCP = MSDrugMixUtility.GetRCPList(t);
        if (listRCP.Count <= 0)
        {
            if (debug)
            {
                Log.Message("RCP is False.");
            }

            return false;
        }

        if (debug)
        {
            Log.Message($"RCP Listings: {listRCP.Count}");
        }

        var RCPListPotentials = new List<RCPItemCanUse>();
        var RCPGroups = new List<int>();
        foreach (var listItem in listRCP)
        {
            var MaterialsMin = 0;
            var MaterialsMax = 0;
            var RCPMinNumNeeded = (int)Math.Round(listItem.num * minProd * listItem.ratio);
            var RCPMaxNumNeeded = (int)Math.Round(listItem.num * maxProd * listItem.ratio);
            if (HasEnoughMaterialInHoppers(listItem.def, RCPMinNumNeeded, true))
            {
                MaterialsMin = RCPMinNumNeeded;
            }

            if (HasEnoughMaterialInHoppers(listItem.def, RCPMaxNumNeeded, false))
            {
                MaterialsMax = RCPMaxNumNeeded;
            }

            if (MaterialsMin > 0 || MaterialsMax > 0)
            {
                RCPListPotentials.Add(new RCPItemCanUse
                {
                    def = listItem.def,
                    Min = MaterialsMin,
                    Max = MaterialsMax,
                    Grp = listItem.mixgrp
                });
            }

            if (!RCPGroups.Contains(listItem.mixgrp))
            {
                RCPGroups.Add(listItem.mixgrp);
            }
        }

        if (debug)
        {
            Log.Message(
                $"InnerRecipe List: Groups: {RCPGroups.Count} , Potentials: {RCPListPotentials.Count}");
        }

        FinalList = [];
        var NotAllGroups = false;
        if (RCPGroups.Count > 0)
        {
            foreach (var grp in RCPGroups)
            {
                var foundGroup = false;
                if (RCPListPotentials.Count > 0)
                {
                    RCPItemCanUse bestthingsofar = default;
                    var best = false;
                    var bestmax = false;
                    foreach (var itemchk in RCPListPotentials)
                    {
                        if (itemchk.Grp != grp)
                        {
                            continue;
                        }

                        foundGroup = true;
                        if (itemchk.Min <= 0)
                        {
                            continue;
                        }

                        if (itemchk.Max > 0)
                        {
                            if (bestmax)
                            {
                                continue;
                            }

                            bestthingsofar.def = itemchk.def;
                            bestthingsofar.Min = itemchk.Min;
                            bestthingsofar.Max = itemchk.Max;
                            bestthingsofar.Grp = itemchk.Grp;
                            best = true;
                            bestmax = true;
                        }
                        else if (!best)
                        {
                            bestthingsofar.def = itemchk.def;
                            bestthingsofar.Min = itemchk.Min;
                            bestthingsofar.Max = itemchk.Max;
                            bestthingsofar.Grp = itemchk.Grp;
                            best = true;
                        }
                    }

                    if (!bestmax)
                    {
                        bestthingsofar.Max = 0;
                    }

                    FinalList.Add(bestthingsofar);
                }

                if (foundGroup)
                {
                    continue;
                }

                NotAllGroups = true;
                DoNotFoundGroupsOverlay(this, t, grp);
            }
        }

        if (FinalList.Count > 0)
        {
            for (var l = 0; l < FinalList.Count; l++)
            {
                if (FinalList[l].Max == 0)
                {
                    CanUseMax = false;
                }
            }
        }

        if (NotAllGroups)
        {
            if (debug)
            {
                Log.Message("RCP is False. Not all inputs found");
            }

            return false;
        }

        if (debug)
        {
            Log.Message($"RCP is True. with ({FinalList.Count}) final list items");
        }

        return true;
    }

    private static void DoNotFoundGroupsOverlay(Building_MSDrugMix b, ThingDef def, int grp)
    {
        if (Find.CurrentMap == null || Find.CurrentMap != b.Map)
        {
            return;
        }

        var listRCP = MSDrugMixUtility.GetRCPList(def);
        var alerts = new List<ThingDef>();
        if (listRCP.Count > 0)
        {
            foreach (var item in listRCP)
            {
                if (item.mixgrp == grp)
                {
                    alerts.AddDistinct(item.def);
                }
            }
        }

        if (alerts.Count <= 0)
        {
            return;
        }

        var OutOfFuelMat = MaterialPool.MatFrom("UI/Overlays/OutOfFuel", ShaderDatabase.MetaOverlay);
        var i = 0;
        foreach (var alert in alerts)
        {
            if (!alert.defName.StartsWith("Chunk") || alert.defName.StartsWith("Chunk") && i < 1)
            {
                var mat = MaterialPool.MatFrom(alert.uiIcon, ShaderDatabase.MetaOverlay, Color.white);
                var BaseAlt = AltitudeLayer.WorldClipper.AltitudeFor();
                if (mat != null)
                {
                    var altInd = 21;
                    var plane = MeshPool.plane08;
                    var drawPos = b.TrueCenter();
                    drawPos.y = BaseAlt + (0.046875f * altInd);
                    drawPos.x += i;
                    drawPos.z += grp - 2;
                    var num2 = ((float)Math.Sin(
                        (Time.realtimeSinceStartup + (397f * (b.thingIDNumber % 571))) * 4f) + 1f) * 0.5f;
                    num2 = 0.3f + (num2 * 0.7f);
                    for (var j = 0; j < 2; j++)
                    {
                        var material = FadedMaterialPool.FadedVersionOf(j < 1 ? mat : OutOfFuelMat, num2);

                        if (material != null)
                        {
                            Graphics.DrawMesh(plane, drawPos, Quaternion.identity, material, 0);
                        }
                    }
                }
            }

            i++;
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (Faction != Faction.OfPlayer)
        {
            yield break;
        }

        string SelectDesc = "MSDrugMix.ChemSelectDesc".Translate();
        if (MixerThingDef == null)
        {
            string NoChem = "MSDrugMix.ChemSelect".Translate();
            yield return new Command_Action
            {
                defaultLabel = NoChem,
                icon = ContentFinder<Texture2D>.Get(chemicalTexPath),
                defaultDesc = SelectDesc,
                action = MSMixerSelectChem
            };
        }
        else
        {
            var IconToUse = MSDrugMixUtility.GetMSMixIcon(MixerThingDef);
            var LabelDetail = MixerThingDef.label.CapitalizeFirst();
            LabelDetail = $"{LabelDetail} [{NumProd}] ";
            if (TotalProdWorkTicks > 0)
            {
                LabelDetail =
                    $"{LabelDetail} ({(int)((TotalProdWorkTicks - ProdWorkTicks) / (float)TotalProdWorkTicks * 100f)}%)";
            }

            yield return new Command_Action
            {
                defaultLabel = LabelDetail,
                icon = IconToUse,
                defaultDesc = SelectDesc,
                action = MSMixerSelectChem
            };
        }

        string LabelProduce = "MSDrugMix.Production".Translate();
        string LabelProduceDesc = "MSDrugMix.ProductionDesc".Translate();
        if (isProducing)
        {
            if (MixerThingDef != null)
            {
                if (MSDrugMixUtility.RCPProdValues(MixerThingDef, out _, out var minProd,
                        out var maxProd, out _))
                {
                    LabelProduce +=
                        "MSDrugMix.ProdLabelRange".Translate(minProd.ToString(), maxProd.ToString());
                }
                else
                {
                    LabelProduce += "MSDrugMix.ProdLabelERR".Translate();
                }
            }
            else
            {
                LabelProduce += "MSDrugMix.ProdNoChem".Translate();
            }
        }
        else
        {
            LabelProduce += "MSDrugMix.ProdStopped".Translate();
        }

        yield return new Command_Toggle
        {
            icon = ContentFinder<Texture2D>.Get(produceTexPath),
            defaultLabel = LabelProduce,
            defaultDesc = LabelProduceDesc,
            isActive = () => isProducing,
            toggleAction = delegate { ToggleProducing(isProducing); }
        };
        var LimitTexPath = FrontLimitPath;
        string LimitLabelDetail;
        if (StockLimit > 0)
        {
            StockLimitReached(this, MixerThingDef, StockLimit, out var ActualStockNum);
            var LimitPct = ActualStockNum * 100 / StockLimit;
            LimitLabelDetail = "MSDrugMix.StockLabel".Translate(StockLimit.ToString(), LimitPct.ToString());
            LimitTexPath += StockLimit.ToString();
        }
        else
        {
            LimitLabelDetail = "MSDrugMix.StockLabelNL".Translate();
            LimitTexPath += "No";
        }

        LimitTexPath += EndLimitPath;
        var LimitIconToUse = ContentFinder<Texture2D>.Get(LimitTexPath);
        string SelectLimit = "MSDrugMix.SelectStockLimit".Translate();
        yield return new Command_Action
        {
            defaultLabel = LimitLabelDetail,
            icon = LimitIconToUse,
            defaultDesc = SelectLimit,
            action = MSMixerSelectLimit
        };
        if (Prefs.DevMode)
        {
            yield return new Command_Toggle
            {
                icon = ContentFinder<Texture2D>.Get(debugTexPath),
                defaultLabel = "Debug Mode",
                defaultDesc = "Send debug messages to Log",
                isActive = () => debug,
                toggleAction = delegate { ToggleDebug(debug); }
            };
        }
    }

    public void ToggleDebug(bool flag)
    {
        debug = !flag;
    }

    public void ToggleProducing(bool flag)
    {
        isProducing = !flag;
    }

    public void MSMixerSelectLimit()
    {
        var list = new List<FloatMenuOption>();
        var Choices = MSDrugMixUtility.GetMaxStock();
        if (Choices.Count > 0)
        {
            foreach (var i in Choices)
            {
                string text;
                if (i > 0)
                {
                    text = i.ToString();
                }
                else
                {
                    text = "MSDrugMix.StockNoLimit".Translate();
                }

                var value = i;
                list.Add(new FloatMenuOption(text, delegate { SetStockLimits(value); }, MenuOptionPriority.Default,
                    null, null, 29f));
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }

    public void MSMixerSelectChem()
    {
        var list = new List<FloatMenuOption>();
        string text = "MSDrugsMix.SelNoChemical".Translate();
        list.Add(new FloatMenuOption(text, delegate { SetProdControlValues(null, false, 0, 0); },
            MenuOptionPriority.Default, null, null, 29f));
        var choices = MSDrugMixUtility.GetMixList();
        var choicesDefs = new List<ThingDef>();
        choices.ForEach(s => choicesDefs.Add(DefDatabase<ThingDef>.GetNamedSilentFail(s)));
        if (choices.Count > 0)
        {
            foreach (var thingDef in choicesDefs.OrderBy(thingDef => thingDef.label))
            {
                text = thingDef.label.CapitalizeFirst();
                if (IsChemAvailable(thingDef))
                {
                    list.Add(new FloatMenuOption(text,
                        delegate { SetProdControlValues(thingDef, true, 0, 0); }, MenuOptionPriority.Default,
                        null, null, 29f,
                        rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f),
                            thingDef)));
                }
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }

    public void SetStockLimits(int aStockLim)
    {
        StockLimit = aStockLim;
    }

    public void SetProdControlValues(ThingDef tdef, bool prod, int num, int ticks)
    {
        if (tdef == null)
        {
            MixerThingDef = null;
            isProducing = false;
            NumProd = 0;
            ProdWorkTicks = 0;
            TotalProdWorkTicks = 0;
            return;
        }

        if (MixerThingDef == tdef)
        {
            return;
        }

        MixerThingDef = tdef;
        NumProd = 0;
        ProdWorkTicks = 0;
        TotalProdWorkTicks = 0;
    }

    private bool IsWorking(Building b)
    {
        return !b.IsBrokenDown() && powerComp.PowerOn;
    }

    private static bool IsChemAvailable(ThingDef chkchemDef)
    {
        return MSDrugMixUtility.RCPProdValues(chkchemDef, out _, out _, out _, out var research) &&
               research != "" && DefDatabase<ResearchProjectDef>.GetNamed(research, false).IsFinished;
    }

    private static bool StockLimitReached(Building b, ThingDef stockThing, int stockLim, out int ActualStockNum)
    {
        ActualStockNum = 0;
        if (stockLim <= 0 || stockThing == null)
        {
            return false;
        }

        var StockListing = b.Map.listerThings.ThingsOfDef(stockThing);
        if (StockListing.Count <= 0)
        {
            return ActualStockNum >= stockLim;
        }

        foreach (var thing in StockListing)
        {
            ActualStockNum += thing.stackCount;
        }

        return ActualStockNum >= stockLim;
    }

    protected virtual bool HasEnoughMaterialInHoppers(ThingDef NeededThing, int required, bool isMin)
    {
        var num = 0;
        foreach (var c in AdjCellsCardinalInBounds)
        {
            Thing thingNeed = null;
            Thing thingHopper = null;
            var thingList = c.GetThingList(Map);
            foreach (var thing3 in thingList)
            {
                if (thing3.def == NeededThing)
                {
                    thingNeed = thing3;
                }

                if (thing3.def.defName == "MSDrugMixInput")
                {
                    thingHopper = thing3;
                }
            }

            if (thingNeed != null && thingHopper != null)
            {
                num += thingNeed.stackCount;
            }
        }

        if (debug)
        {
            Log.Message(
                $"Enough Materials? ({(num >= required ? "Yes" : "No")}): ({NeededThing.defName}) Found:{num} for {required} required as {(isMin ? "Min" : "Max")}");
        }

        return num >= required;
    }

    private struct RCPItemCanUse
    {
        public ThingDef def;

        public int Min;

        public int Max;

        public int Grp;
    }
}