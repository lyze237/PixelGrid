using Microsoft.AspNetCore.Identity;

namespace PixelGrid.Server.Infra.Exceptions;

public class CreateUserException(string? message, IEnumerable<IdentityError> resultErrors) : Exception(message)
{
    public List<IdentityError> Errors { get; } = resultErrors.ToList();
}