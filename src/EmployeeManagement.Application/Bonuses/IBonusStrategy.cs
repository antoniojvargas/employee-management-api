using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public interface IBonusStrategy
{
    PositionType PositionType { get; }
    decimal CalculateBonus(decimal salary);
}
