namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentPosition { get; set; }
    public decimal Salary { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<PositionHistory> PositionHistory { get; set; } = new List<PositionHistory>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
