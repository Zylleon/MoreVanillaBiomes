using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;


namespace VanillaBiomes
{
    public class BiomeWorker_AlpineMeadow : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            if(!BiomeSettings.spawnAlpineMeadow)
            {
                return -100f;
            }
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -7f || tile.temperature > 12f)
            {
                return 0f;
            }
            if (tile.rainfall < 600f)
            {
                return 0f;
            }
            if (tile.hilliness == Hilliness.Flat || tile.hilliness == Hilliness.SmallHills)
            {
                return 0;
            }

            float hills = 0;
            if (tile.hilliness == Hilliness.LargeHills)
            {
                hills = 2f;
            }

            return 11f - hills + (tile.elevation - 600f) / 80f;

        }
    }
}

