namespace Si.Library.Exceptions
{
    public class SiExceptionBase : Exception
    {

        public SiExceptionBase()
        {
        }

        public SiExceptionBase(string? message)
            : base(message)

        {
        }
    }
}
