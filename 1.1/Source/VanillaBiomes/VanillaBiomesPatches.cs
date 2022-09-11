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
            //    //AddPlantsWildBiomes();
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


        #region modded plants and animals


        [HarmonyPatch(typeof(BiomeDef), "CachePlantCommonalitiesIfShould")]
        public static class AddPlantsWildBiomes
        {
            static bool Prefix(ref Dictionary<ThingDef, float> ___cachedPlantCommonalities, string ___defName, List<BiomePlantRecord> ___wildPlants)
            {
                if (___cachedPlantCommonalities != null)
                {
                    return true;
                }

                if (!___defName.Contains("ZBiome"))
                {
                    return true;
                }

                ___cachedPlantCommonalities = new Dictionary<ThingDef, float>();
                for (int i = 0; i < ___wildPlants.Count; i++)
                {
                    if (___wildPlants[i].plant != null)
                    {
                        ___cachedPlantCommonalities.Add(___wildPlants[i].plant, ___wildPlants[i].commonality);
                    }
                }


                foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
                {
                    if (current.plant?.wildBiomes != null)
                    {
                        // add the plant if it's naturally in this biome
                        for (int j = 0; j < current.plant.wildBiomes.Count; j++)
                        {
                            if (current.plant.wildBiomes[j].biome.defName == ___defName)
                            {
                                ___cachedPlantCommonalities.Add(current, current.plant.wildBiomes[j].commonality);
                            }
                        }
                    }
                    if(current.plant != null && !___cachedPlantCommonalities.ContainsKey((current)) && current.modContentPack.PackageId != ModContentPack.CoreModPackageId)
                    {
                        //Alpine Meadow
                        if (___defName == "ZBiome_AlpineMeadow")
                        {
                            if (BiomeDefOf.BorealForest.CommonalityOfPlant(current) != 0)
                            {
                                float commonality = BiomeDefOf.BorealForest.CommonalityOfPlant(current);
                                if (current.plant.IsTree)
                                {
                                    commonality *= 0.4f;
                                }
                                if (current.plant.purpose == PlantPurpose.Beauty)
                                {
                                    commonality *= 2f;
                                }
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    commonality *= 1.4f;
                                }

                                ___cachedPlantCommonalities.Add(current, commonality);
                            }
                        }

                        // Badlands
                        if (___defName == "ZBiome_Badlands")
                        {
                            if (BiomeDefOf.TemperateForest.CommonalityOfPlant(current) != 0)
                            {
                                if (BiomeDef.Named("TemperateSwamp").CommonalityOfPlant(current) == 0)
                                {
                                    float commonality = BiomeDefOf.TemperateForest.CommonalityOfPlant(current);
                                    if (current.plant.purpose == PlantPurpose.Food)
                                    {
                                        commonality *= 0.6f;
                                    }
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                        }


                        // Cloud Forest
                        if (___defName == "ZBiome_CloudForest")
                        {
                            if (BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current) != 0)
                            {
                                if (!current.plant.IsTree)
                                {
                                    float commonality = BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current);
                                    if (current.plant.IsTree)
                                    {
                                        commonality *= 0.4f;
                                    }
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                            else if (BiomeDefOf.TemperateForest.CommonalityOfPlant(current) != 0)
                            {
                                if (current.plant.IsTree)
                                {
                                    float commonality = BiomeDefOf.TemperateForest.CommonalityOfPlant(current);
                                    if (current.plant.IsTree)
                                    {
                                        commonality *= 0.4f;
                                    }
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                        }

                        //Coastal Dunes
                        if (___defName == "ZBiome_CoastalDunes")
                        {
                            if (BiomeDefOf.AridShrubland.CommonalityOfPlant(current) != 0)
                            {
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    float commonality = BiomeDefOf.AridShrubland.CommonalityOfPlant(current);
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                            else if (BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current) != 0)
                            {
                                if (current.plant.purpose != PlantPurpose.Food)
                                {
                                    float commonality = BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current);
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                        }

                        // Desert Oasis
                        if (___defName == "ZBiome_DesertOasis")
                        {
                            if (BiomeDefOf.AridShrubland.CommonalityOfPlant(current) != 0)
                            {
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    float commonality = BiomeDefOf.AridShrubland.CommonalityOfPlant(current);
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                            else if (BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current) != 0)
                            {
                                if (current.plant.purpose != PlantPurpose.Food)
                                {
                                    float commonality = BiomeDefOf.TropicalRainforest.CommonalityOfPlant(current);
                                    ___cachedPlantCommonalities.Add(current, commonality);
                                }
                            }
                        }


                        // Glaial Shield
                        if (___defName == "ZBiome_GlacialShield")
                        {
                            if (BiomeDefOf.Tundra.CommonalityOfPlant(current) != 0)
                            {
                                float commonality = BiomeDefOf.Tundra.CommonalityOfPlant(current);
                                if (current.plant.IsTree)
                                {
                                    commonality *= 1.7f;
                                }
                                if (current.plant.purpose == PlantPurpose.Beauty || current.plant.purpose == PlantPurpose.Food)
                                {
                                    commonality *= 1.2f;
                                }
                                ___cachedPlantCommonalities.Add(current, commonality);
                            }
                        }

                        //Grasslands
                        if (___defName == "ZBiome_Grasslands")
                        {
                            if (BiomeDefOf.AridShrubland.CommonalityOfPlant(current) != 0)
                            {
                                float commonality = BiomeDefOf.AridShrubland.CommonalityOfPlant(current);
                                if (current.plant.IsTree)
                                {
                                    commonality *= 0.4f;
                                }
                                ___cachedPlantCommonalities.Add(current, commonality);
                            }
                        }

                        // icebergs
                        if (___defName == "ZBiome_Iceberg_NoBeach")
                        {
                            if (BiomeDefOf.SeaIce.CommonalityOfPlant(current) != 0)
                            {
                                float commonality = BiomeDefOf.SeaIce.CommonalityOfPlant(current);
                                ___cachedPlantCommonalities.Add(current, commonality);
                            }
                        }

                        //Marsh
                        if (___defName == "ZBiome_Marsh")
                        {
                            if (BiomeDef.Named("ColdBog").CommonalityOfPlant(current) != 0)
                            {
                                float commonality = BiomeDef.Named("ColdBog").CommonalityOfPlant(current);

                                if (current.plant.IsTree)
                                {
                                    commonality *= 0.3f;
                                }
                                ___cachedPlantCommonalities.Add(current, commonality);
                            }
                        }


                        //Sandbar
                        if (___defName == "ZBiome_Sandbar_NoBeach")
                        {
                            if (BiomeDefOf.ExtremeDesert.CommonalityOfPlant(current) != 0)
                            {
                                ___cachedPlantCommonalities.Add(current, BiomeDefOf.ExtremeDesert.CommonalityOfPlant(current));
                            }
                            else if (BiomeDefOf.Desert.CommonalityOfPlant(current) != 0)
                            {
                                ___cachedPlantCommonalities.Add(current, 0.5f * BiomeDefOf.Desert.CommonalityOfPlant(current));
                            }
                        }

                    }

                }
                

                return false;

            }
        }


        // adapted from RF-Archipelagos
        [HarmonyPatch(typeof(BiomeDef), "CommonalityOfAnimal")]
        public static class AddAnimalsWildBiomes
        {
            static bool Prefix(PawnKindDef animalDef, ref Dictionary<PawnKindDef, float> ___cachedAnimalCommonalities, string ___defName, List<BiomeAnimalRecord> ___wildAnimals)
            {
                if (___cachedAnimalCommonalities != null)
                {
                    return true;
                }

                if (!___defName.Contains("ZBiome"))
                {
                    return true;
                }

                ___cachedAnimalCommonalities = new Dictionary<PawnKindDef, float>();
                for (int i = 0; i < ___wildAnimals.Count; i++)
                {
                    ___cachedAnimalCommonalities.Add(___wildAnimals[i].animal, ___wildAnimals[i].commonality);
                }
                foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
                {
                    if (current.RaceProps.wildBiomes == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
                    {
                        if (current.RaceProps.wildBiomes[j].biome.defName == ___defName)
                        {
                            ___cachedAnimalCommonalities.Add(current, current.RaceProps.wildBiomes[j].commonality);
                        }
                    }


                    if (current.RaceProps.wildBiomes != null)
                    {
                        // to check if it's in any of these biomes already
                        if (!current.RaceProps.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                        {
                            //Alpine Meadow
                            if (___defName == "ZBiome_AlpineMeadow")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "BorealForest"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(b => b.biome.defName == "BorealForest").FirstOrDefault().commonality;
                                    if (current.RaceProps.predator && current.RaceProps.maxPreyBodySize >= 0.9f)
                                    {
                                       commonality *= 0.5f;
                                    }
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                                {
                                    if (current.RaceProps.predator && current.RaceProps.maxPreyBodySize >= 0.9f)
                                    {
                                        float commonality = current.RaceProps.wildBiomes.Where(b => b.biome.defName == "BorealForest").FirstOrDefault().commonality;
                                        commonality *= 0.5f;
                                        ___cachedAnimalCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            //Badlands
                            if (___defName == "ZBiome_Badlands")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                {
                                    if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateSwamp"))
                                    {
                                        float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;

                                        if (!current.RaceProps.predator && current.RaceProps.baseBodySize > 1.0)    // less large herbivores
                                        {
                                            commonality *= 0.5f;
                                        }
                                        ___cachedAnimalCommonalities.Add(current, commonality);
                                    }
                                    else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                                    {
                                        float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "AridShrubland").FirstOrDefault().commonality;
                                        commonality *= 0.7f;
                                        if (!current.RaceProps.predator && current.RaceProps.baseBodySize > 1.0)    // less large herbivores
                                        {
                                            commonality *= 0.5f;
                                        }
                                        ___cachedAnimalCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            // Cloud Forest
                            if (___defName == "ZBiome_CloudForest")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.baseBodySize <= 0.5f && current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }

                            // Coastal Dunes
                            if (___defName == "ZBiome_CoastalDunes")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Desert"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }

                            // Desert Oasis
                            if (___defName == "ZBiome_DesertOasis")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Desert"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }


                            // Glacial Shield
                            if (___defName == "ZBiome_GlacialShield")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Tundra").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.baseBodySize <= 0.5f && current.RaceProps.wildBiomes.Any(b => b.biome.defName == "IceSheet"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "IceSheet").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                                else if (current.RaceProps.baseBodySize <= 0.5f && current.RaceProps.wildBiomes.Any(b => b.biome.defName == "BorealForest"))
                                {
                                    if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                    {
                                        float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "BorealForest").FirstOrDefault().commonality;
                                        ___cachedAnimalCommonalities.Add(current, commonality);
                                    }
                                }
                            }


                            // Grasslands
                            if (___defName == "ZBiome_Grasslands")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "AridShruland"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "AridShruland").FirstOrDefault().commonality;
                                    if (current.RaceProps.herdAnimal)
                                    {
                                        commonality *= 1.5f;
                                    }
                                    else
                                    {
                                        commonality *= 0.5f;
                                    }
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }


                            // Iceberg
                            if (___defName == "ZBiome_Iceberg_NoBeach")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "SeaIce"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "SeaIce").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }



                            //Marsh
                            if (___defName == "ZBiome_Marsh")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ColdBog"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ColdBog").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }



                            // Sandbar
                            if (___defName == "ZBiome_Sandbar_NoBeach")
                            {
                                if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                                {
                                    float commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                    ___cachedAnimalCommonalities.Add(current, commonality);
                                }
                            }

                        }
                    }
                }

                return true;

            }
        }
    


        #endregion
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
