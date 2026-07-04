# Employee Management API

Technical test for .NET Backend Developer. RESTful API built with ASP.NET Core on PostgreSQL, featuring JWT authentication, role-based authorization, and Clean Architecture.

## Stack

- .NET 10 (LTS) + ASP.NET Core Web API
- Entity Framework Core 10 + `Npgsql.EntityFrameworkCore.PostgreSQL`
- PostgreSQL 16 (`postgres:16-alpine` image)
- ASP.NET Core Identity + JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Swashbuckle (Swagger UI)
- xUnit + Moq (unit tests)
- Docker + docker-compose

> **Note on .NET version:** the test does not specify a version. .NET 10 (latest LTS available) was used. The code compiles and runs unchanged on .NET 8/9 — only the `TargetFramework` value needs updating.

## Architecture

**Clean Architecture** with 4 projects, dependencies pointing inward:

```
src/
├── EmployeeManagement.Domain/          # Entities and enums — no external dependencies
├── EmployeeManagement.Application/     # Interfaces, DTOs, services, patterns
├── EmployeeManagement.Infrastructure/  # EF Core, repositories, Identity, JWT
└── EmployeeManagement.Api/             # Controllers, middleware, composition root
tests/
└── EmployeeManagement.Tests/           # xUnit + Moq
```

Dependency rule: `Api → Infrastructure → Application → Domain`. `Domain` depends on nothing.

## Design Patterns Applied (Singleton excluded)

1. **Strategy** — `IBonusStrategy` with `RegularEmployeeBonusStrategy` (10%), `ManagerBonusStrategy` (20%), and `SeniorManagerBonusStrategy` (25%). Adding a new manager type requires only a new class and a DI registration (OCP).
2. **Factory** — `BonusCalculatorFactory : IBonusCalculator` receives all strategies via `IEnumerable<IBonusStrategy>` and selects the correct one based on `Employee.CurrentPosition`.
3. **Repository** — `IEmployeeRepository` decouples the application layer from EF Core. The implementation (`EmployeeRepository`) lives in Infrastructure.

DI registration (`InfrastructureRegistration`):

```csharp
services.AddScoped<IBonusStrategy, RegularEmployeeBonusStrategy>();
services.AddScoped<IBonusStrategy, ManagerBonusStrategy>();
services.AddScoped<IBonusStrategy, SeniorManagerBonusStrategy>();
services.AddScoped<IBonusCalculator, BonusCalculatorFactory>();
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
```

## SOLID Principles

- **S**: `EmployeeService`, `BonusCalculatorFactory`, each strategy, and each `IEntityTypeConfiguration<T>` have a single reason to change.
- **O**: Adding a new bonus strategy does not touch existing code — just add a new class.
- **L**: Any `IBonusStrategy` implementation is interchangeable inside the factory.
- **I**: Focused interfaces (`IEmployeeRepository`, `IBonusCalculator`, `IJwtTokenService`, `IAuthService`).
- **D**: Controllers depend on abstractions (`IEmployeeService`, `IAuthService`), not implementations.

## Requirements

- Docker + Docker Compose (required)
- .NET SDK 10.0+ (only needed to run tests outside Docker)

## Running with Docker

```bash
docker compose up --build
```

Starts two containers:
- `employees-db` → `postgres:16-alpine`, exposed on port `5432`.
- `employees-api` → API available at `http://localhost:8080`.

On startup the API automatically applies EF Core migrations and seeds:
- Roles `Admin` and `User`.
- Default admin user (`admin@example.com` / `Admin1234`).

Swagger UI: `http://localhost:8080/swagger`

## Endpoint Testing — Full curl Walkthrough

### Setup — get an Admin token

```bash
# 1. Login as admin
curl -s -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin1234"}' | cat

# Copy the token from the response and set it:
TOKEN="<paste-token-here>"
```

---

### Employee CRUD (Admin token)

```bash
# 2. GET all employees — returns empty list initially (Admin and User roles)
curl -s http://localhost:8080/api/employees \
  -H "Authorization: Bearer $TOKEN" | cat
# Expected: 200 []

# 3. POST — create an employee (Admin only)
curl -s -X POST http://localhost:8080/api/employees \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Ada Lovelace","currentPosition":2,"salary":5000,"departmentId":null}' | cat
# Expected: 201 — employee created, yearlyBonus: 1000 (20% of 5000)

# 4. GET employee by id (Admin and User roles)
curl -s http://localhost:8080/api/employees/1 \
  -H "Authorization: Bearer $TOKEN" | cat
# Expected: 200 — employee details

# 5. PUT — update employee (Admin only)
curl -s -X PUT http://localhost:8080/api/employees/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Ada Lovelace Updated","currentPosition":3,"salary":6000,"departmentId":null}' | cat
# Expected: 200 — updated employee, yearlyBonus: 1500 (25% of 6000)

# 6. DELETE — remove employee (Admin only)
curl -s -X DELETE http://localhost:8080/api/employees/1 \
  -H "Authorization: Bearer $TOKEN" | cat
# Expected: 204 No Content
```

---

### Role-based authorization check

```bash
# Register a User-role account
curl -s -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"User1234"}' | cat

# Set the User token
USER_TOKEN="<paste-user-token-here>"

# 7. GET — User can list employees (allowed)
curl -s http://localhost:8080/api/employees \
  -H "Authorization: Bearer $USER_TOKEN" | cat
# Expected: 200

# POST — User cannot create employees (forbidden)
curl -s -X POST http://localhost:8080/api/employees \
  -H "Authorization: Bearer $USER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","currentPosition":1,"salary":3000,"departmentId":null}' | cat
# Expected: 403 Forbidden
```

`currentPosition`: 1 = Regular (10% bonus), 2 = Manager (20%), 3 = SeniorManager (25%)

## Tests

```bash
dotnet test
```

Covers: bonus strategies, factory selection logic, and `EmployeeService` (with mocked repository and bonus calculator).

## Database Schema

All tables managed by EF Core migrations:

- `employees` (Id, Name, CurrentPosition, Salary, DepartmentId)
- `position_history` (Id, EmployeeId, Position, StartDate, EndDate)
- `departments` (Id, Name)
- `projects` (Id, Name, StartDate, EndDate)
- `employee_projects` (many-to-many join table)
- Identity tables: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.

### LINQ query — Section 4.3

`EmployeeRepository.GetByDepartmentWithProjectsAsync`:

```csharp
return await _context.Employees
    .Include(e => e.Projects)
    .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
    .AsNoTracking()
    .ToListAsync(ct);
```

## Endpoints

| Method | Route                   | Roles       |
| ------ | ----------------------- | ----------- |
| GET    | `/api/employees`        | Admin, User |
| GET    | `/api/employees/{id}`   | Admin, User |
| POST   | `/api/employees`        | Admin       |
| PUT    | `/api/employees/{id}`   | Admin       |
| DELETE | `/api/employees/{id}`   | Admin       |
| POST   | `/api/auth/register`    | anonymous   |
| POST   | `/api/auth/login`       | anonymous   |

## Technical Questions — Written Answers

### Section 2.2 — Authentication and Authorization

ASP.NET Core Identity manages users and roles (persisted via EF Core on PostgreSQL). JWT Bearer handles stateless authentication.

The Bearer scheme is registered at startup:

```csharp
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });
```

Authorization is applied declaratively per controller/action with `[Authorize(Roles = "...")]`. `JwtTokenService` injects the `role` claim into the token; the authorization middleware evaluates it against those attributes.

### Section 2.3 — Middleware

Middleware in ASP.NET Core is a component that participates in the HTTP request processing pipeline. Each component can: (a) inspect/modify the request, (b) invoke the next component via `RequestDelegate`, (c) inspect/modify the response. Components are registered in order in `Program.cs` — order matters (e.g., `UseAuthentication` must come before `UseAuthorization`).

This project includes `RequestLoggingMiddleware` (`src/EmployeeManagement.Api/Middleware/RequestLoggingMiddleware.cs`) that logs the HTTP method, path, status code, duration in ms, and the `correlationId` (TraceIdentifier) for every request.

### Section 5.1 — Common Performance Issues in .NET

- **N+1 queries** with EF Core → use explicit `Include` or projections.
- **Missing `AsNoTracking()`** on reads → EF unnecessarily tracks entities.
- **Materializing full tables** (`.ToList()` before `.Where()`) → filter on the server side.
- **Synchronous I/O blocking** → use `async/await` end-to-end.
- **GC pressure from large allocations** → `Span<T>`, `ArrayPool<T>`, object pooling.
- **Heavy JSON serialization** → prefer `System.Text.Json` with source generators over `Newtonsoft.Json`.
- **Missing cache for frequent reads** → `IMemoryCache` / `IDistributedCache`.
- **Reflection in hot paths** → cache compiled delegates or expression trees.

### Section 5.2 — Profiling and Optimizing a Slow Query

1. **Reproduce and measure**: capture the actual SQL with `EnableSensitiveDataLogging()` in development, or via `IDbCommandInterceptor` in production.
2. **`EXPLAIN (ANALYZE, BUFFERS)`** in PostgreSQL to inspect the execution plan — look for `Seq Scan` on large tables or expensive hash joins.
3. **Indexes** on filter (`WHERE`) and join columns. Use composite indexes for multi-column filters.
4. **`AsNoTracking()`** on read-only queries.
5. **Projection** to a DTO with `.Select(...)` to fetch only the required columns.
6. **Mandatory pagination** (`Skip/Take`).
7. **`AsSplitQuery()`** when multiple `Include` calls cause a Cartesian explosion.
8. **Result caching** where applicable.
9. **APM / profiling tools** (MiniProfiler, dotTrace, Application Insights) to locate hotspots.
10. For isolated complex queries: consider a stored procedure or `FromSqlRaw` as a last resort.
