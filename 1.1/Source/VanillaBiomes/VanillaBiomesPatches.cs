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

            if (BiomeSettings.spawnModdedPlantsAnimals)
            {
                AddAnimalsWildBiomes();
                //AddPlantsWildBiomes();
            }

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

        // adapted from RF-Archipelagos

        [HarmonyPatch(typeof(BiomeDef), "CachePlantCommonalitiesIfShould")]
        public static class AddPlantsWildBiomes
        {
            static bool Prefix(ref Dictionary<ThingDef, float> ___cachedPlantCommonalities, string ___defName, List<BiomePlantRecord> ___wildPlants)
            {
                if (___cachedPlantCommonalities != null)
                {
                    return true;
                }

                if(!___defName.Contains("ZBiome"))
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

                        // to check if it's in any of these biomes already
                      
                        if (!current.plant.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                        {
                            //Alpine Meadow
                            if (___defName == "ZBiome_AlpineMeadow")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "BorealForest"))
                                {
                                    float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "BorealForest").FirstOrDefault().commonality;
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
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                {
                                    if (!current.plant.wildBiomes.Any(b => b.biome.defName == "TemperateSwamp"))
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            // Cloud Forest
                            if (___defName == "ZBiome_CloudForest")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                                {
                                    if (!current.plant.IsTree)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                                        if (current.plant.IsTree)
                                        {
                                            commonality *= 0.4f;
                                        }
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                                else if (current.plant.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                {
                                    if (current.plant.IsTree)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                        if (current.plant.IsTree)
                                        {
                                            commonality *= 0.4f;
                                        }
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            // Coastal Dunes
                            if (___defName == "ZBiome_CoastalDunes")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                                {
                                    if (current.plant.purpose == PlantPurpose.Food)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "AridShrubland").FirstOrDefault().commonality;
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }

                                }
                                else if (current.plant.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                                {
                                    if (current.plant.purpose != PlantPurpose.Food)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            // Desert Oasis
                            if (___defName == "ZBiome_DesertOasis")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                                {
                                    if (current.plant.purpose == PlantPurpose.Food)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "AridShrubland").FirstOrDefault().commonality;
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                                else if (current.plant.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                                {
                                    if (current.plant.purpose != PlantPurpose.Food)
                                    {
                                        float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                                        ___cachedPlantCommonalities.Add(current, commonality);
                                    }
                                }
                            }

                            // Glaial Shield
                            if (___defName == "ZBiome_GlacialShield")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                                {
                                    float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "Tundra").FirstOrDefault().commonality;
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
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                                {
                                    float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "AridShrubland").FirstOrDefault().commonality;
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
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "SeaIce"))
                                {
                                    ___cachedPlantCommonalities.Add(current, current.plant.wildBiomes.Where(bi => bi.biome.defName == "SeaIce").FirstOrDefault().commonality);
                                }
                            }
 
                            //Marsh
                            if (___defName == "ZBiome_Marsh")
                            {
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "ColdBog"))
                                {
                                    float commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "ColdBog").FirstOrDefault().commonality;
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
                                if (current.plant.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                                {
                                    ___cachedPlantCommonalities.Add(current, current.plant.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality);
                                }
                                else if (current.plant.wildBiomes.Any(b => b.biome.defName == "Desert"))
                                {
                                    ___cachedPlantCommonalities.Add(current, current.plant.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality * 0.5f);
                                }
                            }

                        }

                    }
                }

                return false;

            }
        }


        // adapted from RF-Archipelagos
        private static void AddAnimalsWildBiomes()
        {
            foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (current.RaceProps?.wildBiomes != null)
                {
                    // to check if it's in any of these biomes already
                    if (!current.RaceProps.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {

                        //Dunes, Oasis
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Desert"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);

                            AnimalBiomeRecord newRecord2 = new AnimalBiomeRecord();
                            newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                            newRecord2.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord2);
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);

                            AnimalBiomeRecord newRecord2 = new AnimalBiomeRecord();
                            newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                            newRecord2.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord2);
                        }

                        //Cloud Forest
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);
                        }
                        else if (current.RaceProps.baseBodySize <= 0.5f && current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);
                        }

                        //Badlands
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                        {
                            if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateSwamp"))
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Badlands");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                if (!current.RaceProps.predator && current.RaceProps.baseBodySize > 1.0)    // less large herbivores
                                {
                                    newRecord1.commonality *= 0.5f;
                                }
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }
                            else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Badlands");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                newRecord1.commonality *= 0.5f;
                                if (!current.RaceProps.predator && current.RaceProps.baseBodySize > 1.0)    // less large herbivores
                                {
                                    newRecord1.commonality *= 0.5f;
                                }
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }
                        }

                        // Glacial Shield
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Tundra").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "IceSheet"))
                        {
                            AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                            newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "IceSheet").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord1);
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "BorealForest"))
                        {
                            if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "BorealForest").FirstOrDefault().commonality;
                                newRecord1.commonality *= 0.5f;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }
                        }
                        
                        
                        for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
                        {
                            //Iceberg
                            if (current.RaceProps.wildBiomes[j].biome.defName == "SeaIce")
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Iceberg_NoBeach");
                                newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }

                            //Meadow
                            if (current.RaceProps.wildBiomes[j].biome.defName == "BorealForest")
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                                newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                if (current.RaceProps.predator && current.RaceProps.maxPreyBodySize >= 0.9f)
                                {
                                    newRecord1.commonality *= 0.5f;
                                }
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }

                            //Grasslands
                            if (current.RaceProps.wildBiomes[j].biome.defName == "AridShrubland")
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Grasslands");
                                newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                if (current.RaceProps.herdAnimal)
                                {
                                    newRecord1.commonality *= 1.5f;
                                }
                                else
                                {
                                    newRecord1.commonality *= 0.5f;
                                }
                                current.RaceProps.wildBiomes.Add(newRecord1);

                            }


                            //Sandbar
                            if (current.RaceProps.wildBiomes[j].biome.defName == "ExtremeDesert")
                            {
                                AnimalBiomeRecord newRecord3 = new AnimalBiomeRecord();
                                newRecord3.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord3.commonality = current.RaceProps.wildBiomes[j].commonality;
                                current.RaceProps.wildBiomes.Add(newRecord3);
                            }

                            //Marsh
                            if (current.RaceProps.wildBiomes[j].biome.defName == "ColdBog")
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Marsh");
                                newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                            }


                            

                        }
                    }
                }
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
