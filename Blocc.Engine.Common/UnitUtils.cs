using System;

namespace Blocc.Engine.Common
{
    public static class UnitUtils
    {
        public const double DegreeRadians = Math.PI / 180;
        public const double RadianDegrees = 180 / Math.PI;

        public const float DegreeRadiansF = MathF.PI / 180;
        public const float RadianDegreesF = 180 / MathF.PI;

        public static double ToRadians(double degrees) => degrees * DegreeRadians;
        public static double FromRadians(double radians) => radians * RadianDegrees;

        public static float ToRadiansF(float degrees) => degrees * DegreeRadiansF;
        public static float FromRadiansF(float radians) => radians * RadianDegreesF;
    }
}