﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;

namespace VanillaBiomes
{
    public class BiomeWorker_Test : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            return 100;


        }
    }
}
