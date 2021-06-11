using System;

namespace Blocc.Engine.Common.Extensions
{
    public static class SpanExtensions
    {
        public static unsafe void* AsVoidPtr<T>(this Span<T> @this)
            where T : unmanaged
        {
            fixed (void* ptr = @this)
            {
                return ptr;
            }
        }

        public static unsafe T* AsPtr<T>(this Span<T> @this)
            where T : unmanaged
        {
            fixed (T* ptr = @this)
            {
                return ptr;
            }
        }
    }
}