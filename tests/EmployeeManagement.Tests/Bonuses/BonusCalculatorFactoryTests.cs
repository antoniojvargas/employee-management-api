using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Xunit;

namespace EmployeeManagement.Tests.Bonuses;

public class BonusCalculatorFactoryTests
{
    private static BonusCalculatorFactory BuildFactory() => new(new IBonusStrategy[]
    {
        new RegularEmployeeBonusStrategy(),
        new ManagerBonusStrategy(),
        new SeniorManagerBonusStrategy()
    });

    [Theory]
    [InlineData(PositionType.Regular, 1000, 100)]
    [InlineData(PositionType.Manager, 1000, 200)]
    [InlineData(PositionType.SeniorManager, 1000, 250)]
    public void Calculate_selects_strategy_by_position(PositionType position, decimal salary, decimal expected)
    {
        var factory = BuildFactory();
        var employee = new Employee { CurrentPosition = (int)position, Salary = salary };

        Assert.Equal(expected, factory.Calculate(employee));
    }

    [Fact]
    public void Calculate_throws_when_no_strategy_registered_for_position()
    {
        var factory = BuildFactory();
        var employee = new Employee { CurrentPosition = 999, Salary = 1000 };

        Assert.Throws<InvalidOperationException>(() => factory.Calculate(employee));
    }
}
