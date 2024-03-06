namespace Si.Library.Exceptions
{
    public class SiNullException : SiExceptionBase
    {
        public SiNullException()
        {
        }

        public SiNullException(string message)
            : base($"Null exception: {message}.")
        {
        }
    }
}