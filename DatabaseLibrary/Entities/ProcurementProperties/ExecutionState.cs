namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class ExecutionState
{
    public int Id { get; set; }
    public string Kind { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}