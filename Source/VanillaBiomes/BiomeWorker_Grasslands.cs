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

            if (tile.rainfall < 600f || tile.rainfall >= 2000f)
            {
                return 0f;
            }

            //return 19.5f + (tile.temperature - 15f) * 1.5f + (tile.rainfall - 600) / 150f;        //TESTED version A
            //return 30f + (tile.temperature - 15f) * 1.5f - (tile.rainfall - 900) / 150f;          // B
            //return 20f + (tile.temperature - 15f) * 1.5f - (tile.rainfall - 1200) / 150f;         // C
            //return 19f + (tile.temperature - 15f) * 1.5f + Math.Max(tile.rainfall - 600, 1200 - tile.rainfall) / 150f;     // D

            return 19.5f + (tile.temperature - 15f) * 1.5f + Math.Max(tile.rainfall - 600, 1200 - tile.rainfall) / 150f;    //E 


        }
    }
}
