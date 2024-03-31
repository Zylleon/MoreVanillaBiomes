using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;

namespace VanillaBiomes
{
    public class BiomeWorker_CoastalDunes : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (!BiomeSettings.spawnCoastalDunes)
            {
                return -100f;
            }
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -5f)
            {
                return 0f;
            }
            if (tile.elevation > 200f)
            {
                return 0f;
            }
            if (tile.rainfall > 1800f)
            {
                return 0f;
            }


            float hills = -15f;

            switch (tile.hilliness)
            {
                case Hilliness.Flat:
                    hills = -14f;
                    break;
                case Hilliness.SmallHills:
                    hills = -17f;
                    break;
                case Hilliness.LargeHills:
                    hills = -20f;
                    break;
                default:
                    hills = -40f;
                    break;
            }

            return hills + (250 - tile.elevation) / 6f + 2f * (tile.temperature - 18f);

            // Mostly works, revert here if needed
            //return -15f + (200 - tile.elevation) / 5f + 1.5f * (tile.temperature - 15f);

        }
    }
}




