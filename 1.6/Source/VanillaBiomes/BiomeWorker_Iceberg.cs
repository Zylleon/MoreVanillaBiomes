using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Noise;

namespace VanillaBiomes
{
    public class BiomeWorker_Iceberg : BiomeWorker
    {


        private ModuleBase cachedSeaIceAllowedNoise;

        private int cachedSeaIceAllowedNoiseForSeed;

        public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
        {
            if (!BiomeSettings.spawnIceberg)
            {
                return -100f;
            }
            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (!this.AllowedAt(planetTile))
            {
                return -100f;
            }

            if (tile.temperature < -21.45f || tile.temperature > -12f)
            {
                return 0f;
            }

            if (Rand.Value < 0.93f)
            {
                return -100f;
            }

            if (tile.temperature < -20.5f)
            {
                return -21f + -tile.temperature * 2f;
            }

            if(tile.elevation > -75f)
            {
                return -1f - tile.temperature;
            }

            return -100;


        }

        private bool AllowedAt(int tile)
        {
            Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tile);
            Vector3 viewCenter = Find.WorldGrid.SurfaceViewCenter;
            float value = Vector3.Angle(viewCenter, tileCenter);
            float viewAngle = Find.WorldGrid.SurfaceViewAngle;
            float num = Mathf.Min(7.5f, viewAngle * 0.12f);
            float num2 = Mathf.InverseLerp(viewAngle - num, viewAngle, value);
            if (num2 <= 0)
            {
                return true;
            }
            if (this.cachedSeaIceAllowedNoise == null || this.cachedSeaIceAllowedNoiseForSeed != Find.World.info.Seed)
            {
                this.cachedSeaIceAllowedNoise = new Perlin(0.017000000923871994, 2.0, 0.5, 6, Find.World.info.Seed, QualityMode.Medium);
                this.cachedSeaIceAllowedNoiseForSeed = Find.World.info.Seed;
            }
            float headingFromTo = Find.WorldGrid.GetHeadingFromTo(viewCenter, tileCenter);
            float num3 = (float)this.cachedSeaIceAllowedNoise.GetValue((double)headingFromTo, 0.0, 0.0) * 0.5f + 0.5f;


            return num2 <= num3;

            //bool allow = num2 <= num3;

            //if (allow)
            //{
            //    Log.Message("Tile was allowed!");
            //    Log.Message("num2: " + num2);
            //    Log.Message("num3: " + num3);
            //}
            // return allow;


        }

    }




        /*
        public class BiomeWorker_Iceberg : BiomeWorker
        {
            public override float GetScore(Tile tile, int tileID)
            {
                if (!tile.WaterCovered)
                {
                    return -100f;
                }

                if (tile.elevation < -100)
                {
                    return -100;
                }

                if(tile.temperature > -10)
                {
                    return -100;
                }

                //Log.Message("Looking for neighbors");
                //List<int> tmpNeighbors = new List<int>();
                //List<Rot4> tmpOceanDirs = new List<Rot4>();

                //WorldGrid grid = Find.World.grid;
                //grid.GetTileNeighbors(tileID, tmpNeighbors);

                //Log.Message("Neighbors: " + tmpNeighbors.Count);
                ////for(int i = 0; i < tmpNeighbors.Count; i++)
                ////{

                ////}


                //int i = 0;
                //int count = tmpNeighbors.Count;

                //foreach (int otherTile in tmpNeighbors)
                //{
                //    Log.Message("Trying to find a tile with ID: " + otherTile);


                    // this is where it's breaking
                    //Tile tile2 = grid[otherTile];
                    //Log.Message("Foud the tile");



                    //if (tile2.elevation >= 0)
                    //{
                    //    Log.Message("Found a beach");
                    //    return -100;
                    //}
                    //i++;
                //}
                //while (i < count)
                //{

                //    Tile tile2 = grid[tmpNeighbors[i]];

                //    if (tile2.elevation >= 0)
                //    {
                //        Log.Message("Found a beach");
                //        return -100;
                //    }
                //    i++;
                //}

                return 100;

            }
        } */
    }
