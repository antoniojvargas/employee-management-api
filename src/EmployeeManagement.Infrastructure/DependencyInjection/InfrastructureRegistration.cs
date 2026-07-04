using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Auth;
using EmployeeManagement.Infrastructure.Identity;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.DependencyInjection;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

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

        // Strategies are pure functions — safe and correct as Singletons
        services.AddSingleton<IBonusStrategy, RegularEmployeeBonusStrategy>();
        services.AddSingleton<IBonusStrategy, ManagerBonusStrategy>();
        services.AddSingleton<IBonusStrategy, SeniorManagerBonusStrategy>();
        services.AddSingleton<IBonusCalculator, BonusCalculatorFactory>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    public static async Task MigrateAndSeedAsync(this WebApplication app) =>
        await DatabaseSeeder.MigrateAndSeedAsync(app.Services);
}
