namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PostalAddress { get; set; }

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}