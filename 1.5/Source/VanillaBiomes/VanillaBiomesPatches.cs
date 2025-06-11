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

            //These are now ONLY to tell me, personally, what to put in XML patches. The auto-patching no longer functions, and will not run unless these lines are uncommented.

            //AddAnimalsWildBiomes();
            //AddPlantsWildBiomes();
            

            Log.Message("More Vanilla Biomes initialized......");
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
        private static void AddPlantsWildBiomes()
        {

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (current.plant?.wildBiomes != null)
                {
                    string newPatch = String.Format("<xpath>/Defs/ThingDef[defName = \"{0}\"]/plant/wildBiomes</xpath>", current.defName);
                    newPatch = newPatch + "\n<value>";
                    bool toSay = false;

                    // to check if it's in any of these biomes already
                    if (!current.plant.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {
                        toSay = true;
                        //Sandbar
                        if (current.plant.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                        {
                            try
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Sandbar", current.defName));
                            }

                        }
                        else if (current.plant.wildBiomes.Any(b => b.biome.defName == "Desert"))
                        {
                            try
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                current.plant.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Sandbar", current.defName));
                            }
                        }

                        else if (current.plant.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                        {
                            try
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                                newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "Tundra").FirstOrDefault().commonality;
                                if (current.plant.IsTree)
                                {
                                    newRecord1.commonality *= 1.7f;
                                }
                                if (current.plant.purpose == PlantPurpose.Beauty || current.plant.purpose == PlantPurpose.Food)
                                {
                                    newRecord1.commonality *= 1.2f;
                                }
                                current.plant.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Coastal Dunes", current.defName));
                            }
                        }


                        for (int j = 0; j < current.plant.wildBiomes.Count; j++)
                        {

                            // icebergs
                            if (current.plant.wildBiomes[j].biome.defName == "SeaIce")
                            {
                                try
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Iceberg_NoBeach");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    current.plant.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Iceberg", current.defName));
                                }
                            }

                            //Meadow
                            if (current.plant.wildBiomes[j].biome.defName == "BorealForest")
                            {
                                try
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
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Alpine Meadow", current.defName));
                                }
                            }

                            // Grasslands
                            if (current.plant.wildBiomes[j].biome.defName == "AridShrubland")
                            {
                                try
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Grasslands");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    if (current.plant.IsTree)
                                    {
                                        newRecord1.commonality *= 0.4f;
                                    }
                                    current.plant.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Grasslands", current.defName));
                                }
                            }

                            // Dunes and Oasis
                            if (current.plant.wildBiomes[j].biome.defName == "AridShrubland")
                            {
                                if (current.plant.purpose == PlantPurpose.Food)
                                {
                                    try
                                    {
                                        PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                        newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                        newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord1);

                                        PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                        newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                        newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord2);

                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);
                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord2.biome.defName, newRecord2.commonality);

                                    }
                                    catch
                                    {
                                        Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Coastal Dunes & Desert Oasis", current.defName));
                                    }
                                }
                            }

                            if (current.plant.wildBiomes[j].biome.defName == "TropicalRainforest")
                            {
                                if (current.plant.purpose != PlantPurpose.Food)
                                {
                                    try
                                    {
                                        PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                        newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                        newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord1);

                                        PlantBiomeRecord newRecord2 = new PlantBiomeRecord();
                                        newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                        newRecord2.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord2);

                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);
                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord2.biome.defName, newRecord2.commonality);
                                    }
                                    catch
                                    {
                                        Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Coastal Dunes & Desert Oasis", current.defName));
                                    }
                                }

                                if (!current.plant.IsTree)
                                {
                                    try
                                    {
                                        PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                        newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                        newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord1);

                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);
                                    }
                                    catch
                                    {
                                        Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Cloud Forest", current.defName));
                                    }
                                }

                            }

                            if (current.plant.wildBiomes[j].biome.defName == "TemperateForest")
                            {
                                if (current.plant.IsTree)
                                {
                                    try
                                    {
                                        PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                        newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                        newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord1);
                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                    }
                                    catch
                                    {
                                        Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Cloud Forest", current.defName));
                                    }
                                }

                                // Badlands
                                if (!current.plant.wildBiomes.Any(b => b.biome.defName == "TemperateSwamp"))
                                {
                                    try
                                    {
                                        PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                        newRecord1.biome = BiomeDef.Named("ZBiome_Badlands");
                                        newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                        current.plant.wildBiomes.Add(newRecord1);
                                        newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                    }
                                    catch
                                    {
                                        Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Badlands", current.defName));
                                    }
                                }
                            }


                            //Marsh
                            if (current.plant.wildBiomes[j].biome.defName == "ColdBog")
                            {
                                try
                                {
                                    PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Marsh");
                                    newRecord1.commonality = current.plant.wildBiomes[j].commonality;
                                    if (current.plant.IsTree)
                                    {
                                        newRecord1.commonality *= 0.3f;
                                    }
                                    current.plant.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }

                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Marsh", current.defName));
                                }
                            }

                        }
                    }

                    if (toSay)
                    {
                        Log.Message(newPatch);
                    }
                }
            }
        }


        // adapted from RF-Archipelagos
        private static void AddAnimalsWildBiomes()
        {
            Log.Message("Adding animals.......");
            Log.Message("Found animals: " + DefDatabase<PawnKindDef>.AllDefs.Count());
   

            foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
            {
                string newPatch = String.Format("<xpath>/Defs/ThingDef[defName = \"{0}\"]/race/wildBiomes</xpath>", current.defName);
                newPatch = newPatch + "\n<value>";
                bool toSay = false;
                //Log.Message(current.defName + " 1");
                if (current.RaceProps?.wildBiomes != null)
                {
                    //Log.Message(current.defName + " 2");
                    // to check if it's in any of these biomes already
                    if (!current.RaceProps.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {
                        toSay = true;
                        Log.Message(current.defName);
                        //Dunes, Oasis
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Desert"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);
                                //Log.Message(String.Format("<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality));
                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Coastal Dunes", current.defName));
                            }
                            try
                            {
                                AnimalBiomeRecord newRecord2 = new AnimalBiomeRecord();
                                newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                newRecord2.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord2);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord2.biome.defName, newRecord2.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Desert Oasis", current.defName));
                            }
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "ExtremeDesert"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Coastal Dunes", current.defName));
                            }
                            try
                            {
                                AnimalBiomeRecord newRecord2 = new AnimalBiomeRecord();
                                newRecord2.biome = BiomeDef.Named("ZBiome_DesertOasis");
                                newRecord2.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord2);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord2.biome.defName, newRecord2.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Desert Oasis", current.defName));
                            }

                        }

                        //Cloud Forest
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TropicalRainforest"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TropicalRainforest").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Cloud Forest", current.defName));
                            }
                        }
                        else if (current.RaceProps.baseBodySize <= 0.5f && current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_CloudForest");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Cloud Forest", current.defName));
                            }
                        }

                        //Badlands
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                        {
                            if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateSwamp"))
                            {
                                try
                                {
                                    AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Badlands");
                                    newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "TemperateForest").FirstOrDefault().commonality;
                                    if (!current.RaceProps.predator && current.RaceProps.baseBodySize > 1.0)    // less large herbivores
                                    {
                                        newRecord1.commonality *= 0.5f;
                                    }
                                    current.RaceProps.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Badlands", current.defName));
                                }
                            }
                            else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "AridShrubland"))
                            {
                                try
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
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Badlands", current.defName));
                                }
                            }
                        }

                        // Glacial Shield
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Tundra"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Tundra").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Glacial Shield", current.defName));
                            }
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "IceSheet"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "IceSheet").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
                                newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Glacial Shield", current.defName));
                            }
                        }
                        else if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "BorealForest"))
                        {
                            try
                            {
                                if (!current.RaceProps.wildBiomes.Any(b => b.biome.defName == "TemperateForest"))
                                {
                                    AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_GlacialShield");
                                    newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "BorealForest").FirstOrDefault().commonality;
                                    newRecord1.commonality *= 0.5f;
                                    current.RaceProps.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                            }
                            catch
                            {
                                Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Glacial Shield", current.defName));
                            }
                        }


                        for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
                        {
                            //Iceberg
                            if (current.RaceProps.wildBiomes[j].biome.defName == "SeaIce")
                            {
                                try
                                {
                                    AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Iceberg_NoBeach");
                                    newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                    current.RaceProps.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Iceberg", current.defName));
                                }
                            }

                            //Meadow
                            if (current.RaceProps.wildBiomes[j].biome.defName == "BorealForest")
                            {
                                try
                                {
                                    AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_AlpineMeadow");
                                    newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                    if (current.RaceProps.predator && current.RaceProps.maxPreyBodySize >= 0.9f)
                                    {
                                        newRecord1.commonality *= 0.5f;
                                    }
                                    current.RaceProps.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Alpine Meadow", current.defName));
                                }
                            }

                            //Grasslands
                            if (current.RaceProps.wildBiomes[j].biome.defName == "AridShrubland")
                            {
                                try
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
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Grasslands", current.defName));
                                }
                            }


                            //Sandbar
                            if (current.RaceProps.wildBiomes[j].biome.defName == "ExtremeDesert")
                            {
                                try
                                {
                                    AnimalBiomeRecord newRecord3 = new AnimalBiomeRecord();
                                    newRecord3.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                    newRecord3.commonality = current.RaceProps.wildBiomes[j].commonality;
                                    current.RaceProps.wildBiomes.Add(newRecord3);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord3.biome.defName, newRecord3.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Sandbar", current.defName));
                                }
                            }

                            //Marsh
                            if (current.RaceProps.wildBiomes[j].biome.defName == "ColdBog")
                            {
                                try
                                {
                                    AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                    newRecord1.biome = BiomeDef.Named("ZBiome_Marsh");
                                    newRecord1.commonality = current.RaceProps.wildBiomes[j].commonality;
                                    current.RaceProps.wildBiomes.Add(newRecord1);
                                    newPatch = newPatch + String.Format("\n<{0}>{1}</{0}>", newRecord1.biome.defName, newRecord1.commonality);

                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Marsh", current.defName));
                                }
                            }




                        }
                    }
                }

               if(toSay)
                {
                    Log.Message(newPatch);
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
