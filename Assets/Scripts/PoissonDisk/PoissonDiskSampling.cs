using System.Collections.Generic;
using UnityEngine;

// algorithm based on: https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
// as implemented by Sebastian Lague: https://www.youtube.com/watch?v=7WcmyxyFO7o

namespace Poisson
{
    public static class PoissonDiskSampling
    {
        // Generate samples in a rectangular region
        public static List<Vector2> GenerateSamples2D(Vector2 rectRegion, float radius, int rejectionSamples = 30)
        {
            // diagonal length of each cell == radius to maintain around each point —
            // this guarantees that exclusion circle will cover entire cell regardless
            // of where the point is in the cell.
            float cellSize = radius / Mathf.Sqrt(2);

            // initialize 2D background grid based on cell size as array of integers;
            // cell value == 0 indicates no sample, cell value > 0 is sample index (1-based index)
            int[,] grid = new int[Mathf.CeilToInt(rectRegion.x / cellSize), Mathf.CeilToInt(rectRegion.y / cellSize)];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = -1;
                }
            }

            List<Vector2> points = new List<Vector2>();
            List<Vector2> generatorPoints = new List<Vector2>();

            // choose random point in region as starting sample
            float initSampleX = Random.Range(0, rectRegion.x);
            float initSampleY = Random.Range(0, rectRegion.y);

            Vector2 initSample = new Vector2(initSampleX, initSampleY);
            generatorPoints.Add(initSample);

            // add new points while there are points available to spawn from
            while (generatorPoints.Count > 0)
            {
                // randomly select new spawn point
                int genIndex = Random.Range(0, generatorPoints.Count);
                Vector2 genPoint = generatorPoints[genIndex];

                bool isValidPoint = false;

                // generate and check up to 30 new points from spawn point
                for (int i = 0; i < rejectionSamples; i++)
                {
                    // generate new point within spherical annulus between r and 2r of spawn point
                    float randAngle = Random.value * 2 * Mathf.PI;
                    Vector2 randDir = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                    Vector2 nextSample = genPoint + randDir * Random.Range(radius, 2 * radius);

                    // check validity of generated point
                    isValidPoint = IsValid2D(nextSample, rectRegion, cellSize, radius, points, grid);

                    if (isValidPoint)
                    {
                        // add new point to grid and point lists
                        points.Add(nextSample);
                        generatorPoints.Add(nextSample);
                        grid[(int)(nextSample.x / cellSize), (int)(nextSample.y / cellSize)] = points.Count - 1;
                        break;
                    }
                }

                // if we reject 30 points, remove the current point from the generator list
                if (!isValidPoint)
                    generatorPoints.RemoveAt(genIndex);
            }

            return points;
        }

        // Generate samples in a rectangular region with starting point seed(s)
        public static List<Vector2> GenerateSamples2D(Vector2 rectRegion, float radius, List<Vector2> seeds, int rejectionSamples = 30)
        {
            // diagonal length of each cell == radius to maintain around each point —
            // this guarantees that exclusion circle will cover entire cell regardless
            // of where the point is in the cell.
            float cellSize = radius / Mathf.Sqrt(2);

            // initialize 2D background grid based on cell size as array of integers;
            // cell value == 0 indicates no sample, cell value > 0 is sample index (1-based index)
            int[,] grid = new int[Mathf.CeilToInt(rectRegion.x / cellSize), Mathf.CeilToInt(rectRegion.y / cellSize)];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = -1;
                }
            }

            List<Vector2> points = new List<Vector2>();
            List<Vector2> generatorPoints = new List<Vector2>();

            // choose first point in seed as starting sample
            if (seeds.Count > 0)
            {
                if (IsValid2D(seeds[0], rectRegion, cellSize, radius, points, grid))
                {
                    Vector2 initSample = seeds[0];
                    points.Add(initSample);
                    generatorPoints.Add(initSample);
                    grid[(int)(initSample.x / cellSize), (int)(initSample.y / cellSize)] = points.Count - 1;
                }
                else
                {
                    Debug.LogError("The first seed is not in range.");
                    return new List<Vector2>();
                }
            }
            else
            {
                Debug.LogError("Seed cannot be null.");
                return new List<Vector2>();
            }

            // add remaining valid seed points to lists
            if (seeds.Count > 1)
            {
                foreach (var s in seeds.GetRange(1, seeds.Count - 1))
                {
                    if (IsValid2D(s, rectRegion, cellSize, radius, points, grid))
                    {
                        points.Add(s);
                        generatorPoints.Add(s);
                        grid[(int)(s.x / cellSize), (int)(s.y / cellSize)] = points.Count - 1;
                    }
                    else
                    {
                        Debug.LogError("A point in the list of seed points is not valid.");
                        return new List<Vector2>();
                    }
                }
            }

            // add new points while there are points available to spawn from
            while (generatorPoints.Count > 0)
            {
                // randomly select new spawn point
                int genIndex = Random.Range(0, generatorPoints.Count);
                Vector2 genPoint = generatorPoints[genIndex];

                bool isValidPoint = false;

                // generate and check up to 30 new points from spawn point
                for (int i = 0; i < rejectionSamples; i++)
                {
                    // generate new point within spherical annulus between r and 2r of spawn point
                    float randAngle = Random.value * 2 * Mathf.PI;
                    Vector2 randDir = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                    Vector2 nextSample = genPoint + randDir * Random.Range(radius, 2 * radius);

                    // check validity of generated point
                    isValidPoint = IsValid2D(nextSample, rectRegion, cellSize, radius, points, grid);

                    if (isValidPoint)
                    {
                        // add new point to grid and point lists
                        points.Add(nextSample);
                        generatorPoints.Add(nextSample);
                        grid[(int)(nextSample.x / cellSize), (int)(nextSample.y / cellSize)] = points.Count - 1;
                        break;
                    }
                }

                // if we reject 30 points, remove the current point from the generator list
                if (!isValidPoint)
                    generatorPoints.RemoveAt(genIndex);
            }

            return points;
        }

        // Validate samples in a rectangular region
        private static bool IsValid2D(Vector2 nextSample, Vector2 rectRegion, float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            // check if new point is in-bounds
            if (nextSample.x >= 0 && nextSample.x <= rectRegion.x &&
                nextSample.y >= 0 && nextSample.y <= rectRegion.y)
            {
                // check all cells within 2 cells of new point
                int nextX = (int)(nextSample.x / cellSize);
                int nextY = (int)(nextSample.y / cellSize);

                int startX = Mathf.Max(nextX - 2, 0);
                int startY = Mathf.Max(nextY - 2, 0);

                int endX = Mathf.Min(nextX + 2, grid.GetLength(0) - 1);
                int endY = Mathf.Min(nextY + 2, grid.GetLength(1) - 1);

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        int foundIndex = grid[x, y];
                        if (foundIndex >= 0)
                        {
                            // if there is a point found in a neighboring cell, verify it is outide the exclusion radius
                            float sqrDist = (nextSample - points[foundIndex]).sqrMagnitude;
                            
                            if (sqrDist < (radius * radius))
                                return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        
        // Generate samples in a circular region
        public static List<Vector2> GenerateSamples2D(float regionRadius, float radius, int rejectionSamples = 30)
        {
            // diagonal length of each cell == radius to maintain around each point —
            // this guarantees that exclusion circle will cover entire cell regardless
            // of where the point is in the cell.
            float cellSize = radius / Mathf.Sqrt(2);

            // initialize 2D background grid based on cell size as array of integers;
            // cell value == 0 indicates no sample, cell value > 0 is sample index (1-based index)
            int[,] grid = new int[Mathf.CeilToInt(2 * regionRadius / cellSize), Mathf.CeilToInt(2 * regionRadius / cellSize)];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = -1;
                }
            }

            List<Vector2> points = new List<Vector2>();
            List<Vector2> generatorPoints = new List<Vector2>();

            // choose random point in region as starting sample
            float randTheta = Random.value * 2 * Mathf.PI;
            float randRadius = Random.value * regionRadius;

            float initSampleX = regionRadius + randRadius * Mathf.Cos(randTheta);
            float initSampleY = regionRadius + randRadius * Mathf.Sin(randTheta);

            Vector2 initSample = new Vector2(initSampleX, initSampleY);
            generatorPoints.Add(initSample);

            // add new points while there are points available to spawn from
            while (generatorPoints.Count > 0)
            {
                // randomly select new spawn point
                int genIndex = Random.Range(0, generatorPoints.Count);
                Vector2 genPoint = generatorPoints[genIndex];

                bool isValidPoint = false;

                // generate and check up to 30 new points from spawn point
                for (int i = 0; i < rejectionSamples; i++)
                {
                    // generate new point within spherical annulus between r and 2r of spawn point
                    float randAngle = Random.value * 2 * Mathf.PI;
                    Vector2 randDir = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                    Vector2 nextSample = genPoint + randDir * Random.Range(radius, 2 * radius);

                    // check validity of generated point
                    isValidPoint = IsValid2D(nextSample, regionRadius, cellSize, radius, points, grid);

                    if (isValidPoint)
                    {
                        // add new point to grid and point lists
                        points.Add(nextSample);
                        generatorPoints.Add(nextSample);
                        grid[(int)(nextSample.x / cellSize), (int)(nextSample.y / cellSize)] = points.Count - 1;
                        break;
                    }
                }

                // if we reject 30 points, remove the current point from the generator list
                if (!isValidPoint)
                    generatorPoints.RemoveAt(genIndex);
            }

            return points;
        }

        // Generate samples in a circular region with starting seed(s)
        public static List<Vector2> GenerateSamples2D(float regionRadius, float radius, List<Vector2> seeds, int rejectionSamples = 30)
        {
            // diagonal length of each cell == radius to maintain around each point —
            // this guarantees that exclusion circle will cover entire cell regardless
            // of where the point is in the cell.
            float cellSize = radius / Mathf.Sqrt(2);

            // initialize 2D background grid based on cell size as array of integers;
            // cell value == 0 indicates no sample, cell value > 0 is sample index (1-based index)
            int[,] grid = new int[Mathf.CeilToInt(2 * regionRadius / cellSize), Mathf.CeilToInt(2 * regionRadius / cellSize)];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = -1;
                }
            }

            List<Vector2> points = new List<Vector2>();
            List<Vector2> generatorPoints = new List<Vector2>();

            // choose first point in seed as starting sample
            if (seeds.Count > 0)
            {
                if (IsValid2D(seeds[0], regionRadius, cellSize, radius, points, grid))
                {
                    Vector2 initSample = seeds[0];
                    points.Add(initSample);
                    generatorPoints.Add(initSample);
                    grid[(int)(initSample.x / cellSize), (int)(initSample.y / cellSize)] = points.Count - 1;
                }
                else
                {
                    Debug.LogError("The first seed is not in range.");
                    return new List<Vector2>();
                }
            }
            else
            {
                Debug.LogError("Seed cannot be null.");
                return new List<Vector2>();
            }

            // add remaining valid seed points to lists
            if (seeds.Count > 1)
            {
                foreach (var s in seeds.GetRange(1, seeds.Count - 1))
                {
                    if (IsValid2D(s, regionRadius, cellSize, radius, points, grid))
                    {
                        points.Add(s);
                        generatorPoints.Add(s);
                        grid[(int)(s.x / cellSize), (int)(s.y / cellSize)] = points.Count - 1;
                    }
                    else
                    {
                        Debug.LogError("A point in the list of seed points is not valid.");
                        return new List<Vector2>();
                    }
                }
            }

            // add new points while there are points available to spawn from
            while (generatorPoints.Count > 0)
            {
                // randomly select new spawn point
                int genIndex = Random.Range(0, generatorPoints.Count);
                Vector2 genPoint = generatorPoints[genIndex];

                bool isValidPoint = false;

                // generate and check up to 30 new points from spawn point
                for (int i = 0; i < rejectionSamples; i++)
                {
                    // generate new point within spherical annulus between r and 2r of spawn point
                    float randAngle = Random.value * 2 * Mathf.PI;
                    Vector2 randDir = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                    Vector2 nextSample = genPoint + randDir * Random.Range(radius, 2 * radius);

                    // check validity of generated point
                    isValidPoint = IsValid2D(nextSample, regionRadius, cellSize, radius, points, grid);

                    if (isValidPoint)
                    {
                        // add new point to grid and point lists
                        points.Add(nextSample);
                        generatorPoints.Add(nextSample);
                        grid[(int)(nextSample.x / cellSize), (int)(nextSample.y / cellSize)] = points.Count - 1;
                        break;
                    }
                }

                // if we reject 30 points, remove the current point from the generator list
                if (!isValidPoint)
                    generatorPoints.RemoveAt(genIndex);
            }

            return points;
        }

        // Validate samples in a circular region
        private static bool IsValid2D(Vector2 nextSample, float regionRadius, float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            Vector2 center = new Vector2(regionRadius, regionRadius);

            // check if new point is in-bounds
            if ((center - nextSample).magnitude <= regionRadius)
            {
                // check all cells within 2 cells of new point
                int nextX = (int)(nextSample.x / cellSize);
                int nextY = (int)(nextSample.y / cellSize);

                int startX = Mathf.Max(nextX - 2, 0);
                int startY = Mathf.Max(nextY - 2, 0);

                int endX = Mathf.Min(nextX + 2, grid.GetLength(0) - 1);
                int endY = Mathf.Min(nextY + 2, grid.GetLength(1) - 1);

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        int foundIndex = grid[x, y];
                        if (foundIndex >= 0)
                        {
                            // if there is a point found in a neighboring cell, verify it is outide the exclusion radius
                            float sqrDist = (nextSample - points[foundIndex]).sqrMagnitude;
                            
                            if (sqrDist < (radius * radius))
                                return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}