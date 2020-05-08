using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceRts.Noise
{
    public static class PoissonSample
    {
        public static List<Vector2> GeneratePoints(Random random, float radius, Vector2 sampleRegionSize, int numSamplesBeforeAbort = 30)
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
                int spawnIndex = random.Next(0, spawnPoints.Count);
                Vector2 spawnCentre = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeAbort; i++)
                {
                    float angle = (float)(random.NextDouble() * Math.PI * 2);
                    Vector2 dir = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                    Vector2 candidate = spawnCentre + dir * random.Next((int)radius, (int)(2 * radius));
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

        public static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
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
    }
}
