using EmployeeManagement.Infrastructure.Identity;

namespace EmployeeManagement.Infrastructure.Auth;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}
