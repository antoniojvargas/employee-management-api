using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Bonuses;

public class BonusCalculatorFactory : IBonusCalculator
{
    private readonly IReadOnlyDictionary<PositionType, IBonusStrategy> _strategies;

    public BonusCalculatorFactory(IEnumerable<IBonusStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.PositionType);
    }

    public decimal Calculate(Employee employee)
    {
        var positionType = (PositionType)employee.CurrentPosition;
        if (!_strategies.TryGetValue(positionType, out var strategy))
        {
            throw new InvalidOperationException(
                $"No bonus strategy registered for position type '{positionType}'.");
        }
        return strategy.CalculateBonus(employee.Salary);
    }
}
