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
        e.Id,
        e.Name,
        e.CurrentPosition,
        e.Salary,
        _bonusCalculator.Calculate(e),
        e.DepartmentId,
        e.PositionHistory
            .Select(p => new PositionHistoryDto(p.Id, p.Position, p.StartDate, p.EndDate))
            .ToList());
}
