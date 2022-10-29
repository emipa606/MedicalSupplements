using System;
using System.Collections.Generic;
using Verse;

namespace MSOptions;

[StaticConstructorOnStartup]
internal static class MSOptions_Initializer
{
    static MSOptions_Initializer()
    {
        LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);
    }

    public static void Setup()
    {
        var allDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
        if (allDefs.Count <= 0)
        {
            return;
        }

        var MSList = MSResearchList();
        foreach (var ResDef in allDefs)
        {
            if (!MSList.Contains(ResDef.defName))
            {
                continue;
            }

            var Resbase = ResDef.baseCost;
            Resbase = checked((int)Math.Round(Resbase * (Controller.Settings.ResPct / 100f)));
            ResDef.baseCost = Resbase;
        }
    }

    public static List<string> MSResearchList()
    {
        var list = new List<string>();
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