using System;
namespace SpaceRts.Util
{
    public static class MathExtended
    {
        /// <summary>
        /// Finds 0 based index of value inside of gradient.
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="value">Value to search.</param>
        /// <param name="gradient">Gradient - 0 to 1 float array.</param>
        public static int GradientIndex(float value, float[] gradient)
        {
            int r = 0;

            for (int i = 0; i < gradient.Length; i++)
                if (value > gradient[i])
                    r = i;

            return r;
        }
    }
}
