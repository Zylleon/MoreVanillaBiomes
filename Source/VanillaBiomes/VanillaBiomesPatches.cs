using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using Harmony;
using RimWorld.Planet;

namespace VanillaBiomes
{
    [StaticConstructorOnStartup]
    static class VanillaBiomesPatches
    {

        static VanillaBiomesPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.vanillabiomes");
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Planet.World), "CoastDirectionAt");
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("CoastDirectionAt_Prefix"));
            harmony.Patch(targetmethod, prefixmethod, null);


            targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_ScatterDeepResourceLumps), "Generate");
            prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("ScatterDeepResourceLumps_Prefix"));
            harmony.Patch(targetmethod, prefixmethod, null);

            targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_Terrain), "TerrainFrom");
            prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("TerrainFrom_Prefix"));
            harmony.Patch(targetmethod, prefixmethod, null);

            targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_ElevationFertility), "Generate");
            prefixmethod = new HarmonyMethod(typeof(VanillaBiomes.VanillaBiomesPatches).GetMethod("GenstepElevationFertility_Prefix"));
            harmony.Patch(targetmethod, prefixmethod, null);

            if (BiomeSettings.spawnModdedPlantsAnimals)
            {
                AddAnimalsWildBiomes();
                AddPlantsWildBiomes();
            }
        }


        public static bool GenstepElevationFertility_Prefix(Map map, GenStepParams parms)
        {
            if (map.Biome.defName == "ZBiome_Badlands")
            {
                GenStep_Badlands_ElevationFertility elevationFertility = new GenStep_Badlands_ElevationFertility();
                elevationFertility.Generate(map, parms);

                return false;
            }
            return true;
        }



        public static bool TerrainFrom_Prefix(IntVec3 c, Map map, float elevation, float fertility, RiverMaker river, bool preferSolid, ref TerrainDef __result)
        {
            if(map.Biome.defName == "ZBiome_Badlands")
            {
                // Set default first because of return types changing. Original sets this last.
                __result = TerrainDefOf.Sand;

                TerrainDef terrainDef = null;
                if (river != null)
                {
                    terrainDef = river.TerrainAt(c, true);
                }
                if (terrainDef == null && preferSolid)
                {
                    __result = GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
                    return false;
                }
                TerrainDef terrainDef2 = BeachMaker.BeachTerrainAt(c, map.Biome);
                if (terrainDef2 == TerrainDefOf.WaterOceanDeep)
                {
                    __result = terrainDef2;
                    return false;
                }
                if (terrainDef != null && terrainDef.IsRiver)
                {
                    __result = terrainDef;
                    return false;
                }
                if (terrainDef2 != null)
                {
                    __result = terrainDef2;
                    return false;
                }
                if (terrainDef != null)
                {
                    __result = terrainDef;
                    return false;
                }
                for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
                {
                    terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
                    if (terrainDef2 != null)
                    {
                        __result = terrainDef2;
                        return false;
                    }
                }
                if (elevation > 0.35f && elevation < 0.50f)
                {
                    __result = TerrainDefOf.Gravel;
                    return false;
                }
                if (elevation >= 0.50f)
                {
                    __result = GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
                    return false;
                }
                terrainDef2 = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
                if (terrainDef2 != null)
                {
                    __result = terrainDef2;
                    return false;
                }
                //if (!GenStep_Terrain.debug_WarnedMissingTerrain)
                //{
                //    Log.Error(string.Concat(new object[]
                //    {
                //    "No terrain found in biome ",
                //    map.Biome.defName,
                //    " for elevation=",
                //    elevation,
                //    ", fertility=",
                //    fertility
                //    }), false);
                //    GenStep_Terrain.debug_WarnedMissingTerrain = true;
                //}

                return false;
            }
            return true;
        }


        public static bool ScatterDeepResourceLumps_Prefix(Map map, GenStepParams parms)
        {
            if(map.Biome.defName == "ZBiome_Sandbar_NoBeach")
            {
                GenStep_OceanDeepResources deepLumps = new GenStep_OceanDeepResources();
                deepLumps.Generate(map, parms);

                return false;
            }
            return true;
        }

        // from RF-Archipelagos
        public static bool CoastDirectionAt_Prefix(int tileID, ref Rot4 __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tileID].biome.defName.Contains("NoBeach"))
            {
                __result = Rot4.Invalid;
                return false;
            }
            return true;
        }




        #region gahhhhhh
        // adapted from RF-Archipelagos
        private static void AddPlantsWildBiomes()
        {

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant?.wildBiomes != null)
                {

                    // to check if it's in any of these biomes already
                    if (!current.plant.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {
                        //Sandbar
                        if (current.plant.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                        {
                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                            newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                            current.plant.wildBiomes.Add(newRecord1);
                        }
                        else if (current.plant.wildBiomes.Any(b => b.biome.defName == "Desert"))
                        {
                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                            newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                            current.plant.wildBiomes.Add(newRecord1);
                        }

                        for (int j = 0; j < current.plant.wildBiomes.Count; j++)
                        {
                            // icebergs
                            if (current.plant.wildBiomes[j].biome.defName == "SeaIce")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Iceberg_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //Meadow
                            if (current.plant.wildBiomes[j].biome.defName == "BorealForest")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 0.4f;
                                }
                                else if (current.plant.purpose == PlantPurpose.Beauty)
                                {
                                    newRecord1.commonality *= 2f;
                                }
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            //Grasslands
                            if (current.plant.wildBiomes[j].biome.defName == "AridShubland")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Grasslands");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 0.4f;
                                }
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                            if (current.plant.wildBiomes[j].biome.defName == "AridShubland")
                            {
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);

                                    PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                    newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                    newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord2);
                                }
                            }

                            if (current.plant.wildBiomes[j].biome.defName == "TropicalRainforest")
                            {
                                if (current.plant.purpose != PlantPurpose.Food)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);

                                    PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                    newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                    newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord2);
                                }

                                if (!current.plant.IsTree)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);
                                }

                            }

                            if (current.plant.wildBiomes[j].biome.defName == "TemperateForest")
                            {
                                if (current.plant.IsTree)
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);
                                }

                            }


                            //Marsh
                            if (current.plant.wildBiomes[j].biome.defName == "ColdBog")
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Marsh");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 0.3f;
                                }
                                current.plant.wildBiomes.Add(newRecord1);
                            }

                        }
                    }
                }
            }
        }


        // adapted from RF-Archipelagos
        private static void AddAnimalsWildBiomes()
        {
            foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (current.RaceProps.wildBiomes != null)
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
}
