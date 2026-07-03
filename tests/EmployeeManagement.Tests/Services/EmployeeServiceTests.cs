using EmployeeManagement.Application.Bonuses;
using EmployeeManagement.Application.Dtos;
using EmployeeManagement.Application.Repositories;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repository = new();
    private readonly Mock<IBonusCalculator> _bonusCalculator = new();

    private EmployeeService BuildService() => new(_repository.Object, _bonusCalculator.Object);

    [Fact]
    public async Task GetByIdAsync_returns_null_when_repository_returns_null()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var service = BuildService();
        var result = await service.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_maps_employee_with_calculated_bonus()
    {
        var employee = new Employee
        {
            Id = 1,
            Name = "Ada",
            CurrentPosition = (int)PositionType.Manager,
            Salary = 5000m
        };
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        _bonusCalculator.Setup(b => b.Calculate(employee)).Returns(1000m);

        var result = await BuildService().GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Ada", result.Name);
        Assert.Equal(1000m, result.YearlyBonus);
    }

    [Fact]
    public async Task CreateAsync_persists_and_returns_dto()
    {
        var dto = new CreateEmployeeDto("Grace", (int)PositionType.Regular, 4000m, null);
        _bonusCalculator.Setup(b => b.Calculate(It.IsAny<Employee>())).Returns(400m);

        var result = await BuildService().CreateAsync(dto);

        _repository.Verify(r => r.AddAsync(
            It.Is<Employee>(e => e.Name == "Grace" && e.Salary == 4000m),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Grace", result.Name);
        Assert.Equal(400m, result.YearlyBonus);
    }

    [Fact]
    public async Task DeleteAsync_delegates_to_repository()
    {
        _repository.Setup(r => r.DeleteAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await BuildService().DeleteAsync(7);

        Assert.True(result);
    }
}
