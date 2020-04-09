using System;
using System.Collections.Generic;
using System.Text;
using SpaceRts;
using static SpaceRts.Planet;

namespace Map
{
    public class NoiseGenerator
    {
        private static float[] frequencyList = {
            5f,     //Magma
            5f,     //Desert
            5f,   //Rocky
            5f,     //Terran
            5f,     //Cold
            5f    //Gas // GASDLC
        };

        private static float[] amplitudesList =
        {
            0.9f,     //Magma
            0.9f,     //Desert
            0.9f,     //Rocky
            0.9f,     //Terran
            0.9f,   //Cold
            0.9f,    //Gas // GASDLC
        };

        private static int[] minMountainRadiusList =
        {
            10,      //Magma
            10,      //Desert
            40,      //Rocky
            20,      //Terran
            40,      //Cold
            40      //Gas // GASDLC
        };

        private static int[] maxMountainRadiusList =
        {
            100,     //Magma
            400,     //Desert
            800,     //Rocky
            600,     //Terran
            400,   //Cold
            400,    //Gas // GASDLC
        };

        private static int[] minMuntainDistanceList =
        {
            20,     //Magma
            20,     //Desert
            20,     //Rocky
            20,     //Terran
            20,   //Cold
            20,    //Gas // GASDLC
        };

        private static Type[][] noiseGenerators = {
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Magma
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Desert
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Rocky
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Terran
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Cold
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Gas 
        };

        public const float IREGUALRITY_FREQUENCY = 10f, IREGULARITY_AMPLITUDE = 10f;

        private SubNoiseGenerator[] SubNoiseGenerators;
        public PlanetTypes PlanetType;
        public int Seed, Width, Height;
        public float[] noiseMap;

        public float Min, Max;

        public float[][] noiseIregularityMap;
        public NoiseGenerator(PlanetTypes planetType, int seed, int width, int height)
        {
            PlanetType = planetType;
            Console.WriteLine(PlanetType);
            noiseMap = new float[width * height];
            Type[] requredNoiseGenerators = noiseGenerators[(int)planetType];
            SubNoiseGenerators = new SubNoiseGenerator[requredNoiseGenerators.Length];

            float frequency = frequencyList[(int)planetType];
            float amplitude = amplitudesList[(int)planetType];
            int minMountainRadius = minMountainRadiusList[(int)planetType];
            int maxMountainRadius = maxMountainRadiusList[(int)planetType];
            int minMuntainDistance = minMuntainDistanceList[(int)planetType];

            for (int i = 0; i < requredNoiseGenerators.Length; i++)
            {
                if(requredNoiseGenerators[i].Name == "Hills")
                    SubNoiseGenerators[i] = (SubNoiseGenerator)Activator.CreateInstance(requredNoiseGenerators[i], new object[] { seed, width, height, frequency, amplitude });
                else if(requredNoiseGenerators[i].Name == "Mountains")
                    SubNoiseGenerators[i] = (SubNoiseGenerator)Activator.CreateInstance(requredNoiseGenerators[i], new object[] { seed, width, height, minMountainRadius, maxMountainRadius, minMuntainDistance, 10 });
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = 0;
                    for (int i = 0; i < SubNoiseGenerators.Length; i++)
                    {
                        value += SubNoiseGenerators[i].GenerateAtPosition(x,y);
                    }
                    Min = Math.Min(Min, value);
                    Max = Math.Max(Max, value);
                    noiseMap[y * width + x] = value;
                }
            }

            Noise2d iregularityNoise = new Noise2d(seed);
            noiseIregularityMap = new float[6][];

            for (int i = 0; i < 6; i++)
            {
                noiseIregularityMap[i] = iregularityNoise.GenerateNoiseMap(width, height, IREGUALRITY_FREQUENCY, IREGULARITY_AMPLITUDE);
                iregularityNoise.Reseed();
            }
        }

        public float GenerateAtPosition(int x, int y)
        {
            return noiseMap[y * Width + x] / Max % 0.1f;
        }

        public float GenerateAtPosition(int i)
        {
            return (int)(noiseMap[i] * 4) / Max;
        }

        public float TryGenerateAtIndex(int i, out bool sucess)
        {
            if(i > 0 && i < noiseMap.Length)
            {
                sucess = true;
                return GenerateAtPosition(i);
            }

            sucess = false;
            return -100;
        }

        public float GenerateIregularityAtPosition(int i, int x)
        {
            return noiseIregularityMap[x % 6][i];
        }

    }
}
