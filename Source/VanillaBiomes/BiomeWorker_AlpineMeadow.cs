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
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -10f || tile.temperature > 15f)
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

            return 10f - hills + (tile.elevation - 600f) / 80f;

        }
    }
}

