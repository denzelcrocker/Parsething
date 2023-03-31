namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class ProcurementState
{
    public int Id { get; set; }
    public string Kind { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}