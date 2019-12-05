using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;


namespace VanillaBiomes
{
    public class BiomeWorker_Bog : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            return 100;


        }
    }
}