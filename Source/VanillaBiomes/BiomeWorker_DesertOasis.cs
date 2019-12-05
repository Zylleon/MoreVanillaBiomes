using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace VanillaBiomes
{
    public class BiomeWorker_DesertOasis : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -10f)
            {
                return 0f;
            }
            if (tile.rainfall > 500f)
            {
                return 0f;
            }
            if (Rand.Value < 0.997f)
            {
                return 0f;
            }

            return tile.temperature * 2f;
        }
    }
}
