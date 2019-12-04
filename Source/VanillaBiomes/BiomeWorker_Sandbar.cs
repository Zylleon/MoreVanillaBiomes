using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;

namespace VanillaBiomes
{
    public class BiomeWorker_Sandbar : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.elevation < -1)
            {
                return -100;
            }
            if (tile.temperature < -5)
            {
                return -100;
            }
            return 100;


        }
    }
}
