namespace DatabaseLibrary.Entities.ComponentCalculationProperties;

public partial class ComponentCalculation
{
    public int Id { get; set; }
    public int ProcurementId { get; set; }
    public int ComponentId { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public int SellerId { get; set; }
    public int? ComponentStateId { get; set; }
    public DateTime? Date { get; set; }
    public string? Reserve { get; set; }
    public string? Note { get; set; }

    public virtual Component Component { get; set; } = null!;
    public virtual ComponentState? ComponentState { get; set; }
    public virtual Procurement Procurement { get; set; } = null!;
    public virtual Seller Seller { get; set; } = null!;
}