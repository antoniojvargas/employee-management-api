using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class ManagerBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.Manager;
    public decimal CalculateBonus(decimal salary) => salary * 0.20m;
}
