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

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);

            AddAnimalsWildBiomes();
            AddPlantsWildBiomes();

            //AddBiomesWildPlants();

            //TestPlantsOld();
            //TestPlantsOldB();

            //TestPlantsNew();
            //TestPlantsNewB();


        }

        private static void TestPlantsOld()
        {
            Log.Message("---Old version 1---");
            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant?.wildBiomes != null)
                {
                    if (!current.plant.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {
                        for (int j = 0; j < current.plant.wildBiomes.Count; j++)
                        {
                            if (current.plant.wildBiomes[j].biome.defName == "TemperateForest")
                            {
                                Log.Message("-----------------------------");
                                Log.Message("Plant: " + current.defName);
                                Log.Message("Found commonality: " + current.plant.wildBiomes[j].commonality);

                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                                newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                Log.Message("Added commonality to record: " + newRecord1.commonality);

                                current.plant.wildBiomes.Add(newRecord1);
                                Log.Message("Added to plant with commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "ZBiome_AlpineMeadow").FirstOrDefault().commonality);
                                Log.Message("BIOME RESULT---: " + VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current));
                            }                    
                        }
                    }
                }
            }
        }


        private static void TestPlantsOldB()
        {
            Log.Message("---Old version 2---");

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant?.wildBiomes != null)
                {
                    if (!current.plant.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {
                        if (current.plant.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                        {
                            Log.Message("-----------------------------");
                            Log.Message("Plant: " + current.defName);
                            Log.Message("Found commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality);

                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                            newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                            Log.Message("Added commonality to record: " + newRecord1.commonality);

                            current.plant.wildBiomes.Add(newRecord1);
                            Log.Message("Added to plant with commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "ZBiome_AlpineMeadow").FirstOrDefault().commonality);
                            Log.Message("BIOME RESULT---: " + VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current));
                        }
                    }
                }
            }
        }
        


        private static void TestPlantsNew()
        {
            Log.Message("---New version 1---");
            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant != null)
                {
                    if (BiomeDefOf.TemperateForest.CommonalityOfPlant(current) > 0f && VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current) == 0f)
                    {
                        try
                        {
                            Log.Message("-----------------------------");
                            Log.Message("Plant: " + current.defName);
                            Log.Message("Found commonality: " + BiomeDefOf.TemperateForest.CommonalityOfPlant(current));

                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                            newRecord1.commonality = BiomeDefOf.TemperateForest.CommonalityOfPlant(current);
                            Log.Message("Added commonality to record: " + newRecord1.commonality);

                            current.plant.wildBiomes.Add(newRecord1);
                            Log.Message("Added to plant with commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "ZBiome_AlpineMeadow").FirstOrDefault().commonality);
                            Log.Message("BIOME RESULT---: " + VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current));
                        }
                        catch (Exception ex)
                        {
                            Log.Message(ex.Message);
                        }
                    }
                }
            }
        }


        private static void TestPlantsNewB()
        {
            Log.Message("---New version 2---");

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant != null)
                {
                    if (BiomeDefOf.TemperateForest.CommonalityOfPlant(current) > 0f && VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current) == 0f)
                    {
                        try
                        {
                            Log.Message("-----------------------------");
                            Log.Message("Plant: " + current.defName);
                            Log.Message("Found commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality);

                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                            newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                            Log.Message("Added commonality to record: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality);

                            current.plant.wildBiomes.Add(newRecord1);
                            Log.Message("Added to plant with commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "ZBiome_AlpineMeadow").FirstOrDefault().commonality);
                            Log.Message("BIOME RESULT---: " + VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current));

                        }
                        catch (Exception ex)
                        {
                            Log.Message(ex.Message);
                        }
                    }
                }
            }
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

                            //Dunes, Oasis
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
                            newRecord2.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                            current.RaceProps.wildBiomes.Add(newRecord2);
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


        private static void AddBiomesWildAnimals()
        {
            //BiomeDef sandbar = VanillaBiomesDefOf.ZBiome_Sandbar_NoBeach;

            foreach (PawnKindDef animal in VanillaBiomesDefOf.ExtremeDesert.AllWildAnimals)
            {
                //BiomeAnimalRecord rec = new BiomeAnimalRecord();
                //rec.animal = animal;
                //rec.commonality = VanillaBiomesDefOf.ExtremeDesert.CommonalityOfAnimal(animal);
                ////sandbar.CommonalityOfAnimal(animal) = VanillaBiomesDefOf.ExtremeDesert.CommonalityOfAnimal(animal);

                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                newRecord1.commonality = VanillaBiomesDefOf.ExtremeDesert.CommonalityOfAnimal(animal);

                animal.RaceProps.wildBiomes.Add(newRecord1);
            }
        }



        private static void AddBiomesWildPlants()
        {
            //BiomeDef meadow = VanillaBiomesDefOf.ZBiome_AlpineMeadow;

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant != null)
                {
                    if (BiomeDefOf.TemperateForest.CommonalityOfPlant(current) > 0f && VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current) == 0f)
                    {
                        Log.Message("Found plant: " + current.defName);

                        try
                        {
                            PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                            //Log.Message("Made new record for: " + current.defName);
                            newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");

                            //newRecord1.commonality = BiomeDefOf.TemperateForest.CommonalityOfPlant(current);
                            //Log.Message("Plant: " + current.defName + " Commonality: " + BiomeDefOf.TemperateForest.CommonalityOfPlant(current));

                            newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                            Log.Message("Plant: " + current.defName + " Commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality);

                            Log.Message("Record: " + newRecord1.commonality);

                            //if(current.plant.wildBiomes == null)
                            //{
                            //    current.plant.wildBiomes = new List<PlantBiomeRecord>();
                            //}


                            current.plant.wildBiomes.Add(newRecord1);
                            Log.Message("Biome commonality: " + VanillaBiomesDefOf.ZBiome_AlpineMeadow.CommonalityOfPlant(current));
                            Log.Message("Plant biome record commonality: " + current.plant.wildBiomes.Where(bi => bi.biome.defName == "ZBiome_AlpineMeadow").FirstOrDefault().commonality);

                        }
                        catch (Exception ex)
                        {
                            Log.Message(ex.Message);
                        }
                    }
                }

            }
        }
        #endregion
    }
}
