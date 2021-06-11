namespace Blocc.Engine.Configuration
{
    public interface IOptions
    {
        T GetValue<T>(string entry);
    }

    public static class OptionsExtensions
    {
        public static bool TryGetValue<T>(this IOptions @this, string entry, out T value)
        {
            value = @this.GetValue<T>(entry);

            return value != null;
        }
    }
}