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

        public static int GreaterOf(int one, int two) => (one > two) ? one : two;
        public static int LssserOf(int one, int two) => (one < two) ? one : two;

        public static uint GreaterOf(uint one, uint two) => (one > two) ? one : two;
        public static uint LssserOf(uint one, uint two) => (one < two) ? one : two;

        /// <summary>
        /// Executes the given delegate and returns true if successful.
        /// </summary>
        /// <param name="func"></param>
        /// <returns>Returns TRUE successful, returns FALSE if an error occurs.</returns>
        public static bool TryAndIgnore(TryAndIgnoreProc func)
        {
            try { func(); return true; } catch { return false; }
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
            => (new StackTrace())?.GetFrame(1)?.GetMethod()?.Name ?? "{unknown frame}";


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
