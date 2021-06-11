namespace Blocc.Engine.Algorithm.FastNoise
{
    public class FastPerlin : INoiseAlgorithm
    {
        private readonly Gradient _gradient;
        private int _seed;

        public Interp Interp { get; set; }

        public int Seed
        {
            get => _seed;
            set => _gradient.Seed = _seed = value;
        }

        public FastPerlin(int seed)
        {
            Interp = Interp.Quintic;

            _seed = seed;
            _gradient = new Gradient(seed);
        }

        public float At(float x, float y)
        {
            var x0 = FastMath.FastFloor(x);
            var y0 = FastMath.FastFloor(y);
            var x1 = x0 + 1;
            var y1 = y0 + 1;

            float xs;
            float ys;

            switch (Interp)
            {
                default:
                case Interp.Linear:
                    xs = x - x0;
                    ys = y - y0;
                    break;
                case Interp.Hermite:
                    xs = FastMath.InterpHermiteFunc(x - x0);
                    ys = FastMath.InterpHermiteFunc(y - y0);
                    break;
                case Interp.Quintic:
                    xs = FastMath.InterpQuinticFunc(x - x0);
                    ys = FastMath.InterpQuinticFunc(y - y0);
                    break;
            }

            var xd0 = x - x0;
            var yd0 = y - y0;
            var xd1 = xd0 - 1;
            var yd1 = yd0 - 1;

            var xf0 = FastMath.Lerp(_gradient.At(x0, y0, xd0, yd0), _gradient.At(x1, y0, xd1, yd0), xs);
            var xf1 = FastMath.Lerp(_gradient.At(x0, y1, xd0, yd1), _gradient.At(x1, y1, xd1, yd1), xs);

            return FastMath.Lerp(xf0, xf1, ys);
        }

        public float At(float x, float y, float z)
        {
            var x0 = FastMath.FastFloor(x);
            var y0 = FastMath.FastFloor(y);
            var z0 = FastMath.FastFloor(z);
            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            float xs;
            float ys;
            float zs;
            switch (Interp)
            {
                default:
                case Interp.Linear:
                    xs = x - x0;
                    ys = y - y0;
                    zs = z - z0;
                    break;
                case Interp.Hermite:
                    xs = FastMath.InterpHermiteFunc(x - x0);
                    ys = FastMath.InterpHermiteFunc(y - y0);
                    zs = FastMath.InterpHermiteFunc(z - z0);
                    break;
                case Interp.Quintic:
                    xs = FastMath.InterpQuinticFunc(x - x0);
                    ys = FastMath.InterpQuinticFunc(y - y0);
                    zs = FastMath.InterpQuinticFunc(z - z0);
                    break;
            }

            var xd0 = x - x0;
            var yd0 = y - y0;
            var zd0 = z - z0;
            var xd1 = xd0 - 1;
            var yd1 = yd0 - 1;
            var zd1 = zd0 - 1;

            var xf00 = FastMath.Lerp(
                _gradient.At(x0, y0, z0, xd0, yd0, zd0), _gradient.At(x1, y0, z0, xd1, yd0, zd0), xs);
            var xf10 = FastMath.Lerp(
                _gradient.At(x0, y1, z0, xd0, yd1, zd0), _gradient.At(x1, y1, z0, xd1, yd1, zd0), xs);
            var xf01 = FastMath.Lerp(
                _gradient.At(x0, y0, z1, xd0, yd0, zd1), _gradient.At(x1, y0, z1, xd1, yd0, zd1), xs);
            var xf11 = FastMath.Lerp(
                _gradient.At(x0, y1, z1, xd0, yd1, zd1), _gradient.At(x1, y1, z1, xd1, yd1, zd1), xs);

            var yf0 = FastMath.Lerp(xf00, xf10, ys);
            var yf1 = FastMath.Lerp(xf01, xf11, ys);

            return FastMath.Lerp(yf0, yf1, zs);
        }
    }
}