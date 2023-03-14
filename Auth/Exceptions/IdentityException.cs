using Microsoft.AspNetCore.Identity;

namespace FilmFreakApi.Auth.Exceptions;

public class IdentityException : Exception
{
    public IEnumerable<IdentityError> Errors { get; } = new List<IdentityError>();
    public IdentityException(String message) : base(message) { }

    public IdentityException(String message, IEnumerable<IdentityError> errors)
    : base(message)
    {
        Errors = errors;
    }
}