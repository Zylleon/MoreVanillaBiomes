using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;


namespace VanillaBiomes
{
    public class BiomeWorker_CloudForest : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            if (!BiomeSettings.spawnCloudForest)
            {
                return -100f;
            }
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < 10f)
            {
                return 0f;
            }
            if (tile.rainfall < 1600f)
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
                hills = 3f;
            }

            return 16f + tile.temperature - hills + (tile.elevation - 600f) / 60f;

        }
    }
}