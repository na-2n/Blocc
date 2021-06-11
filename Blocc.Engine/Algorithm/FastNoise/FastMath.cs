using System.Runtime.CompilerServices;

namespace Blocc.Engine.Algorithm.FastNoise
{
    public static class FastMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloor(float f) => f >= 0 ? (int)f : (int)f - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastRound(float f) => (f >= 0) ? (int)(f + 0.5f) : (int)(f - 0.5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t) => a + t * (b - a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InterpHermiteFunc(float t) => t * t * (3 - 2 * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InterpQuinticFunc(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CubicLerp(float a, float b, float c, float d, float t)
        {
            float p = (d - c) - (a - b);

            return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
        }
    }
}