using EmployeeManagement.Application.Dtos;

namespace EmployeeManagement.Application.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
}
