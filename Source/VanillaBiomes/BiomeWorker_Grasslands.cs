using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;


namespace VanillaBiomes
{
    public class BiomeWorker_Grasslands : BiomeWorker
    {
        public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
        {
            if (!BiomeSettings.spawnGrasslands)
            {
                return -100f;
            }
            if (tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.temperature < 10f || tile.temperature > 25f)
            {
                return 0f;
            }

            if (tile.rainfall < 500f || tile.rainfall >= 1800f)
            {
                return 0f;
            }
            if(tile.rainfall < 900f)
            {
                return tile.temperature + 1f;
            }
            
            return 19.5f + (tile.temperature - 15f) * 1.5f + Math.Max(tile.rainfall - 600, 1200 - tile.rainfall) / 150f;    //E 

        }
    }
}
