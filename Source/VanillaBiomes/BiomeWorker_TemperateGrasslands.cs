using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;


namespace VanillaBiomes
{
    public class BiomeWorker_TemperateGrasslands : BiomeWorker
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

            if(tile.rainfall > 750)
            {
                return 0;
            }

            //return tile.temperature;
            return 15f + (tile.temperature - 12f) * 2.1f;// + (tile.rainfall - 600) / 150f;



            // below this line still works best
            //if (tile.rainfall < 600f || tile.rainfall >= 2000f)
            //{
            //    return 0f;
            //}

            //return 19.5f + (tile.temperature - 15f) * 1.5f + (tile.rainfall - 600) / 150f;
        }
    }
}
