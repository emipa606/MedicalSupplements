using System;
using System.Collections.Generic;
using Verse;

namespace MSOptions
{
	// Token: 0x02000003 RID: 3
	[StaticConstructorOnStartup]
	internal static class MSOptions_Initializer
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002082 File Offset: 0x00000282
		static MSOptions_Initializer()
		{
			LongEventHandler.QueueLongEvent(new Action(Setup), "LibraryStartup", false, null, true);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020A0 File Offset: 0x000002A0
		public static void Setup()
		{
			List<ResearchProjectDef> allDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
			if (allDefs.Count > 0)
			{
				List<string> MSList = MSResearchList();
				foreach (ResearchProjectDef ResDef in allDefs)
				{
					if (MSList.Contains(ResDef.defName))
					{
						float Resbase = ResDef.baseCost;
						Resbase = (float)(checked((int)Math.Round((double)(unchecked(Resbase * (Controller.Settings.ResPct / 100f))))));
						ResDef.baseCost = Resbase;
					}
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002138 File Offset: 0x00000338
		public static List<string> MSResearchList()
		{
			List<string> list = new List<string>();
			list.AddDistinct("MSMercury");
			list.AddDistinct("MSImmunisation");
			list.AddDistinct("MSCerebrax");
			list.AddDistinct("MSMedicalMat");
			list.AddDistinct("MSMedicalMatSpacer");
			list.AddDistinct("MSMedicalMatUltra");
			list.AddDistinct("MSMedicalMatEarly");
			list.AddDistinct("MSSurgeryItemsIndustrial");
			list.AddDistinct("MSSurgeryItemsSpacer");
			list.AddDistinct("MSBattleStims");
			list.AddDistinct("MSTranscendence");
			list.AddDistinct("MSMedicineUltra");
			list.AddDistinct("MSHealerMechSerum");
			list.AddDistinct("MSRessurectMechSerum");
			list.AddDistinct("MSNeuroMechSerum");
			list.AddDistinct("MSMultiVitamins");
			list.AddDistinct("MSMetasis");
			list.AddDistinct("MSAntinites");
			list.AddDistinct("MSRimoxicillin");
			list.AddDistinct("MSProphylactics");
			list.AddDistinct("MSAndrogen");
			list.AddDistinct("MSRimpeptic");
			list.AddDistinct("MSGlycerol");
			list.AddDistinct("MSFireThroat");
			list.AddDistinct("MSInhaler");
			list.AddDistinct("MSRimProPlus");
			list.AddDistinct("MSAspirin");
			list.AddDistinct("MSRimzac");
			list.AddDistinct("MSAntitox");
			list.AddDistinct("MSPhenol");
			list.AddDistinct("MSWipes");
			list.AddDistinct("MSContacts");
			list.AddDistinct("MSRimtarol");
			list.AddDistinct("MSRimedicrem");
			list.AddDistinct("MSRimBurnEaze");
			list.AddDistinct("MSLithiumSalts");
			list.AddDistinct("MSClarity");
			list.AddDistinct("MSPlacebo");
			list.AddDistinct("MSOpiumPoppy");
			list.AddDistinct("MSOpium");
			list.AddDistinct("MSRimCodamol");
			list.AddDistinct("MSMorphine");
			list.AddDistinct("MSLuciferium");
			list.AddDistinct("MSPerrywinkle");
			list.AddDistinct("MSVinca");
			list.AddDistinct("MSVinacol");
			return list;
		}
	}
}
