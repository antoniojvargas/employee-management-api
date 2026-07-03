using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class RegularEmployeeBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.Regular;
    public decimal CalculateBonus(decimal salary) => salary * 0.10m;
}
