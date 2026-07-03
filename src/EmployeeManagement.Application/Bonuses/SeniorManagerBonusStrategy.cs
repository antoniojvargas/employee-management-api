using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class SeniorManagerBonusStrategy : IBonusStrategy
{
    public PositionType PositionType => PositionType.SeniorManager;
    public decimal CalculateBonus(decimal salary) => salary * 0.25m;
}
