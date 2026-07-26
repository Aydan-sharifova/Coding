namespace Coding.Exceptions;

public sealed class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
