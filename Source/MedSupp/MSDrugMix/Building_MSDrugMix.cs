using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MSDrugMix
{
	// Token: 0x02000020 RID: 32
	public class Building_MSDrugMix : Building
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00006BB2 File Offset: 0x00004DB2
		private List<IntVec3> AdjCellsCardinalInBounds
		{
			get
			{
				if (this.cachedAdjCellsCardinal == null)
				{
					this.cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(this)
					where c.InBounds(base.Map)
					select c).ToList();
				}
				return this.cachedAdjCellsCardinal;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006BE4 File Offset: 0x00004DE4
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref this.MixerThingDef, "MixerThingDef");
			Scribe_Values.Look(ref this.isProducing, "isProducing", false, false);
			Scribe_Values.Look(ref this.NumProd, "NumProd", 0, false);
			Scribe_Values.Look(ref this.ProdWorkTicks, "ProdWorkTicks", 0, false);
			Scribe_Values.Look(ref this.TotalProdWorkTicks, "TotalProdWorkTicks", 0, false);
			Scribe_Values.Look(ref this.StockLimit, "StockLimit", 0, false);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00006C61 File Offset: 0x00004E61
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.cachedAdjCellsCardinal = this.AdjCellsCardinalInBounds;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00006C84 File Offset: 0x00004E84
		public void StartMixSustainer()
		{
			SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
			this.mixSustainer = SoundDef.Named("MSDrugMixer").TrySpawnSustainer(info);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00006CB4 File Offset: 0x00004EB4
		public override void Tick()
		{
			base.Tick();
			if (this.debug && Find.TickManager.TicksGame % 100 == 0)
			{
				string debugMsg = "At Tick: " + Find.TickManager.TicksGame;
				debugMsg = string.Concat(new string[]
				{
					debugMsg,
					" : (",
					(this.MixerThingDef != null) ? this.MixerThingDef.defName : "Null",
					") : Prod: ",
					this.isProducing ? "True" : "false",
					" : Num: ",
					this.NumProd.ToString(),
					" : PWT: ",
					this.ProdWorkTicks.ToString()
				});
				Log.Message(debugMsg, false);
			}

            if (this.IsWorking(this) && this.MixerThingDef != null && !Building_MSDrugMix.StockLimitReached(this, this.MixerThingDef, this.StockLimit, out _))
            {
                if (this.ProdWorkTicks > 0 && this.isProducing)
                {
                    this.ProdWorkTicks--;
                    if (this.mixSustainer == null)
                    {
                        this.StartMixSustainer();
                        return;
                    }
                    if (this.mixSustainer.Ended)
                    {
                        this.StartMixSustainer();
                        return;
                    }
                    this.mixSustainer.Maintain();
                    return;
                }
                else if (this.isProducing && this.NumProd > 0 && this.MixerThingDef != null)
                {
                    if (this.debug)
                    {
                        Log.Message("Production point: " + this.MixerThingDef.defName + " : " + this.ProdWorkTicks.ToString(), false);
                    }
                    if (this.ValidateOutput(this.MixerThingDef, out int hasSpace, out List<Building> candidatesOut) && hasSpace > 0)
                    {
                        if (hasSpace >= this.NumProd)
                        {
                            if (this.debug)
                            {
                                Log.Message("Ejecting: " + this.MixerThingDef.defName + " : " + this.NumProd.ToString(), false);
                            }
                            this.MixerEject(this, this.MixerThingDef, this.NumProd, candidatesOut, out int Surplus);
                            this.NumProd = Surplus;
                        }
                        else
                        {
                            if (this.debug)
                            {
                                Log.Message("Ejecting: " + this.MixerThingDef.defName + " : " + hasSpace.ToString(), false);
                            }
                            this.MixerEject(this, this.MixerThingDef, hasSpace, candidatesOut, out int Surplus2);
                            this.NumProd -= hasSpace - Surplus2;
                        }
                    }
                    if (this.NumProd == 0)
                    {
                        this.TotalProdWorkTicks = 0;
                        return;
                    }
                }
                else if (this.isProducing && this.MixerThingDef != null && this.ValidateRecipe(this.MixerThingDef, out bool UseMax, out List<RCPItemCanUse> RecipeList, out int minProd, out int maxProd, out int ticks))
                {
                    if (this.debug)
                    {
                        Log.Message(string.Concat(new object[]
                        {
                            "StartProduction: ",
                            this.MixerThingDef.defName,
                            " :  RCP Items: ",
                            RecipeList.Count
                        }), false);
                    }
                    if (RecipeList.Count > 0)
                    {
                        for (int i = 0; i < RecipeList.Count; i++)
                        {
                            ThingDef recipeThingDef = RecipeList[i].def;
                            int num;
                            if (UseMax)
                            {
                                num = RecipeList[i].Max;
                            }
                            else
                            {
                                num = RecipeList[i].Min;
                            }
                            if (this.debug)
                            {
                                Log.Message(string.Concat(new string[]
                                {
                                    "Removing: ",
                                    UseMax ? "Max" : "Min",
                                    ": ",
                                    num.ToString(),
                                    " (",
                                    recipeThingDef.defName,
                                    ")"
                                }), false);
                            }
                            this.RemoveRecipeItems(recipeThingDef, num);
                        }
                        this.NumProd = minProd;
                        if (UseMax)
                        {
                            this.NumProd = maxProd;
                        }
                        this.ProdWorkTicks = (int)((float)ticks * this.effeciencyFactor * (float)this.NumProd);
                        this.TotalProdWorkTicks = this.ProdWorkTicks;
                    }
                }
            }
        }

		// Token: 0x06000089 RID: 137 RVA: 0x000070B1 File Offset: 0x000052B1
		public override void TickRare()
		{
			base.TickRare();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000070BC File Offset: 0x000052BC
		public void MixerEject(Building b, ThingDef t, int numProducts, List<Building> candidatesout, out int remaining)
		{
			remaining = numProducts;
			if (candidatesout.Count > 0)
			{
				for (int i = 0; i < candidatesout.Count; i++)
				{
					if (i == 0)
					{
                        Building building;
                        _ = candidatesout[i];
                    }
					if (numProducts > 0)
					{
						List<Thing> thingList = candidatesout[i].Position.GetThingList(candidatesout[i].Map);
						if (thingList.Count > 0)
						{
							bool founditem = false;
							bool blocked = false;
							for (int j = 0; j < thingList.Count; j++)
							{
								if (thingList[j].def == t)
								{
									founditem = true;
									int canPlace = thingList[j].def.stackLimit - thingList[j].stackCount;
									if (canPlace > 0)
									{
										if (canPlace >= numProducts)
										{
											thingList[j].stackCount += numProducts;
											remaining -= numProducts;
											numProducts = 0;
										}
										else
										{
											thingList[j].stackCount += canPlace;
											numProducts -= canPlace;
											remaining -= canPlace;
										}
									}
								}
								else if (thingList[j] != null && !(thingList[j] is Building))
								{
									blocked = true;
								}
							}
							if (!founditem && !blocked)
							{
								int canPlace = t.stackLimit;
								Thing newProduct = ThingMaker.MakeThing(t, null);
								if (candidatesout[i].Position.IsValidStorageFor(candidatesout[i].Map, newProduct))
								{
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
                                    GenPlace.TryPlaceThing(newProduct, candidatesout[i].Position, candidatesout[i].Map, ThingPlaceMode.Direct, out Thing newProductThing, null, null, default);
                                }
							}
						}
					}
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007290 File Offset: 0x00005490
		public void RemoveRecipeItems(ThingDef t, int numToRemove)
		{
			List<IntVec3> AdjCells = this.AdjCellsCardinalInBounds;
			if (AdjCells.Count > 0)
			{
				int TotalRemoved = 0;
				for (int i = 0; i < AdjCells.Count; i++)
				{
					if (numToRemove > 0)
					{
						bool isInputCell = false;
						int has = 0;
						List<Thing> candidates = new List<Thing>();
						List<Thing> thingList = AdjCells[i].GetThingList(base.Map);
						if (thingList.Count > 0)
						{
							for (int j = 0; j < thingList.Count; j++)
							{
								if (thingList[j].def == t)
								{
									has += thingList[j].stackCount;
									candidates.Add(thingList[j]);
								}
								if (thingList[j] is Building && thingList[j].def.defName == "MSDrugMixInput")
								{
									isInputCell = true;
								}
							}
						}
						if (isInputCell && has > 0 && candidates.Count > 0)
						{
							for (int k = 0; k < candidates.Count; k++)
							{
								if (candidates[k].def == t)
								{
									if (numToRemove - candidates[k].stackCount >= 0)
									{
										numToRemove -= candidates[k].stackCount;
										TotalRemoved += candidates[k].stackCount;
										candidates[k].Destroy(DestroyMode.Vanish);
									}
									else
									{
										candidates[k].stackCount -= numToRemove;
										TotalRemoved += numToRemove;
										numToRemove = 0;
									}
								}
							}
						}
					}
				}
				if (this.debug)
				{
					Log.Message("Total Removed: (" + t.defName + ") = " + TotalRemoved.ToString(), false);
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007448 File Offset: 0x00005648
		public bool ValidateOutput(ThingDef t, out int hasSpace, out List<Building> candidatesOut)
		{
			hasSpace = 0;
			int numSpaces = 0;
			candidatesOut = new List<Building>();
			int hasProduct = 0;
			List<IntVec3> AdjCells = this.AdjCellsCardinalInBounds;
			if (AdjCells.Count > 0)
			{
				for (int i = 0; i < AdjCells.Count; i++)
				{
					bool isOutputCell = false;
					int has = 0;
					List<Thing> thingList = AdjCells[i].GetThingList(base.Map);
					if (thingList.Count > 0)
					{
						for (int j = 0; j < thingList.Count; j++)
						{
							if (thingList[j].def == t)
							{
								has += thingList[j].stackCount;
							}
							if (thingList[j] is Building && thingList[j].def.defName == "MSDrugMixOutput")
							{
								isOutputCell = true;
								numSpaces++;
								hasSpace += t.stackLimit;
								candidatesOut.Add(thingList[j] as Building);
							}
						}
					}
					if (isOutputCell)
					{
						hasProduct += has;
						hasSpace -= has;
					}
				}
			}
			if (this.debug)
			{
				Log.Message(hasSpace.ToString() + " item space on " + candidatesOut.Count.ToString() + " points", false);
			}
			return hasSpace > 0;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00007594 File Offset: 0x00005794
		public bool ValidateRecipe(ThingDef t, out bool CanUseMax, out List<RCPItemCanUse> FinalList, out int MinProd, out int MaxProd, out int Ticks)
		{
			CanUseMax = true;
			FinalList = null;
			MinProd = 0;
			MaxProd = 0;
			Ticks = 0;
			if (this.debug && Find.TickManager.TicksGame % 100 == 0)
			{
				Log.Message("ValRep: " + t.defName, false);
			}
            if (!MSDrugMixUtility.RCPProdValues(t, out int ticks, out int minProd, out int maxProd, out string Res))
            {
                return false;
            }
            Ticks = ticks;
			MinProd = minProd;
			MaxProd = maxProd;
			if (this.debug)
			{
				Log.Message(string.Concat(new string[]
				{
					"RCPVals: Ticks: ",
					ticks.ToString(),
					" minProd: ",
					minProd.ToString(),
					" maxProd: ",
					maxProd.ToString(),
					" Res: ",
					Res
				}), false);
			}
			if (!ResearchProjectDef.Named(Res).IsFinished || minProd <= 0 || maxProd <= 0 || ticks <= 0)
			{
				if (!ResearchProjectDef.Named(Res).IsFinished)
				{
					Log.Message("MSDrugMix.ErrorRes".Translate(this.MixerThingDef.label), false);
					this.isProducing = false;
					this.NumProd = 0;
					this.ProdWorkTicks = 0;
					this.TotalProdWorkTicks = 0;
				}
				else
				{
					Log.Message("MSDrugMix.ErrorRCP".Translate(this.MixerThingDef.label, ticks.ToString(), minProd.ToString(), maxProd.ToString()), false);
					this.isProducing = false;
					this.NumProd = 0;
					this.ProdWorkTicks = 0;
					this.TotalProdWorkTicks = 0;
				}
				return false;
			}
			List<MSDrugMixUtility.MSRCPListItem> listRCP = MSDrugMixUtility.GetRCPList(t);
			if (listRCP.Count <= 0)
			{
				if (this.debug)
				{
					Log.Message("RCP is False.", false);
				}
				return false;
			}
			if (this.debug)
			{
				Log.Message("RCP Listings: " + listRCP.Count.ToString(), false);
			}
			List<RCPItemCanUse> RCPListPotentials = new List<RCPItemCanUse>();
			List<int> RCPGroups = new List<int>();
			for (int i = 0; i < listRCP.Count; i++)
			{
				int MaterialsMin = 0;
				int MaterialsMax = 0;
				MSDrugMixUtility.MSRCPListItem RCPItem = listRCP[i];
				int RCPMinNumNeeded = (int)Math.Round((double)((float)(RCPItem.num * minProd) * RCPItem.ratio));
				int RCPMaxNumNeeded = (int)Math.Round((double)((float)(RCPItem.num * maxProd) * RCPItem.ratio));
				if (this.HasEnoughMaterialInHoppers(RCPItem.def, RCPMinNumNeeded, true))
				{
					MaterialsMin = RCPMinNumNeeded;
				}
				if (this.HasEnoughMaterialInHoppers(RCPItem.def, RCPMaxNumNeeded, false))
				{
					MaterialsMax = RCPMaxNumNeeded;
				}
				if (MaterialsMin > 0 || MaterialsMax > 0)
				{
					RCPListPotentials.Add(new RCPItemCanUse
                    {
						def = RCPItem.def,
						Min = MaterialsMin,
						Max = MaterialsMax,
						Grp = RCPItem.mixgrp
					});
				}
				if (!RCPGroups.Contains(RCPItem.mixgrp))
				{
					RCPGroups.Add(RCPItem.mixgrp);
				}
			}
			if (this.debug)
			{
				Log.Message("InnerRecipe List: Groups: " + RCPGroups.Count.ToString() + " , Potentials: " + RCPListPotentials.Count.ToString(), false);
			}
			FinalList = new List<RCPItemCanUse>();
			bool NotAllGroups = false;
			if (RCPGroups.Count > 0)
			{
				for (int j = 0; j < RCPGroups.Count; j++)
				{
					bool foundGroup = false;
					if (RCPListPotentials.Count > 0)
					{
                        RCPItemCanUse bestthingsofar = default;
						bool best = false;
						bool bestmax = false;
						for (int k = 0; k < RCPListPotentials.Count; k++)
						{
                            RCPItemCanUse itemchk = RCPListPotentials[k];
							if (itemchk.Grp == RCPGroups[j])
							{
								foundGroup = true;
								if (itemchk.Min > 0)
								{
									if (itemchk.Max > 0)
									{
										if (!bestmax)
										{
											bestthingsofar.def = itemchk.def;
											bestthingsofar.Min = itemchk.Min;
											bestthingsofar.Max = itemchk.Max;
											bestthingsofar.Grp = itemchk.Grp;
											best = true;
											bestmax = true;
										}
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
							}
						}
						if (!bestmax)
						{
							bestthingsofar.Max = 0;
						}
						FinalList.Add(bestthingsofar);
					}
					if (!foundGroup)
					{
						NotAllGroups = true;
						Building_MSDrugMix.DoNotFoundGroupsOverlay(this, t, RCPGroups[j]);
					}
				}
			}
			if (FinalList.Count > 0)
			{
				for (int l = 0; l < FinalList.Count; l++)
				{
					if (FinalList[l].Max == 0)
					{
						CanUseMax = false;
					}
				}
			}
			if (NotAllGroups)
			{
				if (this.debug)
				{
					Log.Message("RCP is False. Not all inputs found", false);
				}
				return false;
			}
			if (this.debug)
			{
				Log.Message("RCP is True. with (" + FinalList.Count.ToString() + ") final list items", false);
			}
			return true;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00007A9C File Offset: 0x00005C9C
		public static void DoNotFoundGroupsOverlay(Building_MSDrugMix b, ThingDef def, int grp)
		{
			if (Find.CurrentMap != null && Find.CurrentMap == b.Map)
			{
				List<MSDrugMixUtility.MSRCPListItem> listRCP = MSDrugMixUtility.GetRCPList(def);
				List<ThingDef> alerts = new List<ThingDef>();
				if (listRCP.Count > 0)
				{
					foreach (MSDrugMixUtility.MSRCPListItem item in listRCP)
					{
						if (item.mixgrp == grp)
						{
							alerts.AddDistinct(item.def);
						}
					}
				}
				if (alerts.Count > 0)
				{
					Material OutOfFuelMat = MaterialPool.MatFrom("UI/Overlays/OutOfFuel", ShaderDatabase.MetaOverlay);
					int i = 0;
					foreach (ThingDef alert in alerts)
					{
						if (!alert.defName.StartsWith("Chunk") || (alert.defName.StartsWith("Chunk") && i < 1))
						{
							Material mat = MaterialPool.MatFrom(alert.uiIcon, ShaderDatabase.MetaOverlay, Color.white);
							float BaseAlt = AltitudeLayer.WorldClipper.AltitudeFor();
							if (mat != null)
							{
								int altInd = 21;
								Mesh plane = MeshPool.plane08;
								Vector3 drawPos = b.TrueCenter();
								drawPos.y = BaseAlt + 0.046875f * (float)altInd;
								drawPos.x += (float)i;
								drawPos.z += (float)(grp - 2);
								float num2 = ((float)Math.Sin((double)((Time.realtimeSinceStartup + 397f * (float)(b.thingIDNumber % 571)) * 4f)) + 1f) * 0.5f;
								num2 = 0.3f + num2 * 0.7f;
								for (int j = 0; j < 2; j++)
								{
									Material material;
									if (j < 1)
									{
										material = FadedMaterialPool.FadedVersionOf(mat, num2);
									}
									else
									{
										material = FadedMaterialPool.FadedVersionOf(OutOfFuelMat, num2);
									}
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
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			if (base.Faction == Faction.OfPlayer)
			{
				string SelectDesc = "MSDrugMix.ChemSelectDesc".Translate();
				if (this.MixerThingDef == null)
				{
					string NoChem = "MSDrugMix.ChemSelect".Translate();
					yield return new Command_Action
					{
						defaultLabel = NoChem,
						icon = ContentFinder<Texture2D>.Get(this.chemicalTexPath, true),
						defaultDesc = SelectDesc,
						action = delegate()
						{
							this.MSMixerSelectChem();
						}
					};
				}
				else
				{
					Texture2D IconToUse = MSDrugMixUtility.GetMSMixIcon(this.MixerThingDef);
					string LabelDetail = this.MixerThingDef.label.CapitalizeFirst();
					LabelDetail = string.Concat(new object[]
					{
						LabelDetail,
						" [",
						this.NumProd,
						"] "
					});
					if (this.TotalProdWorkTicks > 0)
					{
						LabelDetail = LabelDetail + " (" + ((int)((float)(this.TotalProdWorkTicks - this.ProdWorkTicks) / (float)this.TotalProdWorkTicks * 100f)).ToString() + "%)";
					}
					yield return new Command_Action
					{
						defaultLabel = LabelDetail,
						icon = IconToUse,
						defaultDesc = SelectDesc,
						action = delegate()
						{
							this.MSMixerSelectChem();
						}
					};
				}
				string LabelProduce = "MSDrugMix.Production".Translate();
				string LabelProduceDesc = "MSDrugMix.ProductionDesc".Translate();
				if (this.isProducing)
				{
					if (this.MixerThingDef != null)
					{
                        if (MSDrugMixUtility.RCPProdValues(this.MixerThingDef, out int ticks, out int minProd, out int maxProd, out string research))
                        {
                            LabelProduce += "MSDrugMix.ProdLabelRange".Translate(minProd.ToString(), maxProd.ToString());
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
					icon = ContentFinder<Texture2D>.Get(this.produceTexPath, true),
					defaultLabel = LabelProduce,
					defaultDesc = LabelProduceDesc,
					isActive = (() => this.isProducing),
					toggleAction = delegate()
					{
						this.ToggleProducing(this.isProducing);
					}
				};
				string LimitTexPath = this.FrontLimitPath;
				string LimitLabelDetail;
				if (this.StockLimit > 0)
				{
                    Building_MSDrugMix.StockLimitReached(this, this.MixerThingDef, this.StockLimit, out int ActualStockNum);
                    int LimitPct = ActualStockNum * 100 / this.StockLimit;
					LimitLabelDetail = "MSDrugMix.StockLabel".Translate(this.StockLimit.ToString(), LimitPct.ToString());
					LimitTexPath += this.StockLimit.ToString();
				}
				else
				{
					LimitLabelDetail = "MSDrugMix.StockLabelNL".Translate();
					LimitTexPath += "No";
				}
				LimitTexPath += this.EndLimitPath;
				Texture2D LimitIconToUse = ContentFinder<Texture2D>.Get(LimitTexPath, true);
				string SelectLimit = "MSDrugMix.SelectStockLimit".Translate();
				yield return new Command_Action
				{
					defaultLabel = LimitLabelDetail,
					icon = LimitIconToUse,
					defaultDesc = SelectLimit,
					action = delegate()
					{
						this.MSMixerSelectLimit();
					}
				};
				if (Prefs.DevMode)
				{
					yield return new Command_Toggle
					{
						icon = ContentFinder<Texture2D>.Get(this.debugTexPath, true),
						defaultLabel = "Debug Mode",
						defaultDesc = "Send debug messages to Log",
						isActive = (() => this.debug),
						toggleAction = delegate()
						{
							this.ToggleDebug(this.debug);
						}
					};
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00007CE8 File Offset: 0x00005EE8
		public void ToggleDebug(bool flag)
		{
			this.debug = !flag;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00007CF4 File Offset: 0x00005EF4
		public void ToggleProducing(bool flag)
		{
			this.isProducing = !flag;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00007D00 File Offset: 0x00005F00
		public void MSMixerSelectLimit()
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			List<int> Choices = MSDrugMixUtility.GetMaxStock();
			if (Choices.Count > 0)
			{
				for (int i = 0; i < Choices.Count; i++)
				{
					string text;
					if (Choices[i] > 0)
					{
						text = Choices[i].ToString();
					}
					else
					{
						text = "MSDrugMix.StockNoLimit".Translate();
					}
					int value = Choices[i];
					list.Add(new FloatMenuOption(text, delegate()
					{
						this.SetStockLimits(value);
					}, MenuOptionPriority.Default, null, null, 29f, null, null));
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00007DB0 File Offset: 0x00005FB0
		public void MSMixerSelectChem()
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			string text = "MSDrugsMix.SelNoChemical".Translate();
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.SetProdControlValues(null, false, 0, 0);
			}, MenuOptionPriority.Default, null, null, 29f, null, null));
			List<string> Choices = MSDrugMixUtility.GetMixList();
			if (Choices.Count > 0)
			{
				for (int i = 0; i < Choices.Count; i++)
				{
					ThingDef ChoiceChemDef = DefDatabase<ThingDef>.GetNamed(Choices[i], true);
					text = ChoiceChemDef.label.CapitalizeFirst();
					if (Building_MSDrugMix.IsChemAvailable(ChoiceChemDef))
					{
						list.Add(new FloatMenuOption(text, delegate()
						{
							this.SetProdControlValues(ChoiceChemDef, true, 0, 0);
						}, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, ChoiceChemDef), null));
					}
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00007E9A File Offset: 0x0000609A
		public void SetStockLimits(int aStockLim)
		{
			this.StockLimit = aStockLim;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00007EA4 File Offset: 0x000060A4
		public void SetProdControlValues(ThingDef tdef, bool prod, int num, int ticks)
		{
			if (tdef == null)
			{
				this.MixerThingDef = null;
				this.isProducing = false;
				this.NumProd = 0;
				this.ProdWorkTicks = 0;
				this.TotalProdWorkTicks = 0;
				return;
			}
			if (this.MixerThingDef != tdef)
			{
				this.MixerThingDef = tdef;
				this.NumProd = 0;
				this.ProdWorkTicks = 0;
				this.TotalProdWorkTicks = 0;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007EFD File Offset: 0x000060FD
		public bool IsWorking(Building b)
		{
			return !b.IsBrokenDown() && this.powerComp.PowerOn;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00007F1C File Offset: 0x0000611C
		public static bool IsChemAvailable(ThingDef chkchemDef)
		{
            return MSDrugMixUtility.RCPProdValues(chkchemDef, out _, out _, out _, out string research) && research != "" && DefDatabase<ResearchProjectDef>.GetNamed(research, false).IsFinished;
        }

		// Token: 0x06000098 RID: 152 RVA: 0x00007F58 File Offset: 0x00006158
		public static bool StockLimitReached(Building b, ThingDef stockThing, int stockLim, out int ActualStockNum)
		{
			ActualStockNum = 0;
			if (stockLim > 0 && stockThing != null)
			{
				List<Thing> StockListing = b.Map.listerThings.ThingsOfDef(stockThing);
				if (StockListing.Count > 0)
				{
					for (int i = 0; i < StockListing.Count; i++)
					{
						ActualStockNum += StockListing[i].stackCount;
					}
				}
				if (ActualStockNum >= stockLim)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00007FB4 File Offset: 0x000061B4
		public virtual bool HasEnoughMaterialInHoppers(ThingDef NeededThing, int required, bool isMin)
		{
			int num = 0;
			for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
			{
				IntVec3 c = this.AdjCellsCardinalInBounds[i];
				Thing thingNeed = null;
				Thing thingHopper = null;
				List<Thing> thingList = c.GetThingList(base.Map);
				for (int j = 0; j < thingList.Count; j++)
				{
					Thing thing3 = thingList[j];
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
			if (this.debug)
			{
				Log.Message(string.Concat(new string[]
				{
					"Enough Materials? (",
					(num >= required) ? "Yes" : "No",
					"): (",
					NeededThing.defName,
					") Found:",
					num.ToString(),
					" for ",
					required.ToString(),
					" required as ",
					isMin ? "Min" : "Max"
				}), false);
			}
			return num >= required;
		}

		// Token: 0x04000053 RID: 83
		public bool debug;

		// Token: 0x04000054 RID: 84
		public CompPowerTrader powerComp;

		// Token: 0x04000055 RID: 85
		public ThingDef MixerThingDef;

		// Token: 0x04000056 RID: 86
		public int ProdWorkTicks;

		// Token: 0x04000057 RID: 87
		public int TotalProdWorkTicks;

		// Token: 0x04000058 RID: 88
		public bool isProducing;

		// Token: 0x04000059 RID: 89
		public int NumProd;

		// Token: 0x0400005A RID: 90
		public int StockLimit;

		// Token: 0x0400005B RID: 91
		public float effeciencyFactor = 0.95f;

		// Token: 0x0400005C RID: 92
		private List<IntVec3> cachedAdjCellsCardinal;

		// Token: 0x0400005D RID: 93
		public Sustainer mixSustainer;

		// Token: 0x0400005E RID: 94
		public static string UITexPath = "Things/Building/Misc/MSDrugMixer/UI/";

		// Token: 0x0400005F RID: 95
		[NoTranslate]
		private readonly string produceTexPath = Building_MSDrugMix.UITexPath + "MSDrugsMixerProduce_Icon";

		// Token: 0x04000060 RID: 96
		[NoTranslate]
		private readonly string chemicalTexPath = Building_MSDrugMix.UITexPath + "MSDrugsMixerChem_Icon";

		// Token: 0x04000061 RID: 97
		[NoTranslate]
		private readonly string debugTexPath = Building_MSDrugMix.UITexPath + "MSDrugsMixerDebug_Icon";

		// Token: 0x04000062 RID: 98
		[NoTranslate]
		private readonly string FrontLimitPath = Building_MSDrugMix.UITexPath + "StockLimits/MSDrugMixerStock";

		// Token: 0x04000063 RID: 99
		[NoTranslate]
		private readonly string EndLimitPath = "Limit_icon";

		// Token: 0x02000039 RID: 57
		public struct RCPItemCanUse
		{
			// Token: 0x04000087 RID: 135
			public ThingDef def;

			// Token: 0x04000088 RID: 136
			public int Min;

			// Token: 0x04000089 RID: 137
			public int Max;

			// Token: 0x0400008A RID: 138
			public int Grp;
		}
	}
}
