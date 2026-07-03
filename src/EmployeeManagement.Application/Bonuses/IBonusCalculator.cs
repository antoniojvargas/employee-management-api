using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Bonuses;

public interface IBonusCalculator
{
    decimal Calculate(Employee employee);
}
