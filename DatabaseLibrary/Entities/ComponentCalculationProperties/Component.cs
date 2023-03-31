namespace DatabaseLibrary.Entities.ComponentCalculationProperties;

public partial class Component
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int ManufacturerId { get; set; }
    public int ComponentTypeId { get; set; }

    public virtual ICollection<ComponentCalculation> ComponentCalculations { get; } = new List<ComponentCalculation>();

    public virtual ComponentType ComponentType { get; set; } = null!;
    public virtual Manufacturer Manufacturer { get; set; } = null!;
}