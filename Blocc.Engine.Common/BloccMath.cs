using System;

namespace Blocc.Engine.Common
{
    public static class BloccMath
    {
        // https://www.johndcook.com/blog/csharp_erf/
        public static float Erf(float x)
        {
            // constants
            var a1 = 0.254829592f;
            var a2 = -0.284496736f;
            var a3 = 1.421413741f;
            var a4 = -1.453152027f;
            var a5 = 1.061405429f;
            var p = 0.3275911f;

            // Save the sign of x
            var sign = 1f;

            if (x < 0)
            {
                sign = -1f;
            }

            x = MathF.Abs(x);

            // A&S formula 7.1.26
            var t = 1f / (1f + p * x);
            var y = 1f - ((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t * MathF.Exp(-x * x);

            return sign * y;
        }

        public static float ErfGradient(float x, float max, float curveFactor = 4)
        {
            var curviness = max / curveFactor;
            var offset = max / 2;

            return -Erf((x - offset) / curviness) / 2 + 0.5f;
        }
    }
}