using System;
namespace SpaceRts
{
    public class Cube
    {
        public static Cube[] Neightbours = {
            new Cube(+1, -1, 0), new Cube(+1, 0, -1), new Cube(0, +1, -1),
            new Cube(-1, +1, 0), new Cube(-1, 0, +1), new Cube(0, -1, +1)
        };

        public int X, Y, Z;

        public Cube(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Cube(int q, int r) : this(q, -q - r, r)
        {

        }

        public Cube(Axial axial) : this(axial.Q, axial.R)
        {

        }

        public Axial ToAxial()
        {
            return new Axial(X, Z);
        }

        public OddQ ToOddQ()
        {
            return new OddQ(this);
        }

        public static Cube operator +(Cube a, Cube b)
        {
            return new Cube(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Cube operator -(Cube a, Cube b)
        {
            return new Cube(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Cube operator *(Cube a, int scale)
        {
            return new Cube(a.X * scale, a.Y * scale, a.Z * scale);
        }
    }

    public class Axial
    {
        public static Axial[] Neightbours = {
            new Axial(+1, 0), new Axial(+1, -1), new Axial(0, -1),
            new Axial(-1, 0), new Axial(-1, +1), new Axial(0, +1)
        };

        public int Q, R;

        public Axial(int q, int r)
        {
            Q = q;
            R = r;
        }

        public Axial(Cube cube) : this(cube.X, cube.Z)
        {

        }

        public Cube ToCube()
        {
            return new Cube(Q, R);
        }

        public static Axial operator +(Axial a, Axial b)
        {
            return new Axial(a.Q + b.Q, a.R + b.R);
        }

        public static Axial operator -(Axial a, Axial b)
        {
            return new Axial(a.Q - b.Q, a.R - b.R);
        }

        public static Axial operator *(Axial a, int scale)
        {
            return new Axial(a.Q * scale, a.R * scale);
        }
    }

    public class OddQ
    {
        public int C, R;

        public OddQ(int c, int r)
        {
            C = c;
            R = r;
        }

        public OddQ(Cube cube) : this(cube.X, cube.Z + (cube.X - (cube.X & 1)) / 2)
        {

        }
    }
}
