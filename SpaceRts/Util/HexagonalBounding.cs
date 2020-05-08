using System;
using Microsoft.Xna.Framework;
namespace SpaceRts.Util
{
    public class HexagonalBounding
    {
        private BoundingBox[] BoundingBoxes;

        public HexagonalBounding(Vector3[] corners)
        {
            BoundingBoxes = new BoundingBox[3];

            for (int i = 0; i < 3; i++)
            {
                BoundingBoxes[i / 2] = new BoundingBox(corners[i], corners[i + 3]);
            }
        }

        public float? Intersects(Ray ray)
        {
            float min = float.PositiveInfinity;

            for (int i = 0; i < 3; i++)
            {
                float? m = BoundingBoxes[i].Intersects(ray);
                if (m != null && m < min)
                    min = (float)m;
            }

            return min;
        }
    }
}
