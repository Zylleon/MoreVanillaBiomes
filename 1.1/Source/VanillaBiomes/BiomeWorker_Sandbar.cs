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
        public override float GetScore(Tile tile, int tileID)
        {
                return -100f;
        }
    }
}