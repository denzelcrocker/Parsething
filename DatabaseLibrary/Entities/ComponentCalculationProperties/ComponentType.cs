namespace DatabaseLibrary.Entities.ComponentCalculationProperties;

public partial class ComponentType
{
    public int Id { get; set; }
    public string Kind { get; set; } = null!;

    public virtual ICollection<Component> Components { get; } = new List<Component>();
}