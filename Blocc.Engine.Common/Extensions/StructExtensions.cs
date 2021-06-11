using System;
using System.Runtime.InteropServices;

namespace Blocc.Engine.Common.Extensions
{
    public static class StructExtensions
    {
        public static byte[] AsBytes<TStruct>(this TStruct @this)
            where TStruct : struct
        {
            var size = Marshal.SizeOf<TStruct>();
            var arr = new byte[size];

            MemoryMarshal.Write(arr, ref @this);

            return arr;
        }

        public static Span<TSpan> AsSpan<TSpan, TStruct>(this TStruct @this)
            where TSpan : struct
            where TStruct : struct
        {
            var arr = @this.AsBytes();

            return MemoryMarshal.Cast<byte, TSpan>(arr);
        }
    }
}