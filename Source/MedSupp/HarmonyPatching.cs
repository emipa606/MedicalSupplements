using System.Reflection;
using HarmonyLib;
using Verse;

namespace MedSupp;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.RimWorld.MedSupp").PatchAll(Assembly.GetExecutingAssembly());
    }
}