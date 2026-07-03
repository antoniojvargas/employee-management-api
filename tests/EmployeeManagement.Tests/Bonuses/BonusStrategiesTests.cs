using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Domain.Enums;
using Xunit;

namespace EmployeeManagement.Tests.Bonuses;

public class BonusStrategiesTests
{
    [Fact]
    public void RegularEmployeeBonusStrategy_returns_10_percent()
    {
        var strategy = new RegularEmployeeBonusStrategy();
        Assert.Equal(PositionType.Regular, strategy.PositionType);
        Assert.Equal(100m, strategy.CalculateBonus(1000m));
    }

    [Fact]
    public void ManagerBonusStrategy_returns_20_percent()
    {
        var strategy = new ManagerBonusStrategy();
        Assert.Equal(PositionType.Manager, strategy.PositionType);
        Assert.Equal(200m, strategy.CalculateBonus(1000m));
    }

    [Fact]
    public void SeniorManagerBonusStrategy_returns_25_percent()
    {
        var strategy = new SeniorManagerBonusStrategy();
        Assert.Equal(PositionType.SeniorManager, strategy.PositionType);
        Assert.Equal(250m, strategy.CalculateBonus(1000m));
    }
}
