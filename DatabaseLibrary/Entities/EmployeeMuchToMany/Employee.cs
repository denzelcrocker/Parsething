namespace DatabaseLibrary.Entities.EmployeeMuchToMany;

public partial class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int PositionId { get; set; }
    public string? Photo { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();
    public virtual ICollection<History> Histories { get; } = new List<History>();

    public virtual Position Position { get; set; } = null!;
}
