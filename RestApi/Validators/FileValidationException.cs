using System.Runtime.Serialization;

namespace FilmFreakApi.RestApi.Validators;

[Serializable]
internal class FileValidationException : Exception
{
    public FileValidationException()
    {
    }

    public FileValidationException(string? message) : base(message)
    {
    }

    public FileValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected FileValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}