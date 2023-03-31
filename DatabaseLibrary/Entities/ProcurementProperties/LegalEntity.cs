namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class LegalEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}