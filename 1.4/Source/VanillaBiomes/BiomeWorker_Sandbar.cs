using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace VanillaBiomes
{
    public class BiomeWorker_Sandbar : BiomeWorker
    {
        //public override float GetScore(Tile tile, int tileID)
        //{
        //        return -100f;
        //}

        public override float GetScore(Tile tile, int tileID)
        {
            if (!BiomeSettings.spawnSandbar)
            {
                return -100f;
            }
            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.elevation < -15)
            {
                return -100;
            }
            if (tile.temperature < 0)
            {
                return -100;
            }
            if (Rand.Value < 0.85f)
            {
                return 0f;
            }
            return 100;
        }
    }

}