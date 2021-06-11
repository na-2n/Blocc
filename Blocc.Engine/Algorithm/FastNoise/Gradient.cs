using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Blocc.Engine.Algorithm.FastNoise
{
    public class Gradient
    {
        private const int PrimeX = 1619;
        private const int PrimeY = 31337;
        private const int PrimeZ = 6971;
        private const int PrimeW = 1013;

        public static ReadOnlySpan<Vector2> Gradient2D => new[]
        {
            new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1),
            new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0),
        };

        public static ReadOnlySpan<Vector3> Gradient3D => new[]
        {
            new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0),
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1),
            new Vector3(0, 1, 1), new Vector3(0, -1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, -1),
            new Vector3(1, 1, 0), new Vector3(0, -1, 1), new Vector3(-1, 1, 0), new Vector3(0, -1, -1),
        };

        public int Seed { get; set; }

        public Gradient(int seed)
        {
            Seed = seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float At(int x, int y, float xd, float yd)
        {
            var hash = Seed;
            hash ^= PrimeX * x;
            hash ^= PrimeY * y;

            hash = hash * hash * hash * 60493;
            hash = (hash >> 13) ^ hash;

            var g = Gradient2D[hash & 7];

            return xd * g.X + yd * g.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float At(int x, int y, int z, float xd, float yd, float zd)
        {
            var hash = Seed;
            hash ^= PrimeX * x;
            hash ^= PrimeY * y;
            hash ^= PrimeZ * z;

            hash = hash * hash * hash * 60493;
            hash = (hash >> 13) ^ hash;

            var g = Gradient3D[hash & 15];

            return xd * g.X + yd * g.Y + zd * g.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float At(int x, int y, int z, int w, float xd, float yd, float zd, float wd)
        {
            var hash = Seed;
            hash ^= PrimeX * x;
            hash ^= PrimeY * y;
            hash ^= PrimeZ * z;
            hash ^= PrimeW * w;

            hash = hash * hash * hash * 60493;
            hash = (hash >> 13) ^ hash;

            hash &= 31;
            var a = yd;
            var b = zd;
            var c = wd;

            switch (hash >> 3)
            { // OR, DEPENDING ON HIGH ORDER 2 BITS:
                case 1: // W,X,Y
                    a = wd;
                    b = xd;
                    c = yd;
                    break;
                case 2: // Z,W,X
                    a = zd;
                    b = wd;
                    c = xd;
                    break;
                case 3: // Y,Z,W
                    a = yd;
                    b = zd;
                    c = wd;
                    break;
            }
            return ((hash & 4) == 0 ? -a : a) + ((hash & 2) == 0 ? -b : b) + ((hash & 1) == 0 ? -c : c);
        }
    }
}