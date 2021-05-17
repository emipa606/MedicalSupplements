using System.Reflection;
using HarmonyLib;
using Verse;

namespace MedSupp
{
    // Token: 0x02000029 RID: 41
    [StaticConstructorOnStartup]
    internal static class HarmonyPatching
    {
        // Token: 0x060000C6 RID: 198 RVA: 0x000096E5 File Offset: 0x000078E5
        static HarmonyPatching()
        {
            new Harmony("com.Pelador.RimWorld.MedSupp").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}