namespace DatabaseLibrary.Entities.EmployeeMuchToMany;

public partial class Position
{
    public int Id { get; set; }
    public string Kind { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}