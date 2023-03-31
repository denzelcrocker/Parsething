namespace DatabaseLibrary.Entities.PreferenceMuchToMany;

public partial class ProcurementsPreference
{
    public int ProcurementId { get; set; }
    public int PreferenceId { get; set; }

    public virtual Preference Preference { get; set; } = null!;
    public virtual Procurement Procurement { get; set; } = null!;
}