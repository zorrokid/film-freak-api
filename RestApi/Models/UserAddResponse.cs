using Microsoft.AspNetCore.Identity;

public struct UserAddResponse
{
    public UserAddResponse(long id, IEnumerable<IdentityError> errors)
    {
        Id = id;
        Errors = errors;
    }

    public long Id { get; }
    public IEnumerable<IdentityError> Errors { get; }
}