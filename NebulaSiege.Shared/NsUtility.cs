using NebulaSiege.Shared.Exceptions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NebulaSiege.Shared
{
    public static class NsUtility
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            return (new StackTrace())?.GetFrame(1)?.GetMethod()?.Name ?? "{unknown frame}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNull<T>([NotNull] T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new NsNullException($"Value should not be null: '{strName}'.");
                }
                else
                {
                    throw new NsNullException(message);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] Guid? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == null || value == Guid.Empty)
            {
                throw new NsNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new NsNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NsNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [Conditional("DEBUG")]
        public static void AssertIfDebug(bool condition, string message)
        {
            if (condition)
            {
                throw new NsAssertException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string message)
        {
            if (condition)
            {
                throw new NsAssertException(message);
            }
        }
    }
}
