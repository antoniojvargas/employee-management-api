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
    string Name,
    int CurrentPosition,
    decimal Salary,
    int? DepartmentId);

public record UpdateEmployeeDto(
    string Name,
    int CurrentPosition,
    decimal Salary,
    int? DepartmentId);
