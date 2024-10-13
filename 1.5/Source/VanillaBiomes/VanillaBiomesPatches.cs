using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;
using RimWorld.Planet;

namespace VanillaBiomes
{
    [StaticConstructorOnStartup]
    static class VanillaBiomesPatches
    {

        static VanillaBiomesPatches()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create("rimworld.vanillabiomes");
            Harmony harmony = new Harmony("rimworld.vanillabiomes");

            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.BeachMaker), "Init");
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("BeachMaker_Prefix"));
            harmony.Patch(targetmethod, prefixmethod, null);

            harmony.PatchAll();
            //MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Planet.World), "CoastDirectionAt");
            //HarmonyMethod prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("CoastDirectionAt_Prefix"));
            //harmony.Patch(targetmethod, prefixmethod, null);

            //if (BiomeSettings.spawnModdedPlantsAnimals)
            //{
            //    AddAnimalsWildBiomes();
            //    AddPlantsWildBiomes();
            //}

            Log.Message("More Vanilla Biomes initialized");
        }


        public static bool BeachMaker_Prefix(Map map)
        {
            if (map.Biome.defName.Contains("NoBeach"))
            {
                return false;
            }
            return true;
        }


    
    }


    [HarmonyPatch(typeof(GenStep_Terrain), "TerrainFrom")]
    static class RockyTerrainPatch
    {
        static void Postfix(IntVec3 c, ref TerrainDef __result)
        {
            if (__result == TerrainDef.Named("Sandstone_Rough"))
            {
                __result = GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
            }
        }
    }
}
