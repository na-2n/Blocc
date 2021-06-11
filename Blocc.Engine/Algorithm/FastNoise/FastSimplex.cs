using System;

namespace Blocc.Engine.Algorithm.FastNoise
{
    public class FastSimplex : INoiseAlgorithm
    {
        private const float Sqrt3 = 1.7320508075688772935274463415059f;

        private const float F2 = 0.5f * (Sqrt3 - 1f);
        private const float F3 = 1f / 3f;
        private const float F4 = (2.23606797f - 1f) / 4f;

        private const float G2 = (3f - Sqrt3) / 6f;
        private const float G3 = 1f / 6f;
        private const float G33 = G3 * 3 - 1;
        private const float G4 = (5f - 2.23606797f) / 20f;

        private static ReadOnlySpan<byte> Simplex4D => new byte[]
        {
            0, 1, 2, 3, 0, 1, 3, 2, 0, 0, 0, 0, 0, 2, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 0,
            0, 2, 1, 3, 0, 0, 0, 0, 0, 3, 1, 2, 0, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 0, 3, 0, 0, 0, 0, 1, 3, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 0, 1, 2, 3, 1, 0,
            1, 0, 2, 3, 1, 0, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 3, 1, 0, 0, 0, 0, 2, 1, 3, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            2, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 1, 2, 3, 0, 2, 1, 0, 0, 0, 0, 3, 1, 2, 0,
            2, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 0, 2, 0, 0, 0, 0, 3, 2, 0, 1, 3, 2, 1, 0,
        };

        private readonly Gradient _gradient;
        private int _seed;

        public int Seed
        {
            get => _seed;
            set => _gradient.Seed = _seed = value;
        }

        public FastSimplex(int seed)
        {
            _seed = seed;
            _gradient = new Gradient(seed);
        }

        public float At(float x, float y)
        {
            var t = (x + y) * F2;
            var i = FastMath.FastFloor(x + t);
            var j = FastMath.FastFloor(y + t);

            t = (i + j) * G2;
            var X0 = i - t;
            var Y0 = j - t;

            var x0 = x - X0;
            var y0 = y - Y0;

            int i1;
            int j1;

            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }
            else
            {
                i1 = 0;
                j1 = 1;
            }

            var x1 = x0 - i1 + G2;
            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1 + 2 * G2;
            var y2 = y0 - 1 + 2 * G2;

            float n0;
            float n1;
            float n2;

            t = 0.5f - x0 * x0 - y0 * y0;

            if (t < 0)
            {
                n0 = 0;
            }
            else
            {
                t *= t;
                n0 = t * t * _gradient.At(i, j, x0, y0);
            }

            t = 0.5f - x1 * x1 - y1 * y1;

            if (t < 0)
            {
                n1 = 0;
            }
            else
            {
                t *= t;
                n1 = t * t * _gradient.At(i + i1, j + j1, x1, y1);
            }

            t = 0.5f - x2 * x2 - y2 * y2;

            if (t < 0)
            {
                n2 = 0;
            }
            else
            {
                t *= t;
                n2 = t * t * _gradient.At(i + 1, j + 1, x2, y2);
            }

            return 50 * (n0 + n1 + n2);
        }

        public float At(float x, float y, float z)
        {
            var t = (x + y + z) * F3;
            var i = FastMath.FastFloor(x + t);
            var j = FastMath.FastFloor(y + t);
            var k = FastMath.FastFloor(z + t);

            t = (i + j + k) * G3;

            var x0 = x - (i - t);
            var y0 = y - (j - t);
            var z0 = z - (k - t);

            int i1;
            int j1;
            int k1;

            int i2;
            int j2;
            int k2;

            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0;
                }
                else if (x0 >= z0)
                {
                    i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1;
                }
                else // x0 < z0
                {
                    i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1;
                }
            }
            else // x0 < y0
            {
                if (y0 < z0)
                {
                    i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1;
                }
                else if (x0 < z0)
                {
                    i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1;
                }
                else // x0 >= z0
                {
                    i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0;
                }
            }

            var x1 = x0 - i1 + G3;
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + F3;
            var y2 = y0 - j2 + F3;
            var z2 = z0 - k2 + F3;
            var x3 = x0 + G33;
            var y3 = y0 + G33;
            var z3 = z0 + G33;

            float n0;
            float n1;
            float n2;
            float n3;

            t = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t < 0)
            {
                n0 = 0;
            }
            else
            {
                t *= t;
                n0 = t * t * _gradient.At(i, j, k, x0, y0, z0);
            }

            t = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t < 0)
            {
                n1 = 0;
            }
            else
            {
                t *= t;
                n1 = t * t * _gradient.At(i + i1, j + j1, k + k1, x1, y1, z1);
            }

            t = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t < 0)
            {
                n2 = 0;
            }
            else
            {
                t *= t;
                n2 = t * t * _gradient.At(i + i2, j + j2, k + k2, x2, y2, z2);
            }

            t = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t < 0)
            {
                n3 = 0;
            }
            else
            {
                t *= t;
                n3 = t * t * _gradient.At(i + 1, j + 1, k + 1, x3, y3, z3);
            }

            return 32 * (n0 + n1 + n2 + n3);
        }

        public float At(float x, float y, float z, float w)
        {
            float n0;
            float n1;
            float n2;
            float n3;
            float n4;

            var t = (x + y + z + w) * F4;
            var i = FastMath.FastFloor(x + t);
            var j = FastMath.FastFloor(y + t);
            var k = FastMath.FastFloor(z + t);
            var l = FastMath.FastFloor(w + t);

            t = (i + j + k + l) * G4;
            var X0 = i - t;
            var Y0 = j - t;
            var Z0 = k - t;
            var W0 = l - t;
            var x0 = x - X0;
            var y0 = y - Y0;
            var z0 = z - Z0;
            var w0 = w - W0;

            var c = (x0 > y0) ? 32 : 0;
            c += (x0 > z0) ? 16 : 0;
            c += (y0 > z0) ? 8 : 0;
            c += (x0 > w0) ? 4 : 0;
            c += (y0 > w0) ? 2 : 0;
            c += (z0 > w0) ? 1 : 0;
            c <<= 2;

            var i1 = Simplex4D[c] >= 3 ? 1 : 0;
            var i2 = Simplex4D[c] >= 2 ? 1 : 0;
            var i3 = Simplex4D[c++] >= 1 ? 1 : 0;
            var j1 = Simplex4D[c] >= 3 ? 1 : 0;
            var j2 = Simplex4D[c] >= 2 ? 1 : 0;
            var j3 = Simplex4D[c++] >= 1 ? 1 : 0;
            var k1 = Simplex4D[c] >= 3 ? 1 : 0;
            var k2 = Simplex4D[c] >= 2 ? 1 : 0;
            var k3 = Simplex4D[c++] >= 1 ? 1 : 0;
            var l1 = Simplex4D[c] >= 3 ? 1 : 0;
            var l2 = Simplex4D[c] >= 2 ? 1 : 0;
            var l3 = Simplex4D[c] >= 1 ? 1 : 0;

            var x1 = x0 - i1 + G4;
            var y1 = y0 - j1 + G4;
            var z1 = z0 - k1 + G4;
            var w1 = w0 - l1 + G4;
            var x2 = x0 - i2 + 2 * G4;
            var y2 = y0 - j2 + 2 * G4;
            var z2 = z0 - k2 + 2 * G4;
            var w2 = w0 - l2 + 2 * G4;
            var x3 = x0 - i3 + 3 * G4;
            var y3 = y0 - j3 + 3 * G4;
            var z3 = z0 - k3 + 3 * G4;
            var w3 = w0 - l3 + 3 * G4;
            var x4 = x0 - 1 + 4 * G4;
            var y4 = y0 - 1 + 4 * G4;
            var z4 = z0 - 1 + 4 * G4;
            var w4 = w0 - 1 + 4 * G4;

            t = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t < 0)
            {
                n0 = 0;
            }
            else
            {
                t *= t;
                n0 = t * t * _gradient.At(i, j, k, l, x0, y0, z0, w0);
            }

            t = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t < 0)
            {
                n1 = 0;
            }
            else
            {
                t *= t;
                n1 = t * t * _gradient.At(i + i1, j + j1, k + k1, l + l1, x1, y1, z1, w1);
            }

            t = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t < 0)
            {
                n2 = 0;
            }
            else
            {
                t *= t;
                n2 = t * t * _gradient.At(i + i2, j + j2, k + k2, l + l2, x2, y2, z2, w2);
            }

            t = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t < 0)
            {
                n3 = 0;
            }
            else
            {
                t *= t;
                n3 = t * t * _gradient.At(i + i3, j + j3, k + k3, l + l3, x3, y3, z3, w3);
            }

            t = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t < 0)
            {
                n4 = 0;
            }
            else
            {
                t *= t;
                n4 = t * t * _gradient.At(i + 1, j + 1, k + 1, l + 1, x4, y4, z4, w4);
            }

            return 27 * (n0 + n1 + n2 + n3 + n4);
        }
    }
}