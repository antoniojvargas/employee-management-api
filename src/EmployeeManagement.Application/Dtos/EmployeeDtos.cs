using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Dtos;

public record PositionHistoryDto(
    int Id,
    string Position,
    DateTime StartDate,
    DateTime? EndDate);

public record EmployeeDto(
    int Id,
    string Name,
    int CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int? DepartmentId,
    IReadOnlyCollection<PositionHistoryDto> PositionHistory);

public record CreateEmployeeDto(
    [Required][MaxLength(200)] string Name,
    [Range(1, int.MaxValue)] int CurrentPosition,
    [Range(typeof(decimal), "0.01", "9999999.99")] decimal Salary,
    int? DepartmentId);

public record UpdateEmployeeDto(
    [Required][MaxLength(200)] string Name,
    [Range(1, int.MaxValue)] int CurrentPosition,
    [Range(typeof(decimal), "0.01", "9999999.99")] decimal Salary,
    int? DepartmentId);
