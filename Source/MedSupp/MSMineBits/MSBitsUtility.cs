using System;
using Verse;

namespace MSMineBits
{
	// Token: 0x02000010 RID: 16
	public class MSBitsUtility
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00004664 File Offset: 0x00002864
		public static bool GetIsBitsSource(ThingDef defSource, bool isSource, Pawn pawn, out ThingDef bitsdef, out int bitsyield)
		{
			bitsdef = null;
			bitsyield = 0;
			if (MSBitsUtility.GetBitsSource(defSource) || isSource)
			{
				bitsdef = DefDatabase<ThingDef>.GetNamed(MSBitsUtility.bitschance.RandomElementByWeight((Pair<string, float> x) => x.Second).First, false);
				if (bitsdef != null)
				{
					bitsyield = MSBitsUtility.GetBitsYield(defSource, bitsdef);
					if (bitsyield > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000046D4 File Offset: 0x000028D4
		public static int GetBitsYield(ThingDef defSource, ThingDef def)
		{
			int yield = 0;
			if (defSource != null)
			{
				if (((defSource != null) ? defSource.defName : null) == "ChunkLepidolite")
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(1, 4);
							}
						}
						else
						{
							yield = Rand.Range(1, 1);
						}
					}
					else
					{
						yield = Rand.Range(10, 15);
					}
				}
				else if (((defSource != null) ? defSource.defName : null) == "ChunkSerpentinite")
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(10, 15);
							}
						}
						else
						{
							yield = Rand.Range(1, 1);
						}
					}
					else
					{
						yield = Rand.Range(1, 3);
					}
				}
				else if (((defSource != null) ? defSource.defName : null) == "ChunkDunite")
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(3, 7);
							}
						}
						else
						{
							yield = Rand.Range(2, 5);
						}
					}
					else
					{
						yield = Rand.Range(1, 2);
					}
				}
				else if (((defSource != null) ? defSource.defName : null) == "ChunkPegmatite")
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(1, 2);
							}
						}
						else
						{
							yield = Rand.Range(1, 1);
						}
					}
					else
					{
						yield = Rand.Range(5, 20);
					}
				}
				else if (((defSource != null) ? defSource.defName : null) == "ChunkMarble" || ((defSource != null) ? defSource.defName : null) == "ChunkLimestone" || ((defSource != null) ? defSource.defName : null) == "ChunkEmperadordark" || ((defSource != null) ? defSource.defName : null) == "ChunkBlueschist" || ((defSource != null) ? defSource.defName : null) == "ChunkGreenSchist" || ((defSource != null) ? defSource.defName : null) == "ChunkDacite" || ((defSource != null) ? defSource.defName : null) == "ChunkSovite" || ((defSource != null) ? defSource.defName : null) == "ChunkChalk" || ((defSource != null) ? defSource.defName : null) == "ChunkCreoleMarble" || ((defSource != null) ? defSource.defName : null) == "ChunkEtowahMarble" || ((defSource != null) ? defSource.defName : null) == "ChunkDiorite")
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(1, 3);
							}
						}
						else
						{
							yield = Rand.Range(1, 1);
						}
					}
					else
					{
						yield = Rand.Range(3, 7);
					}
				}
				else
				{
					string defName = def.defName;
					if (!(defName == "MSLithiumSalts"))
					{
						if (!(defName == "MSMercurySalts"))
						{
							if (defName == "MSSulphur")
							{
								yield = Rand.Range(5, 10);
							}
						}
						else
						{
							yield = Rand.Range(1, 1);
						}
					}
					else
					{
						yield = Rand.Range(5, 10);
					}
				}
			}
			else
			{
				string defName = def.defName;
				if (!(defName == "MSLithiumSalts"))
				{
					if (!(defName == "MSMercurySalts"))
					{
						if (defName == "MSSulphur")
						{
							yield = Rand.Range(5, 10);
						}
					}
					else
					{
						yield = Rand.Range(1, 1);
					}
				}
				else
				{
					yield = Rand.Range(5, 10);
				}
			}
			return yield;
		}
		internal static uint ComputeStringHash(string s)
		{
			uint num = 0;
			if (s != null)
			{
				num = 2166136261U;
				for (int i = 0; i < s.Length; i++)
				{
					num = ((uint)s[i] ^ num) * 16777619U;
				}
			}
			return num;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004AC8 File Offset: 0x00002CC8
		public static bool GetBitsSource(ThingDef def)
		{
			bool isBitsSource = false;
			if (def != null)
			{
				string defName = def.defName;
				uint num = ComputeStringHash(defName);
				if (num <= 1154306412U)
				{
					if (num <= 702995605U)
					{
						if (num <= 450668030U)
						{
							if (num <= 135430043U)
							{
								if (num != 130326325U)
								{
									if (num == 135430043U)
									{
										if (defName == "ChunkBasalt")
										{
											isBitsSource = true;
										}
									}
								}
								else if (defName == "ChunkCreoleMarble")
								{
									isBitsSource = true;
								}
							}
							else if (num != 237089535U)
							{
								if (num != 334370396U)
								{
									if (num == 450668030U)
									{
										if (defName == "ChunkAnorthosite")
										{
											isBitsSource = true;
										}
									}
								}
								else if (defName == "ChunkBlueschist")
								{
									isBitsSource = true;
								}
							}
							else if (defName == "ChunkGreenSchist")
							{
								isBitsSource = true;
							}
						}
						else if (num <= 468849279U)
						{
							if (num != 459180747U)
							{
								if (num == 468849279U)
								{
									if (defName == "ChunkGneiss")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkLepidolite")
							{
								isBitsSource = true;
							}
						}
						else if (num != 497423552U)
						{
							if (num != 547426595U)
							{
								if (num == 702995605U)
								{
									if (defName == "ChunkDarkAndesite")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkDunite")
							{
								isBitsSource = true;
							}
						}
						else if (defName == "ChunkDiorite")
						{
							isBitsSource = true;
						}
					}
					else if (num <= 990094961U)
					{
						if (num <= 803651291U)
						{
							if (num != 754271499U)
							{
								if (num == 803651291U)
								{
									if (defName == "ChunkQuartzite")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkSiltstone")
							{
								isBitsSource = true;
							}
						}
						else if (num != 840754687U)
						{
							if (num != 930377356U)
							{
								if (num == 990094961U)
								{
									if (defName == "ChunkGabbro")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkLignite")
							{
								isBitsSource = true;
							}
						}
						else if (defName == "ChunkSlate")
						{
							isBitsSource = true;
						}
					}
					else if (num <= 1113897122U)
					{
						if (num != 1103574401U)
						{
							if (num == 1113897122U)
							{
								if (defName == "ChunkDacite")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkEtowahMarble")
						{
							isBitsSource = true;
						}
					}
					else if (num != 1117948611U)
					{
						if (num != 1138947383U)
						{
							if (num == 1154306412U)
							{
								if (defName == "ChunkSchist")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkSyenite")
						{
							isBitsSource = true;
						}
					}
					else if (defName == "ChunkMarble")
					{
						isBitsSource = true;
					}
				}
				else if (num <= 3134642644U)
				{
					if (num <= 2223887258U)
					{
						if (num <= 1807593693U)
						{
							if (num != 1789891285U)
							{
								if (num == 1807593693U)
								{
									if (defName == "ChunkChalk")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkMonzonite")
							{
								isBitsSource = true;
							}
						}
						else if (num != 2078373069U)
						{
							if (num != 2104149328U)
							{
								if (num == 2223887258U)
								{
									if (defName == "ChunkSovite")
									{
										isBitsSource = true;
									}
								}
							}
							else if (defName == "ChunkRhyolite")
							{
								isBitsSource = true;
							}
						}
						else if (defName == "ChunkVibrantDunite")
						{
							isBitsSource = true;
						}
					}
					else if (num <= 2723944204U)
					{
						if (num != 2277173191U)
						{
							if (num == 2723944204U)
							{
								if (defName == "ChunkClayStone")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkScoria")
						{
							isBitsSource = true;
						}
					}
					else if (num != 2981327554U)
					{
						if (num != 3027871598U)
						{
							if (num == 3134642644U)
							{
								if (defName == "ChunkGranite")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkGreenGabbro")
						{
							isBitsSource = true;
						}
					}
					else if (defName == "ChunkSerpentinite")
					{
						isBitsSource = true;
					}
				}
				else if (num <= 3386876289U)
				{
					if (num <= 3317400192U)
					{
						if (num != 3242717934U)
						{
							if (num == 3317400192U)
							{
								if (defName == "ChunkPegmatite")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkLherzolite")
						{
							isBitsSource = true;
						}
					}
					else if (num != 3349364057U)
					{
						if (num != 3354811171U)
						{
							if (num == 3386876289U)
							{
								if (defName == "ChunkSandstone")
								{
									isBitsSource = true;
								}
							}
						}
						else if (defName == "ChunkMigmatite")
						{
							isBitsSource = true;
						}
					}
					else if (defName == "ChunkCharnockite")
					{
						isBitsSource = true;
					}
				}
				else if (num <= 3660010543U)
				{
					if (num != 3604908289U)
					{
						if (num == 3660010543U)
						{
							if (defName == "ChunkEmperadordark")
							{
								isBitsSource = true;
							}
						}
					}
					else if (defName == "ChunkJaspillite")
					{
						isBitsSource = true;
					}
				}
				else if (num != 3782560473U)
				{
					if (num != 4006516401U)
					{
						if (num == 4282638488U)
						{
							if (defName == "ChunkLimestone")
							{
								isBitsSource = true;
							}
						}
					}
					else if (defName == "ChunkAndesite")
					{
						isBitsSource = true;
					}
				}
				else if (defName == "ChunkThometzekite")
				{
					isBitsSource = true;
				}
			}
			return isBitsSource;
		}

		// Token: 0x04000023 RID: 35
		public const string ChunkGranite = "ChunkGranite";

		// Token: 0x04000024 RID: 36
		public const string ChunkMarble = "ChunkMarble";

		// Token: 0x04000025 RID: 37
		public const string ChunkLimestone = "ChunkLimestone";

		// Token: 0x04000026 RID: 38
		public const string ChunkSlate = "ChunkSlate";

		// Token: 0x04000027 RID: 39
		public const string ChunkSandstone = "ChunkSandstone";

		// Token: 0x04000028 RID: 40
		public const string CollapsedRocks = "CollapsedRocks";

		// Token: 0x04000029 RID: 41
		public const string rxCollapsedRoofRocks = "rxCollapsedRoofRocks";

		// Token: 0x0400002A RID: 42
		internal const string ChunkEmperadordark = "ChunkEmperadordark";

		// Token: 0x0400002B RID: 43
		internal const string ChunkBlueschist = "ChunkBlueschist";

		// Token: 0x0400002C RID: 44
		internal const string ChunkSerpentinite = "ChunkSerpentinite";

		// Token: 0x0400002D RID: 45
		internal const string ChunkDacite = "ChunkDacite";

		// Token: 0x0400002E RID: 46
		internal const string ChunkSovite = "ChunkSovite";

		// Token: 0x0400002F RID: 47
		internal const string ChunkChalk = "ChunkChalk";

		// Token: 0x04000030 RID: 48
		internal const string ChunkCreoleMarble = "ChunkCreoleMarble";

		// Token: 0x04000031 RID: 49
		internal const string ChunkEtowahMarble = "ChunkEtowahMarble";

		// Token: 0x04000032 RID: 50
		internal const string ChunkGreenSchist = "ChunkGreenSchist";

		// Token: 0x04000033 RID: 51
		internal const string ChunkVibrantDunite = "ChunkVibrantDunite";

		// Token: 0x04000034 RID: 52
		internal const string ChunkDarkAndesite = "ChunkDarkAndesite";

		// Token: 0x04000035 RID: 53
		internal const string ChunkAnorthosite = "ChunkAnorthosite";

		// Token: 0x04000036 RID: 54
		internal const string ChunkBasalt = "ChunkBasalt";

		// Token: 0x04000037 RID: 55
		internal const string ChunkCharnockite = "ChunkCharnockite";

		// Token: 0x04000038 RID: 56
		internal const string ChunkGreenGabbro = "ChunkGreenGabbro";

		// Token: 0x04000039 RID: 57
		internal const string ChunkLherzolite = "ChunkLherzolite";

		// Token: 0x0400003A RID: 58
		internal const string ChunkMonzonite = "ChunkMonzonite";

		// Token: 0x0400003B RID: 59
		internal const string ChunkRhyolite = "ChunkRhyolite";

		// Token: 0x0400003C RID: 60
		internal const string ChunkScoria = "ChunkScoria";

		// Token: 0x0400003D RID: 61
		internal const string ChunkJaspillite = "ChunkJaspillite";

		// Token: 0x0400003E RID: 62
		internal const string ChunkLignite = "ChunkLignite";

		// Token: 0x0400003F RID: 63
		internal const string ChunkSiltstone = "ChunkSiltstone";

		// Token: 0x04000040 RID: 64
		internal const string ChunkMigmatite = "ChunkMigmatite";

		// Token: 0x04000041 RID: 65
		internal const string ChunkThometzekite = "ChunkThometzekite";

		// Token: 0x04000042 RID: 66
		internal const string ChunkLepidolite = "ChunkLepidolite";

		// Token: 0x04000043 RID: 67
		internal const string ChunkClaystone = "ChunkClayStone";

		// Token: 0x04000044 RID: 68
		internal const string ChunkAndesite = "ChunkAndesite";

		// Token: 0x04000045 RID: 69
		internal const string ChunkSyenite = "ChunkSyenite";

		// Token: 0x04000046 RID: 70
		internal const string ChunkGneiss = "ChunkGneiss";

		// Token: 0x04000047 RID: 71
		internal const string ChunkQuartzite = "ChunkQuartzite";

		// Token: 0x04000048 RID: 72
		internal const string ChunkSchist = "ChunkSchist";

		// Token: 0x04000049 RID: 73
		internal const string ChunkGabbro = "ChunkGabbro";

		// Token: 0x0400004A RID: 74
		internal const string ChunkDiorite = "ChunkDiorite";

		// Token: 0x0400004B RID: 75
		internal const string ChunkDunite = "ChunkDunite";

		// Token: 0x0400004C RID: 76
		internal const string ChunkPegmatite = "ChunkPegmatite";

		// Token: 0x0400004D RID: 77
		public const string MSLithiumSalts = "MSLithiumSalts";

		// Token: 0x0400004E RID: 78
		public const string MSMercurySalts = "MSMercurySalts";

		// Token: 0x0400004F RID: 79
		public const string MSSulphur = "MSSulphur";

		// Token: 0x04000050 RID: 80
		private static readonly Pair<string, float>[] bitschance = new Pair<string, float>[]
		{
			new Pair<string, float>("MSLithiumSalts", 0.85f),
			new Pair<string, float>("MSMercurySalts", 0.1f),
			new Pair<string, float>("MSSulphur", 1f)
		};
	}
}
