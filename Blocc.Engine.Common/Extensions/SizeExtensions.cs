using System.Drawing;

namespace Blocc.Engine.Common.Extensions
{
    public static class SizeExtensions
    {
        public static float GetAspectRatio(this Size @this)
            => (float)@this.Width / (float)@this.Height;
    }
}