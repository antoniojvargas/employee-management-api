# Guía para recrear el proyecto desde cero

Pasos en orden para recrear `employee-management-api` manualmente en cualquier carpeta.

---

## Prerrequisitos

- .NET SDK 10 instalado (`dotnet --version` → `10.x.x`)
- Docker con `docker compose`
- EF Core CLI: `dotnet tool install --global dotnet-ef`

---

## 1. Crear la carpeta y la solución

```bash
mkdir employee-management-api
cd employee-management-api
dotnet new sln -n EmployeeManagement
```

> Esto genera `EmployeeManagement.slnx` (formato del SDK 10).

---

## 2. Crear los cuatro proyectos

```bash
dotnet new classlib -n EmployeeManagement.Domain      -o src/EmployeeManagement.Domain      --framework net10.0
dotnet new classlib -n EmployeeManagement.Application -o src/EmployeeManagement.Application --framework net10.0
dotnet new classlib -n EmployeeManagement.Infrastructure -o src/EmployeeManagement.Infrastructure --framework net10.0
dotnet new webapi   -n EmployeeManagement.Api         -o src/EmployeeManagement.Api         --framework net10.0 --no-openapi
dotnet new xunit    -n EmployeeManagement.Tests       -o tests/EmployeeManagement.Tests     --framework net10.0
```

---

## 3. Agregar proyectos a la solución

```bash
dotnet sln add src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj
dotnet sln add src/EmployeeManagement.Application/EmployeeManagement.Application.csproj
dotnet sln add src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj
dotnet sln add src/EmployeeManagement.Api/EmployeeManagement.Api.csproj
dotnet sln add tests/EmployeeManagement.Tests/EmployeeManagement.Tests.csproj
```

---

## 4. Configurar referencias entre proyectos

```bash
dotnet add src/EmployeeManagement.Application      reference src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj
dotnet add src/EmployeeManagement.Infrastructure   reference src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj
dotnet add src/EmployeeManagement.Infrastructure   reference src/EmployeeManagement.Application/EmployeeManagement.Application.csproj
dotnet add src/EmployeeManagement.Api              reference src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj
dotnet add src/EmployeeManagement.Api              reference src/EmployeeManagement.Application/EmployeeManagement.Application.csproj
dotnet add src/EmployeeManagement.Api              reference src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj
dotnet add tests/EmployeeManagement.Tests          reference src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj
dotnet add tests/EmployeeManagement.Tests          reference src/EmployeeManagement.Application/EmployeeManagement.Application.csproj
dotnet add tests/EmployeeManagement.Tests          reference src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj
```

---

## 5. Instalar paquetes NuGet

### Infrastructure

```bash
dotnet add src/EmployeeManagement.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add src/EmployeeManagement.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/EmployeeManagement.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/EmployeeManagement.Infrastructure package Microsoft.IdentityModel.Tokens
dotnet add src/EmployeeManagement.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/EmployeeManagement.Infrastructure package System.IdentityModel.Tokens.Jwt
```

Editar `src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj` y agregar un `<ItemGroup>` separado:

```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

> Obligatorio para usar `AddIdentityCore`, `AddJwtBearer`, etc. desde una class library.

### Api

```bash
dotnet add src/EmployeeManagement.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/EmployeeManagement.Api package Microsoft.AspNetCore.OpenApi
dotnet add src/EmployeeManagement.Api package Microsoft.EntityFrameworkCore.Design
dotnet add src/EmployeeManagement.Api package Swashbuckle.AspNetCore
```

### Tests

```bash
dotnet add tests/EmployeeManagement.Tests package Moq
```

---

## 6. Eliminar archivos autogenerados innecesarios

```bash
rm src/EmployeeManagement.Domain/Class1.cs
rm src/EmployeeManagement.Application/Class1.cs
rm src/EmployeeManagement.Infrastructure/Class1.cs
rm tests/EmployeeManagement.Tests/UnitTest1.cs
rm src/EmployeeManagement.Api/WeatherForecast.cs
rm src/EmployeeManagement.Api/Controllers/WeatherForecastController.cs
```

---

## 7. Crear carpetas

```bash
mkdir -p src/EmployeeManagement.Domain/Entities
mkdir -p src/EmployeeManagement.Domain/Enums
mkdir -p src/EmployeeManagement.Application/Bonuses
mkdir -p src/EmployeeManagement.Application/Constants
mkdir -p src/EmployeeManagement.Application/Dtos
mkdir -p src/EmployeeManagement.Application/Repositories
mkdir -p src/EmployeeManagement.Application/Services
mkdir -p src/EmployeeManagement.Infrastructure/Auth
mkdir -p src/EmployeeManagement.Infrastructure/DependencyInjection
mkdir -p src/EmployeeManagement.Infrastructure/Identity
mkdir -p src/EmployeeManagement.Infrastructure/Persistence/Configurations
mkdir -p src/EmployeeManagement.Infrastructure/Repositories
mkdir -p src/EmployeeManagement.Api/Controllers
mkdir -p src/EmployeeManagement.Api/Middleware
mkdir -p tests/EmployeeManagement.Tests/Bonuses
mkdir -p tests/EmployeeManagement.Tests/Services
```

---

## 8. Capa Domain

### `src/EmployeeManagement.Domain/Enums/PositionType.cs`

```csharp
namespace EmployeeManagement.Domain.Enums;

public enum PositionType
{
    Regular = 1,
    Manager = 2,
    SeniorManager = 3
}
```

### `src/EmployeeManagement.Domain/Entities/Department.cs`

```csharp
namespace EmployeeManagement.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
```

### `src/EmployeeManagement.Domain/Entities/Project.cs`

```csharp
namespace EmployeeManagement.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
```

### `src/EmployeeManagement.Domain/Entities/PositionHistory.cs`

```csharp
namespace EmployeeManagement.Domain.Entities;

public class PositionHistory
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Employee? Employee { get; set; }
}
```

### `src/EmployeeManagement.Domain/Entities/Employee.cs`

```csharp
namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentPosition { get; set; }
    public decimal Salary { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public ICollection<PositionHistory> PositionHistory { get; set; } = new List<PositionHistory>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
```

---

## 9. Capa Application

### `src/EmployeeManagement.Application/Constants/Roles.cs`

```csharp
namespace EmployeeManagement.Application.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
```

### `src/EmployeeManagement.Application/Bonuses/IBonusStrategy.cs`

```csharp
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public interface IBonusStrategy
{
    PositionType PositionType { get; }
    decimal CalculateBonus(decimal salary);
}
```

### `src/EmployeeManagement.Application/Bonuses/IBonusCalculator.cs`

```csharp
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Bonuses;

public interface IBonusCalculator
{
    decimal Calculate(Employee employee);
}
```

### `src/EmployeeManagement.Application/Bonuses/RegularEmployeeBonusStrategy.cs`

```csharp
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class RegularEmployeeBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.Regular;
    public decimal CalculateBonus(decimal salary) => salary * 0.10m;
}
```

### `src/EmployeeManagement.Application/Bonuses/ManagerBonusStrategy.cs`

```csharp
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class ManagerBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.Manager;
    public decimal CalculateBonus(decimal salary) => salary * 0.20m;
}
```

### `src/EmployeeManagement.Application/Bonuses/SeniorManagerBonusStrategy.cs`

```csharp
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class SeniorManagerBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.SeniorManager;
    public decimal CalculateBonus(decimal salary) => salary * 0.25m;
}
```

### `src/EmployeeManagement.Application/Bonuses/BonusCalculatorFactory.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class BonusCalculatorFactory : IBonusCalculator
{
    private readonly IReadOnlyDictionary<PositionType, IBonusStrategy> _strategies;

    public BonusCalculatorFactory(IEnumerable<IBonusStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.PositionType);
    }

    public decimal Calculate(Employee employee)
    {
        var positionType = (PositionType)employee.CurrentPosition;
        if (!_strategies.TryGetValue(positionType, out var strategy))
            throw new InvalidOperationException(
                $"No bonus strategy registered for position type '{positionType}'.");
        return strategy.CalculateBonus(employee.Salary);
    }
}
```

### `src/EmployeeManagement.Application/Dtos/AuthDtos.cs`

```csharp
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
    public static AuthResult Success(AuthResponseDto token) => new(token, Array.Empty<string>());
    public static AuthResult Failure(IEnumerable<string> errors) => new(null, errors.ToList());
}
```

### `src/EmployeeManagement.Application/Dtos/EmployeeDtos.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Dtos;

public record PositionHistoryDto(int Id, string Position, DateTime StartDate, DateTime? EndDate);

public record EmployeeDto(
    int Id,
    string Name,
    int CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int? DepartmentId,
    IReadOnlyCollection<PositionHistoryDto> PositionHistory);

public record CreateEmployeeDto(
    [property: Required][property: MaxLength(200)] string Name,
    [property: Range(1, int.MaxValue)] int CurrentPosition,
    [property: Range(typeof(decimal), "0.01", "9999999.99")] decimal Salary,
    int? DepartmentId);

public record UpdateEmployeeDto(
    [property: Required][property: MaxLength(200)] string Name,
    [property: Range(1, int.MaxValue)] int CurrentPosition,
    [property: Range(typeof(decimal), "0.01", "9999999.99")] decimal Salary,
    int? DepartmentId);
```

### `src/EmployeeManagement.Application/Repositories/IEmployeeRepository.cs`

```csharp
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
    Task UpdateAsync(Employee employee, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Employee>> GetByDepartmentWithProjectsAsync(int departmentId, CancellationToken ct = default);
}
```

### `src/EmployeeManagement.Application/Services/IEmployeeService.cs`

```csharp
using EmployeeManagement.Application.Dtos;

namespace EmployeeManagement.Application.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken ct = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
```

### `src/EmployeeManagement.Application/Services/IAuthService.cs`

```csharp
using EmployeeManagement.Application.Dtos;

namespace EmployeeManagement.Application.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
}
```

### `src/EmployeeManagement.Application/Services/EmployeeService.cs`

```csharp
using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IBonusCalculator _bonusCalculator;

    public EmployeeService(IEmployeeRepository repository, IBonusCalculator bonusCalculator)
    {
        _repository = repository;
        _bonusCalculator = bonusCalculator;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var employees = await _repository.GetAllAsync(ct);
        return employees.Select(ToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await _repository.GetByIdAsync(id, ct);
        return employee is null ? null : ToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        var employee = new Employee
        {
            Name = dto.Name,
            CurrentPosition = dto.CurrentPosition,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId
        };
        await _repository.AddAsync(employee, ct);
        return ToDto(employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var employee = await _repository.GetByIdAsync(id, ct);
        if (employee is null) return null;

        employee.Name = dto.Name;
        employee.CurrentPosition = dto.CurrentPosition;
        employee.Salary = dto.Salary;
        employee.DepartmentId = dto.DepartmentId;

        await _repository.UpdateAsync(employee, ct);
        return ToDto(employee);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        _repository.DeleteAsync(id, ct);

    private EmployeeDto ToDto(Employee e) => new(
        e.Id, e.Name, e.CurrentPosition, e.Salary,
        _bonusCalculator.Calculate(e),
        e.DepartmentId,
        e.PositionHistory
            .Select(p => new PositionHistoryDto(p.Id, p.Position, p.StartDate, p.EndDate))
            .ToList());
}
```

---

## 10. Capa Infrastructure

### `src/EmployeeManagement.Infrastructure/Identity/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser { }
```

### `src/EmployeeManagement.Infrastructure/Auth/JwtOptions.cs`

```csharp
namespace EmployeeManagement.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 60;
}
```

### `src/EmployeeManagement.Infrastructure/Auth/IJwtTokenService.cs`

```csharp
using EmployeeManagement.Infrastructure.Identity;

namespace EmployeeManagement.Infrastructure.Auth;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}
```

### `src/EmployeeManagement.Infrastructure/Auth/JwtTokenService.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private static readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer, audience: _options.Audience,
            claims: claims, expires: expiresAt, signingCredentials: creds);

        return (_tokenHandler.WriteToken(jwt), expiresAt);
    }
}
```

### `src/EmployeeManagement.Infrastructure/Auth/AuthService.cs`

```csharp
using EmployeeManagement.Application.Constants;
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return AuthResult.Failure(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, Roles.User);
        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        return AuthResult.Success(new AuthResponseDto(token, expiresAt));
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return null;

        if (!await _userManager.CheckPasswordAsync(user, dto.Password)) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        return new AuthResponseDto(token, expiresAt);
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/AppDbContext.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PositionHistory> PositionHistories => Set<PositionHistory>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/Configurations/EmployeeConfiguration.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Salary).HasColumnType("numeric(18,2)");
        builder.HasOne(e => e.Department).WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(e => e.PositionHistory).WithOne(p => p.Employee!)
            .HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Projects).WithMany(p => p.Employees)
            .UsingEntity(j => j.ToTable("employee_projects"));
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/Configurations/PositionHistoryConfiguration.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
{
    public void Configure(EntityTypeBuilder<PositionHistory> builder)
    {
        builder.ToTable("position_history");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Position).IsRequired().HasMaxLength(200);
        builder.Property(p => p.StartDate).IsRequired();
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/Configurations/DepartmentConfiguration.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/Configurations/ProjectConfiguration.cs`

```csharp
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.StartDate).IsRequired();
    }
}
```

### `src/EmployeeManagement.Infrastructure/Persistence/DatabaseSeeder.cs`

```csharp
using EmployeeManagement.Application.Constants;
using EmployeeManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var config = provider.GetRequiredService<IConfiguration>();
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@example.com";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin1234";

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var created = await userManager.CreateAsync(admin, adminPassword);
            if (created.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
```

### `src/EmployeeManagement.Infrastructure/Repositories/EmployeeRepository.cs`

```csharp
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) => _context = context;

    public Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Employees.Include(e => e.PositionHistory)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Employees.Include(e => e.PositionHistory)
            .AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        await _context.Employees.AddAsync(employee, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        await _context.Employees.Where(e => e.Id == id).ExecuteDeleteAsync(ct) > 0;

    public async Task<IEnumerable<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken ct = default) =>
        await _context.Employees
            .Include(e => e.Projects)
            .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
            .AsNoTracking().ToListAsync(ct);
}
```

### `src/EmployeeManagement.Infrastructure/DependencyInjection/InfrastructureRegistration.cs`

```csharp
using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Auth;
using EmployeeManagement.Infrastructure.Identity;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.DependencyInjection;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // IMPORTANTE: usar AddIdentityCore (no AddIdentity).
        // AddIdentity sobreescribe el scheme de auth a Cookie, rompiendo JWT.
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IBonusStrategy, RegularEmployeeBonusStrategy>();
        services.AddScoped<IBonusStrategy, ManagerBonusStrategy>();
        services.AddScoped<IBonusStrategy, SeniorManagerBonusStrategy>();
        services.AddScoped<IBonusCalculator, BonusCalculatorFactory>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    public static async Task MigrateAndSeedAsync(this WebApplication app) =>
        await DatabaseSeeder.MigrateAndSeedAsync(app.Services);
}
```

---

## 11. Capa Api

### `src/EmployeeManagement.Api/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=employees;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "change-me-super-secret-key-of-at-least-32-chars",
    "Issuer": "EmployeeManagementApi",
    "Audience": "EmployeeManagementApiClients",
    "ExpiresMinutes": 60
  },
  "Seed": {
    "AdminEmail": "admin@example.com",
    "AdminPassword": "Admin1234"
  }
}
```

### `src/EmployeeManagement.Api/Middleware/RequestLoggingMiddleware.cs`

```csharp
using System.Diagnostics;

namespace EmployeeManagement.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    private static readonly string[] _skipPrefixes = ["/swagger", "/health", "/favicon.ico"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (_skipPrefixes.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await _next(context);
            return;
        }

        var correlationId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("HTTP {Method} {Path} started (correlationId={CorrelationId})",
            context.Request.Method, context.Request.Path + context.Request.QueryString, correlationId);

        try { await _next(context); }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms (correlationId={CorrelationId})",
                context.Request.Method, context.Request.Path + context.Request.QueryString,
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
        }
    }
}
```

### `src/EmployeeManagement.Api/Controllers/AuthController.cs`

```csharp
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        if (!result.Succeeded) return BadRequest(new { errors = result.Errors });
        return Ok(result.Token);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(dto, ct);
        if (response is null) return Unauthorized();
        return Ok(response);
    }
}
```

### `src/EmployeeManagement.Api/Controllers/EmployeesController.cs`

```csharp
using EmployeeManagement.Application.Constants;
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken ct)
    {
        var employee = await _service.GetByIdAsync(id, ct);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
```

### `src/EmployeeManagement.Api/Program.cs`

```csharp
using System.Text;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Infrastructure.Auth;
using EmployeeManagement.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration section is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Bearer token. Format: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
    });
});

var app = builder.Build();

await app.MigrateAndSeedAsync();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## 12. Tests

### `tests/EmployeeManagement.Tests/Bonuses/BonusStrategiesTests.cs`

```csharp
using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Tests.Bonuses;

public class BonusStrategiesTests
{
    [Fact]
    public void RegularEmployeeBonusStrategy_returns_10_percent()
    {
        var strategy = new RegularEmployeeBonusStrategy();
        Assert.Equal(PositionType.Regular, strategy.PositionType);
        Assert.Equal(100m, strategy.CalculateBonus(1000m));
    }

    [Fact]
    public void ManagerBonusStrategy_returns_20_percent()
    {
        var strategy = new ManagerBonusStrategy();
        Assert.Equal(PositionType.Manager, strategy.PositionType);
        Assert.Equal(200m, strategy.CalculateBonus(1000m));
    }

    [Fact]
    public void SeniorManagerBonusStrategy_returns_25_percent()
    {
        var strategy = new SeniorManagerBonusStrategy();
        Assert.Equal(PositionType.SeniorManager, strategy.PositionType);
        Assert.Equal(250m, strategy.CalculateBonus(1000m));
    }
}
```

### `tests/EmployeeManagement.Tests/Bonuses/BonusCalculatorFactoryTests.cs`

```csharp
using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Tests.Bonuses;

public class BonusCalculatorFactoryTests
{
    private static BonusCalculatorFactory BuildFactory() => new(new IBonusStrategy[]
    {
        new RegularEmployeeBonusStrategy(),
        new ManagerBonusStrategy(),
        new SeniorManagerBonusStrategy()
    });

    [Theory]
    [InlineData(PositionType.Regular, 1000, 100)]
    [InlineData(PositionType.Manager, 1000, 200)]
    [InlineData(PositionType.SeniorManager, 1000, 250)]
    public void Calculate_selects_strategy_by_position(PositionType position, decimal salary, decimal expected)
    {
        var factory = BuildFactory();
        var employee = new Employee { CurrentPosition = (int)position, Salary = salary };
        Assert.Equal(expected, factory.Calculate(employee));
    }

    [Fact]
    public void Calculate_throws_when_no_strategy_registered_for_position()
    {
        var factory = BuildFactory();
        var employee = new Employee { CurrentPosition = 999, Salary = 1000 };
        Assert.Throws<InvalidOperationException>(() => factory.Calculate(employee));
    }
}
```

### `tests/EmployeeManagement.Tests/Services/EmployeeServiceTests.cs`

```csharp
using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Moq;

namespace EmployeeManagement.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repository = new();
    private readonly Mock<IBonusCalculator> _bonusCalculator = new();

    private EmployeeService BuildService() => new(_repository.Object, _bonusCalculator.Object);

    [Fact]
    public async Task GetByIdAsync_returns_null_when_repository_returns_null()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var result = await BuildService().GetByIdAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_maps_employee_with_calculated_bonus()
    {
        var employee = new Employee { Id = 1, Name = "Ada", CurrentPosition = (int)PositionType.Manager, Salary = 5000m };
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _bonusCalculator.Setup(b => b.Calculate(employee)).Returns(1000m);

        var result = await BuildService().GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Ada", result.Name);
        Assert.Equal(1000m, result.YearlyBonus);
    }

    [Fact]
    public async Task CreateAsync_persists_and_returns_dto()
    {
        var dto = new CreateEmployeeDto("Grace", (int)PositionType.Regular, 4000m, null);
        _bonusCalculator.Setup(b => b.Calculate(It.IsAny<Employee>())).Returns(400m);

        var result = await BuildService().CreateAsync(dto);

        _repository.Verify(r => r.AddAsync(
            It.Is<Employee>(e => e.Name == "Grace" && e.Salary == 4000m),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Grace", result.Name);
        Assert.Equal(400m, result.YearlyBonus);
    }

    [Fact]
    public async Task DeleteAsync_delegates_to_repository()
    {
        _repository.Setup(r => r.DeleteAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await BuildService().DeleteAsync(7);
        Assert.True(result);
    }
}
```

---

## 13. Archivos raíz

### `Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY EmployeeManagement.slnx ./
COPY src/EmployeeManagement.Domain/*.csproj src/EmployeeManagement.Domain/
COPY src/EmployeeManagement.Application/*.csproj src/EmployeeManagement.Application/
COPY src/EmployeeManagement.Infrastructure/*.csproj src/EmployeeManagement.Infrastructure/
COPY src/EmployeeManagement.Api/*.csproj src/EmployeeManagement.Api/
COPY tests/EmployeeManagement.Tests/*.csproj tests/EmployeeManagement.Tests/
RUN dotnet restore src/EmployeeManagement.Api/EmployeeManagement.Api.csproj

COPY src/ src/
RUN dotnet publish src/EmployeeManagement.Api/EmployeeManagement.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
```

### `docker-compose.yml`

```yaml
services:
  db:
    image: postgres:16-alpine
    container_name: employees-db
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: employees
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d employees"]
      interval: 5s
      timeout: 5s
      retries: 10

  api:
    build: { context: ., dockerfile: Dockerfile }
    container_name: employees-api
    depends_on:
      db: { condition: service_healthy }
    environment:
      ConnectionStrings__Default: "Host=db;Port=5432;Database=employees;Username=postgres;Password=postgres"
      Jwt__Key: "change-me-super-secret-key-of-at-least-32-chars"
      Jwt__Issuer: "EmployeeManagementApi"
      Jwt__Audience: "EmployeeManagementApiClients"
      Jwt__ExpiresMinutes: "60"
      Seed__AdminEmail: "admin@example.com"
      Seed__AdminPassword: "Admin1234"
    ports:
      - "8080:8080"

volumes:
  pgdata:
```

### `.dockerignore`

```
**/bin
**/obj
**/.vs
**/.vscode
**/.idea
**/*.user
.git
.gitignore
.env
*.md
Dockerfile
docker-compose.yml
.dockerignore
```

### `.gitignore`

```
bin/
obj/
*.user
.vs/
.vscode/
.idea/
*.suo
*.userprefs
.DS_Store
.env
```

---

## 14. Crear la migración de EF Core

> Necesitas PostgreSQL corriendo localmente con las credenciales del `appsettings.json`.

```bash
dotnet ef migrations add InitialCreate \
  --project src/EmployeeManagement.Infrastructure \
  --startup-project src/EmployeeManagement.Api \
  --output-dir Persistence/Migrations
```

---

## 15. Verificar build y tests

```bash
dotnet build
dotnet test
```

Esperado: `Build succeeded`, `11 passed`.

---

## 16. Levantar con Docker

```bash
docker compose up --build
```

- Swagger: `http://localhost:8080/swagger`
- Admin: `admin@example.com` / `Admin1234`

---

## Notas críticas

| Problema | Causa | Solución |
|---|---|---|
| 401 en endpoints con JWT válido | `AddIdentity` sobreescribe el auth scheme a Cookie | Usar `AddIdentityCore` + `.AddRoles<IdentityRole>()` |
| Error al compilar Identity en class library | Falta referencia a `Microsoft.AspNetCore.App` | Agregar `<FrameworkReference>` en Infrastructure.csproj |
| `COPY EmployeeManagement.sln` falla en Docker | SDK 10 genera `.slnx`, no `.sln` | Usar `COPY EmployeeManagement.slnx ./` en Dockerfile |
| No usar Singleton | Requisito explícito de la prueba | Estrategias de bonos registradas como Scoped |
