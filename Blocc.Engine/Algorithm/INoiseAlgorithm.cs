namespace Blocc.Engine.Algorithm
{
    public interface INoiseAlgorithm
    {
        int Seed { get; set; }

        float this[float x, float y] => At(x, y);
        float this[float x, float y, float z] => At(x, y, z);

        float At(float x, float y) => At(x, y, 0);
        float At(float x, float y, float z);
    }
}