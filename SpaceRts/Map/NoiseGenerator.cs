using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SpaceRts;
using static SpaceRts.Planet;
using SpaceRts.Util;

namespace Map
{
    public class NoiseGenerator
    {
        private static float[] frequencyList = {
            2f,     //Magma
            3f,     //Desert
            5f,   //Rocky
            3f,     //Terran
            0.5f,     //Cold
            0.2f    //Gas // GASDLC
        };

        private static float[] amplitudesList =
        {
            3f,     //Magma
            2f,     //Desert
            5f,     //Rocky
            2f,     //Terran
            0.5f,   //Cold
            0.2f    //Gas // GASDLC
        };

        private static int[] minMountainRadiusList =
        {
            1,      //Magma
            1,      //Desert
            4,      //Rocky
            2,      //Terran
            2,      //Cold
            40      //Gas // GASDLC
        };

        private static int[] maxMountainRadiusList =
        {
            5,     //Magma
            4,     //Desert
            8,     //Rocky
            5,     //Terran
            4,   //Cold
            41    //Gas // GASDLC
        };

        private static int[] minMuntainDistanceList =
        {
            10,     //Magma
            15,     //Desert
            8,     //Rocky
            14,     //Terran
            15,   //Cold
            40    //Gas // GASDLC
        };

        private static Type[][] noiseGenerators = {
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Magma
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Desert
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Rocky
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Terran
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Cold
            new Type[] {typeof(Hills),  typeof(Mountains), },   //Gas 
        };

        // TODO extract elsewhere, adjust
        public enum BiomeType
        {
            // Magma
            LavaLake,
            BurningGround,
            Ashes,
            VolcanicMountains,

            // Desert
            Oasis,
            DryLand,
            DryPlains,
            DryMountains,

            // Rocky
            WaterLake,
            RockyBeach,
            SmallRocks,
            Rock,
            RockyMountains,


            // Terran
            Ocean,
            Shallows,
            Beach,
            Plains,
            Hills,
            Mountains,


            // Cold
            FrozenLake,
            FrozenLand,
            FrozenMountains,

            // Gas
            // DLC 
            Gas,

            END_VALUE
        }

        public static (float, BiomeType)[][] biomeValues = {
            new (float, BiomeType)[] {(0.0f, BiomeType.LavaLake), (0.2f, BiomeType.BurningGround), (0.6f, BiomeType.Ashes), (0.9f, BiomeType.VolcanicMountains) },   //Magma
            new (float, BiomeType)[] {(0.0f, BiomeType.Oasis), (0.15f, BiomeType.DryLand), (0.4f, BiomeType.DryPlains), (0.9f, BiomeType.DryMountains) },   //Desert
            new (float, BiomeType)[] {(0.0f, BiomeType.WaterLake), (0.1f, BiomeType.RockyBeach), (0.3f, BiomeType.SmallRocks),(0.5f, BiomeType.Rock), (0.9f, BiomeType.RockyMountains) },   //Rocky
            new (float, BiomeType)[] {(0.0f, BiomeType.Ocean), (0.30f, BiomeType.Shallows), (0.35f, BiomeType.Beach),(0.45f, BiomeType.Plains), (0.7f, BiomeType.Hills),(0.9f, BiomeType.Mountains) },   //Terran
            new (float, BiomeType)[] {(0.0f, BiomeType.FrozenLake), (0.3f, BiomeType.FrozenLand), (0.8f, BiomeType.FrozenMountains) },   //Cold
            new (float, BiomeType)[] {(0.0f, BiomeType.Gas)}, //Gas 
        };

        private static float[][] ConstructedBiomeValues = new float[biomeValues.Length][];

        public static Texture2D[] Textures = new Texture2D[(int)BiomeType.END_VALUE];

        public static void LoadTextures(ContentManager content)
        {
            Console.WriteLine("LOAD");
            for (int i = 0; i < biomeValues.Length; i++)
            {
                float[] constructedFloatValues = new float[biomeValues[i].Length];

                for (int j = 0; j < biomeValues[i].Length; j++)
                    constructedFloatValues[j] = biomeValues[i][j].Item1;

                ConstructedBiomeValues[i] = constructedFloatValues;
            }


            for (int i = 0; i < (int)BiomeType.END_VALUE; i++)
            {
                Textures[i] = content.Load<Texture2D>("Textures/" + ((BiomeType)i).ToString()); 
            }

        }

        public const float IREGUALRITY_FREQUENCY = 10f, IREGULARITY_AMPLITUDE = 10f;

        private SubNoiseGenerator[] SubNoiseGenerators;
        public PlanetTypes PlanetType;
        public int Seed, Width, Height;
        public float[] noiseMap;

        public float Min, Max;

        public float[][] noiseIregularityMap;

        public float[] biomeNoiseMap;
        public NoiseGenerator(PlanetTypes planetType, int seed, int width, int height)
        {
            Noise2d biomeNoise = new Noise2d(new Random(seed).Next());
            biomeNoiseMap = biomeNoise.GenerateNoiseMap(width, height, 10, 1); // TODO config per planet

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
            return -1;
        }

        public float GenerateIregularityAtPosition(int i, int x)
        {
            return noiseIregularityMap[x % 6][i];
        }

        public BiomeType BiomeAtLocation(int x, int y)
        {
            return BiomeAtIndex(y * Width + x);
        }

        public BiomeType BiomeAtIndex(int i)
        {
            float value = biomeNoiseMap[i];
            return (BiomeType)MathExtended.GradientIndex(value, ConstructedBiomeValues[(int)PlanetType]);
        }

    }
}
