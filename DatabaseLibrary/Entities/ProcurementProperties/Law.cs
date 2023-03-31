namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class Law
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;

    public virtual ICollection<Procurement> Procurements { get; } = new List<Procurement>();
}