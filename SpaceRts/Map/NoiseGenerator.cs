using System;
using System.Collections.Generic;
using System.Text;
using SpaceRts;
using static SpaceRts.Planet;

namespace Map
{
    class NoiseGenerator
    {
        public PlanetTypes PlanetType;

        private static int[] frequencyList = {

        }
        private SubNoiseGenerator[];

        private float[] noiseMap;
        public NoiseGenerator(PlanetTypes planetType, int seed, int width, int height)
        {
            PlanetType = planetType;


            Noise2d noise = new Noise2d(seed);
            noiseMap = noise.GenerateNoiseMap(width, height, );
        }
    }
}
