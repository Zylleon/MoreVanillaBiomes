using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;


namespace VanillaBiomes
{
    public class BiomeSettings : ModSettings
    {
        public static bool spawnAlpineMeadow = true;
        public static bool spawnCoastalDunes = true;
        public static bool spawnDesertOasis = true;
        public static bool spawnGrasslands = true;
        public static bool spawnIceberg = true;
        public static bool spawnMarsh = true;
        public static bool spawnCloudForest = true;
        public static bool spawnBadlands = true;
        public static bool spawnGlacialShield = true;
        public static bool spawnModdedPlantsAnimals = true;

        public static bool spawnSandbar = true;



        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref spawnAlpineMeadow, "spawnAlpineMeadow", true);
            Scribe_Values.Look(ref spawnCoastalDunes, "spawnCoastalDunes", true);
            Scribe_Values.Look(ref spawnDesertOasis, "spawnDesertOasis", true);
            Scribe_Values.Look(ref spawnGrasslands, "spawnGrasslands", true);
            Scribe_Values.Look(ref spawnIceberg, "spawnIceberg", true);
            Scribe_Values.Look(ref spawnMarsh, "spawnMarsh", true);
            Scribe_Values.Look(ref spawnCloudForest, "spawnCloudForest", true);
            Scribe_Values.Look(ref spawnBadlands, "spawnBadlands", true);
            Scribe_Values.Look(ref spawnGlacialShield, "spawnGlacialShield", true);
            Scribe_Values.Look(ref spawnModdedPlantsAnimals, "spawnModdedPlantsAnimals", true);

            Scribe_Values.Look(ref spawnSandbar, "spawnSandbar", true);

        }
    }



    public class VanillaBiomesMod : Mod
    {
        BiomeSettings settings;

        public VanillaBiomesMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<BiomeSettings>();
            //LongEventHandler.ExecuteWhenFinished(PatchModdedLifeforms);
            PatchModdedLifeforms();
        }

        private void PatchModdedLifeforms()
        {
            if (BiomeSettings.spawnModdedPlantsAnimals)
            {
                AddAnimalsWildBiomes();
                AddPlantsWildBiomes();
            }
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {

            inRect.width = 450f;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("ZB_CheckboxMeadow".Translate(), ref BiomeSettings.spawnAlpineMeadow);
            listingStandard.CheckboxLabeled("ZB_CheckboxDunes".Translate(), ref BiomeSettings.spawnCoastalDunes);
            listingStandard.CheckboxLabeled("ZB_CheckboxOasis".Translate(), ref BiomeSettings.spawnDesertOasis);
            listingStandard.CheckboxLabeled("ZB_CheckboxGrasslands".Translate(), ref BiomeSettings.spawnGrasslands);
            listingStandard.CheckboxLabeled("ZB_CheckboxIceberg".Translate(), ref BiomeSettings.spawnIceberg);
            listingStandard.CheckboxLabeled("ZB_CheckboxMarsh".Translate(), ref BiomeSettings.spawnMarsh);
            listingStandard.CheckboxLabeled("ZB_CheckboxCloudForest".Translate(), ref BiomeSettings.spawnCloudForest);
            listingStandard.CheckboxLabeled("ZB_CheckboxSandbar".Translate(), ref BiomeSettings.spawnSandbar);
            listingStandard.CheckboxLabeled("ZB_CheckboxBadlands".Translate(), ref BiomeSettings.spawnBadlands);
            listingStandard.CheckboxLabeled("ZB_CheckboxGlacialShield".Translate(), ref BiomeSettings.spawnGlacialShield);


            listingStandard.GapLine();

            listingStandard.CheckboxLabeled("ZB_CheckboxModdedThings".Translate(), ref BiomeSettings.spawnModdedPlantsAnimals, "ZB_CheckboxModdedThingsDesc".Translate());


            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ZB_ModName".Translate();
        }


        #region modded plants and animals

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
                            try
                            {
                                PlantBiomeRecord newRecord1 = new PlantBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_Sandbar_NoBeach");
                                newRecord1.commonality = current.plant.wildBiomes.Where(bi => bi.biome.defName == "ExtremeDesert").FirstOrDefault().commonality;
                                current.plant.wildBiomes.Add(newRecord1);
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
                                }

                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Marsh", current.defName));
                                }
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
                if (current.RaceProps?.wildBiomes != null)
                {
                    // to check if it's in any of these biomes already
                    if (!current.RaceProps.wildBiomes.Any(w => w.biome.defName.Contains("ZBiome")))
                    {

                        //Dunes, Oasis
                        if (current.RaceProps.wildBiomes.Any(b => b.biome.defName == "Desert"))
                        {
                            try
                            {
                                AnimalBiomeRecord newRecord1 = new AnimalBiomeRecord();
                                newRecord1.biome = BiomeDef.Named("ZBiome_CoastalDunes");
                                newRecord1.commonality = current.RaceProps.wildBiomes.Where(bi => bi.biome.defName == "Desert").FirstOrDefault().commonality;
                                current.RaceProps.wildBiomes.Add(newRecord1);
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
                                }
                                catch
                                {
                                    Log.Message(String.Format("[More Vanilla Biomes] Error adding {0} to Marsh", current.defName));
                                }
                            }




                        }
                    }
                }
            }
        }


        #endregion







    }

}
