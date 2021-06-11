using System;

namespace Blocc.Engine.Common.Extensions
{
    public static class ReadOnlySpanExtensions
    {
        public static unsafe void* AsVoidPtr<T>(this ReadOnlySpan<T> @this)
            where T : unmanaged
        {
            fixed (void* ptr = @this)
            {
                return ptr;
            }
        }

        public static unsafe T* AsPtr<T>(this ReadOnlySpan<T> @this)
            where T : unmanaged
        {
            fixed (T* ptr = @this)
            {
                return ptr;
            }
        }
    }
}