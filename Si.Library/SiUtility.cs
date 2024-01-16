using Si.Library.Exceptions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Si.Library
{
    public static class SiUtility
    {
        public delegate void TryAndIgnoreProc();
        public delegate T TryAndIgnoreProc<T>();

        /// <summary>
        /// We didnt need that exception! Did we?... DID WE?!
        /// </summary>
        public static void TryAndIgnore(TryAndIgnoreProc func)
        {
            try { func(); } catch { }
        }

        /// <summary>
        /// We didnt need that exception! Did we?... DID WE?!
        /// </summary>
        public static T? TryAndIgnore<T>(TryAndIgnoreProc<T> func)
        {
            try { return func(); } catch { }
            return default;
        }

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
                    throw new SiNullException($"Value should not be null: '{strName}'.");
                }
                else
                {
                    throw new SiNullException(message);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] Guid? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == null || value == Guid.Empty)
            {
                throw new SiNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new SiNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new SiNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [Conditional("DEBUG")]
        public static void AssertIfDebug(bool condition, string message)
        {
            if (condition)
            {
                throw new SiAssertException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string message)
        {
            if (condition)
            {
                throw new SiAssertException(message);
            }
        }
    }
}
