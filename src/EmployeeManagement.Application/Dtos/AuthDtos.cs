using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Dtos;

public record RegisterDto(
    [property: Required][property: EmailAddress] string Email,
    [property: Required][property: MinLength(8)] string Password);

public record LoginDto(
    [property: Required][property: EmailAddress] string Email,
    [property: Required] string Password);

public record AuthResponseDto(string Token, DateTime ExpiresAt);

public record AuthResult(AuthResponseDto? Token, IReadOnlyList<string> Errors)
{
    public bool Succeeded => Token is not null;

    public static AuthResult Success(AuthResponseDto token) =>
        new(token, Array.Empty<string>());

    public static AuthResult Failure(IEnumerable<string> errors) =>
        new(null, errors.ToList());
}
