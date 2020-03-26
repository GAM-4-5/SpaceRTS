using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceRts;

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

            List<Vector2> points = GeneratePoints(minMuntainDistance, new Vector2(width, height), numSamplesBeforeAbort);

            for (int i = 0; i < points.Count; i++)
            {
                int cx = (int)points[i].X;
                int cy = (int)points[i].Y;

                int rnd = Random.Next(minMountainRadius, maxMountainRadius);
                int buildStartX = Math.Max(0, cx - rnd);
                int buildEndX = Math.Min(width, cx + rnd);
                int buildStartY = Math.Max(0, cy - rnd);
                int buildEndY = Math.Min(height, cy + rnd);

                for (int y = buildStartY; y < buildEndY; y++)
                {
                    for (int x = buildStartX; x < buildEndX; x++)
                    {
                        noiseMap[y * width + x] = 1;
                    }
                }
            }
        }

        public Mountains(object[] args) : this((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5], (int)args[6])
        {

        }

        private List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeAbort = 30)
        {
            float cellSize = (float)(radius / Math.Sqrt(2));

            int[,] grid = new int[(int)(Math.Ceiling(sampleRegionSize.X / cellSize)), (int)(Math.Ceiling(sampleRegionSize.Y / cellSize))];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>
            {
                sampleRegionSize / 2
            };

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Next(0, spawnPoints.Count);
                Vector2 spawnCentre = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeAbort; i++)
                {
                    float angle = (float)(Random.NextDouble() * Math.PI * 2);
                    Vector2 dir = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                    Vector2 candidate = spawnCentre + dir * Random.Next((int)radius, (int)(2 * radius));
                    if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.X / cellSize), (int)(candidate.Y / cellSize)] = points.Count;
                        candidateAccepted = true;
                        break;
                    }
                }

                if (!candidateAccepted)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }

            return points;
        }

        private bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            if (candidate.X >= 0 && candidate.X < sampleRegionSize.X && candidate.Y >= 0 && candidate.Y < sampleRegionSize.Y)
            {
                int cellX = (int)(candidate.X / cellSize);
                int cellY = (int)(candidate.Y / cellSize);
                int searchStartX = Math.Max(0, cellX - 2);
                int searchEndX = Math.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Math.Max(0, cellY - 2);
                int searchEndY = Math.Min(cellY + 2, grid.GetLength(1) - 1);

                for (int x = searchStartX; x <= searchEndX; x++)
                {
                    for (int y = searchStartY; y <= searchEndY; y++)
                    {
                        int pointIndex = grid[x, y] - 1;
                        if (pointIndex != -1)
                        {
                            float sqrDst = Vector2.DistanceSquared(candidate, points[pointIndex]);
                            if (sqrDst < radius * radius)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override float GenerateAtPosition(int x, int y)
        {
            return base.GenerateAtPosition(x, y);
        }
    }
}