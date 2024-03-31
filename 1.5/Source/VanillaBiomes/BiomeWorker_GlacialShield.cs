using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;
using Verse.Noise;

namespace VanillaBiomes
{
    public class BiomeWorker_GlacialShield : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (!BiomeSettings.spawnGlacialShield)
            {
                return -100f;
            }
            if (tile.WaterCovered)
			{
				return -100f;
			}

			if (tile.swampiness > 0f)
            {
				return -100f;
            }
			if(tile.temperature < -23f)
            {
				return -100f;
            }
			if (tile.temperature > -7f)
            {
				return -100f;
            }
			if(tile.rainfall < 200f)
            {
				return 0f;
            }

			float score = 1f - 1.1f * tile.temperature;
			score += ((float)tile.hilliness - 2.5f);
            return score;
		}
    }
}