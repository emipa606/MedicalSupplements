using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSDrugMix
{
	// Token: 0x02000021 RID: 33
	public class MSDrugMixUtility
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x000081D0 File Offset: 0x000063D0
		public static Texture2D GetMSMixIcon(ThingDef t)
		{
			if (t != null)
			{
				if (t != null)
				{
					GraphicData graphicData = t.graphicData;
					if (((graphicData != null) ? graphicData.texPath : null) != null)
					{
						string texturePath = t.graphicData.texPath;
						if (t.graphicData.graphicClass.Name == "Graphic_StackCount")
						{
							texturePath = texturePath + "/" + t.defName + "_a";
						}
						return ContentFinder<Texture2D>.Get(texturePath, true);
					}
				}
				return ContentFinder<Texture2D>.Get(MSDrugMixUtility.ChemIconPath, true);
			}
			return ContentFinder<Texture2D>.Get(MSDrugMixUtility.ChemIconPath, true);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00008258 File Offset: 0x00006458
		public static bool RCPProdValues(ThingDef t, out int ticks, out int minProd, out int maxProd, out string research)
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
			List<ThingDef> list = new List<ThingDef>();
			List<ThingDef> allthings = DefDatabase<ThingDef>.AllDefsListForReading;
			if (allthings.Count > 0)
			{
				foreach (ThingDef thingdef in allthings)
				{
					List<ThingCategoryDef> thingCats = (thingdef != null) ? thingdef.thingCategories : null;
					if (thingCats != null && thingCats.Count > 0)
					{
						using (List<ThingCategoryDef>.Enumerator enumerator2 = thingCats.GetEnumerator())
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
			MSDrugMixUtility.cachedChunkList = list;
			return list;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00008558 File Offset: 0x00006758
		public static List<MSDrugMixUtility.MSRCPListItem> GetRCPList(ThingDef thingdef)
		{
			List<MSDrugMixUtility.MSRCPListItem> list = new List<MSDrugMixUtility.MSRCPListItem>();
			list.Clear();
			MSDrugMixUtility.MSRCPListItem item = default(MSDrugMixUtility.MSRCPListItem);
			if (thingdef.defName == "Neutroamine")
			{
				item.def = DefDatabase<ThingDef>.GetNamed("MSGlycerol", true);
				item.mixgrp = 1;
				item.num = 1;
				item.ratio = 1f;
				list.Add(item);
				item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
				item.mixgrp = 2;
				item.num = 2;
				item.ratio = 1f;
				list.Add(item);
			}
			else if (thingdef.defName == "MSGlycerol")
			{
				item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
				item.mixgrp = 1;
				item.num = 2;
				item.ratio = 1f;
				list.Add(item);
				item.def = DefDatabase<ThingDef>.GetNamed("Beer", true);
				item.mixgrp = 2;
				item.num = 1;
				item.ratio = 1f;
				list.Add(item);
				item.def = DefDatabase<ThingDef>.GetNamed("MSEthanol", true);
				item.mixgrp = 2;
				item.num = 1;
				item.ratio = 1f;
				list.Add(item);
			}
			else
			{
				if (thingdef.defName == "MSLithiumSalts")
				{
					item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
					item.mixgrp = 1;
					item.num = 5;
					item.ratio = 0.1f;
					list.Add(item);
					item.def = DefDatabase<ThingDef>.GetNamed("MSPhenol", true);
					item.mixgrp = 2;
					item.num = 10;
					item.ratio = 0.2f;
					list.Add(item);
					item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid", true);
					item.mixgrp = 2;
					item.num = 10;
					item.ratio = 0.2f;
					list.Add(item);
					List<ThingDef> useChunkList = new List<ThingDef>();
					if (MSDrugMixUtility.cachedChunkList.Count > 0)
					{
						useChunkList = MSDrugMixUtility.cachedChunkList;
					}
					else
					{
						useChunkList = MSDrugMixUtility.ChunkList();
					}
					if (useChunkList.Count <= 0)
					{
						return list;
					}
					using (List<ThingDef>.Enumerator enumerator = useChunkList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ThingDef ChunkDef = enumerator.Current;
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
					item.def = DefDatabase<ThingDef>.GetNamed("MSOPSeedPod", true);
					item.mixgrp = 1;
					item.num = 2;
					item.ratio = 1f;
					list.Add(item);
				}
				else if (thingdef.defName == "MSPhenol")
				{
					item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
					item.mixgrp = 1;
					item.num = 3;
					item.ratio = 1f;
					list.Add(item);
				}
				else
				{
					if (thingdef.defName == "MSMercurySalts")
					{
						List<ThingDef> useChunkList2 = new List<ThingDef>();
						if (MSDrugMixUtility.cachedChunkList.Count > 0)
						{
							useChunkList2 = MSDrugMixUtility.cachedChunkList;
						}
						else
						{
							useChunkList2 = MSDrugMixUtility.ChunkList();
						}
						if (useChunkList2.Count <= 0)
						{
							return list;
						}
						using (List<ThingDef>.Enumerator enumerator = useChunkList2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ThingDef ChunkDef2 = enumerator.Current;
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
						item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
						item.mixgrp = 1;
						item.num = 10;
						item.ratio = 1f;
						list.Add(item);
						item.def = DefDatabase<ThingDef>.GetNamed("MSMercurySalts", true);
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
							List<ThingDef> useChunkList3 = new List<ThingDef>();
							if (MSDrugMixUtility.cachedChunkList.Count > 0)
							{
								useChunkList3 = MSDrugMixUtility.cachedChunkList;
							}
							else
							{
								useChunkList3 = MSDrugMixUtility.ChunkList();
							}
							if (useChunkList3.Count <= 0)
							{
								return list;
							}
							using (List<ThingDef>.Enumerator enumerator = useChunkList3.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									ThingDef ChunkDef3 = enumerator.Current;
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
							item.def = DefDatabase<ThingDef>.GetNamed("MSSulphur", true);
							item.mixgrp = 1;
							item.num = 3;
							item.ratio = 1f;
							list.Add(item);
						}
						else if (thingdef.defName == "MSEthanol")
						{
							item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
							item.mixgrp = 1;
							item.num = 2;
							item.ratio = 1f;
							list.Add(item);
							item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid", true);
							item.mixgrp = 2;
							item.num = 1;
							item.ratio = 1f;
							list.Add(item);
						}
						else if (thingdef.defName == "MSHydrogenPeroxide")
						{
							item.def = DefDatabase<ThingDef>.GetNamed("Chemfuel", true);
							item.mixgrp = 1;
							item.num = 1;
							item.ratio = 1f;
							list.Add(item);
							item.def = DefDatabase<ThingDef>.GetNamed("Neutroamine", true);
							item.mixgrp = 2;
							item.num = 1;
							item.ratio = 0.2f;
							list.Add(item);
							item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid", true);
							item.mixgrp = 3;
							item.num = 1;
							item.ratio = 0.2f;
							list.Add(item);
						}
						else if (thingdef.defName == "MSVincaAlkaloid")
						{
							item.def = DefDatabase<ThingDef>.GetNamed("MSPerrywinkleLeaves", true);
							item.mixgrp = 1;
							item.num = 1;
							item.ratio = 5f;
							list.Add(item);
							item.def = DefDatabase<ThingDef>.GetNamed("MSPhenol", true);
							item.mixgrp = 2;
							item.num = 1;
							item.ratio = 0.2f;
							list.Add(item);
						}
						else if (thingdef.defName == "MSHydrochloricAcid")
						{
							List<ThingDef> useChunkList4 = new List<ThingDef>();
							if (MSDrugMixUtility.cachedChunkList.Count > 0)
							{
								useChunkList4 = MSDrugMixUtility.cachedChunkList;
							}
							else
							{
								useChunkList4 = MSDrugMixUtility.ChunkList();
							}
							if (useChunkList4.Count > 0)
							{
								foreach (ThingDef ChunkDef4 in useChunkList4)
								{
									item.def = ChunkDef4;
									item.mixed = true;
									item.mixgrp = 1;
									item.num = 1;
									item.ratio = 0.1f;
									list.Add(item);
								}
							}
							item.def = DefDatabase<ThingDef>.GetNamed("MSSulphuricAcid", true);
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
			int Value;
			if (ToConvert != "No Limit" && int.TryParse(ToConvert, out Value))
			{
				return Value;
			}
			return 0;
		}

		// Token: 0x04000064 RID: 100
		[NoTranslate]
		public static string ChemIconPath = "Things/Building/Misc/MSDrugMixer/UI/MSDrugsMixerChem_Icon";

		// Token: 0x04000065 RID: 101
		public static List<ThingDef> cachedChunkList = new List<ThingDef>();

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
