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
        _context.Employees
            .Include(e => e.PositionHistory)
            .Include(e => e.Projects)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Employees
            .Include(e => e.PositionHistory)
            .AsNoTracking()
            .ToListAsync(ct);

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

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var employee = await _context.Employees.FindAsync(new object?[] { id }, ct);
        if (employee is null) return false;
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken ct = default) =>
        await _context.Employees
            .Include(e => e.Projects)
            .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
            .AsNoTracking()
            .ToListAsync(ct);
}
