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
    public class BiomeWorker_Badlands : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
			
			if (tile.WaterCovered)
			{
				return -100f;
			}
			if (tile.temperature < 5f)
			{
				return 0f;
			}
            if (tile.rainfall < 600f)
            {
                return 0f;
            }
            if (tile.swampiness > 0.0f)
			{
				return 0f;
			}
			if (tile.elevation < 700f || tile.elevation > 1100f)
            {
				return 0f;
            }
			

			float output = 16f + (tile.temperature - 7f) + (tile.rainfall - 600f) / 200f;
			if (tile.hilliness == Hilliness.Flat)
            {
				output -= 0.85f;
            }
			return output;

		}
    }
}