using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace VanillaBiomes
{
    public class BiomeWorker_Marsh : BiomeWorker
    {
        public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
        {
            if (!BiomeSettings.spawnMarsh)
            {
                return -100f;
            }
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < -10f || tile.temperature > 14f)
            {
                return 0f;
            }
            if (tile.rainfall < 600f)
            {
                return 0f;
            }
            if (tile.swampiness < 0.5f)
            {
                return 0f;
            }

            return 11f + tile.swampiness * 5f + tile.temperature;

        }
    }
}