namespace DatabaseLibrary.Entities.EmployeeMuchToMany;

public partial class ProcurementsEmployee
{
    public int ProcurementId { get; set; }
    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;
    public virtual Procurement Procurement { get; set; } = null!;
}