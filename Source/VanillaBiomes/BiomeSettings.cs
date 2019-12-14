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
            Scribe_Values.Look(ref spawnSandbar, "spawnSandbar", true);

        }
    }



    public class VanillaBiomesMod : Mod
    {
        BiomeSettings settings;

        public VanillaBiomesMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<BiomeSettings>();
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
            listingStandard.CheckboxLabeled("ZB_CheckboxSandbar".Translate(), ref BiomeSettings.spawnSandbar);

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ZB_ModName".Translate();
        }

    }


    

}
