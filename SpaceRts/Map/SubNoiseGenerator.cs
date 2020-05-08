using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceRts;
using SpaceRts.Noise;

namespace Map
{
    public class SubNoiseGenerator
    {
        internal int Seed;
        internal int Width, Height;
        internal Random Random;
        internal float[] noiseMap;

        public SubNoiseGenerator(int seed, int width, int height)
        {
            Seed = seed;
            Width = width;
            Height = height;

            Random = new Random(seed);
        }

        public virtual float GenerateAtPosition(int x, int y)
        {
            return noiseMap[y * Width + x];
        }
    }


    public class Hills : SubNoiseGenerator
    {
        public Hills(int seed, int width, int height, float frequency, float amplitude) : base(seed, width, height)
        {
            Noise2d noise = new Noise2d(seed);
            noiseMap = noise.GenerateNoiseMap(width, height, frequency, amplitude);
        }

        public Hills(object[] args) : this((int)args[0], (int)args[1], (int)args[2], (float)args[3], (float)args[4])
        {

        }

        public override float GenerateAtPosition(int x, int y)
        {
            return base.GenerateAtPosition(x, y);
        }
    }

    public class Mountains : SubNoiseGenerator
    {
        public Mountains(int seed, int width, int height, int minMountainRadius, int maxMountainRadius, int minMuntainDistance, int numSamplesBeforeAbort) : base(seed, width, height)
        {
            noiseMap = new float[width * height];
            float side = minMountainRadius / (float)Math.Sqrt(2);

            List<Vector2> points = PoissonSample.GeneratePoints(Random,minMuntainDistance, new Vector2(width, height), numSamplesBeforeAbort);

            for (int i = 0; i < points.Count; i++)
            {
                int cx = (int)points[i].X;
                int cy = (int)points[i].Y;

                int rnd = Random.Next(minMountainRadius, maxMountainRadius);
                int buildStartX = Math.Max(0, cx - rnd);
                int buildEndX = Math.Min(width, cx + rnd);
                int buildStartY = Math.Max(0, cy - rnd);
                int buildEndY = Math.Min(height, cy + rnd);
                int center = Math.Min((buildStartX + buildEndX) / 2 - buildStartX, (buildStartY + buildEndY) / 2 - buildStartY);
                float MountainHeight = (float)Random.NextDouble();
                float h = 0f;
                for (int c = 0; c < center; c++)
                {
                    h += MountainHeight / center * 2;
                    
                    for (int y = buildStartY + c; y < buildEndY - c; y++)
                    {
                        for (int x = buildStartX + c; x < buildEndX - c; x++)
                        {
                            noiseMap[y * width + x] = h;
                        }
                    }
                }
            }
        }

        public Mountains(object[] args) : this((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5], (int)args[6])
        {
        
        }


        public override float GenerateAtPosition(int x, int y)
        {
            return base.GenerateAtPosition(x, y);
        }
    }
}