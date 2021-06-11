using System.Collections.Generic;
using Blocc.Engine.Configuration;
using Silk.NET.Input.Common;

namespace Blocc.Desktop.Configuration
{
    public class DesktopOptions : IOptions
    {
        private static readonly Dictionary<string, object> Temp = new Dictionary<string, object>
        {
            ["Forward"] = Key.W,
            ["Left"] = Key.A,
            ["Backward"] = Key.S,
            ["Right"] = Key.D,
            ["Jump"] = Key.Space,
            ["Crouch"] = Key.ControlLeft,
            ["Boost"] = Key.ShiftLeft,
        };

        public T GetValue<T>(string entry)
            => (T)Temp.GetValueOrDefault(entry, null);
    }
}