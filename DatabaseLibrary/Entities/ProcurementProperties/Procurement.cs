namespace DatabaseLibrary.Entities.ProcurementProperties;

public partial class Procurement
{
    public int Id { get; set; }
    public string RequestUri { get; set; } = null!;
    public string Number { get; set; } = null!;
    public int LawId { get; set; }
    public string Object { get; set; } = null!;
    public decimal InitialPrice { get; set; }
    public int OrganizationId { get; set; }
    public int? MethodId { get; set; }
    public int? PlatformId { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? Deadline { get; set; }
    public int? TimeZoneId { get; set; }
    public string? Securing { get; set; }
    public string? Enforcement { get; set; }
    public string? Warranty { get; set; }
    public int? RegionId { get; set; }
    public string? OrganizationContractName { get; set; }
    public string? OrganizationContractPostalAddress { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? DeliveryDetails { get; set; }
    public string? DeadlineAndType { get; set; }
    public string? DeliveryDeadline { get; set; }
    public string? AcceptanceDeadline { get; set; }
    public string? ContractDeadline { get; set; }
    public bool? Indefinitely { get; set; }
    public string? AnotherDeadline { get; set; }
    public string? DeadlineAndOrder { get; set; }
    public int? RepresentativeTypeId { get; set; }
    public int? CommissioningWorksId { get; set; }
    public int? PlaceCount { get; set; }
    public string? FinesAndPennies { get; set; }
    public string? PenniesPerDay { get; set; }
    public string? TerminationConditions { get; set; }
    public string? EliminationDeadline { get; set; }
    public string? GuaranteePeriod { get; set; }
    public string? Inn { get; set; }
    public string? ContractNumber { get; set; }
    public int? EmployeeId { get; set; }
    public bool? AssemblyNeed { get; set; }
    public int? MinopttorgId { get; set; }
    public int? LegalEntityId { get; set; }
    public bool? Applications { get; set; }
    public decimal? Bet { get; set; }
    public decimal? MinimalPrice { get; set; }
    public decimal? ContractAmount { get; set; }
    public decimal? ReserveContractAmount { get; set; }
    public DateTime? ProtocolDate { get; set; }
    public int? ShipmentPlanId { get; set; }
    public bool? WaitingList { get; set; }
    public bool? Calculating { get; set; }
    public bool? Purchase { get; set; }
    public int? ExecutionStateId { get; set; }
    public int? WarrantyStateId { get; set; }
    public DateTime? SigningDeadline { get; set; }
    public DateTime? SigningDate { get; set; }
    public DateTime? ConclusionDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public DateTime? DepartureDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? MaxAcceptanceDate { get; set; }
    public DateTime? CorrectionDate { get; set; }
    public DateTime? ActDate { get; set; }
    public DateTime? MaxDueDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? RealDueDate { get; set; }
    public decimal? Amount { get; set; }
    public int? SignedOriginalId { get; set; }
    public bool? Judgment { get; set; }
    public bool? Fas { get; set; }
    public int? ProcurementStateId { get; set; }

    public virtual ICollection<ComponentCalculation> ComponentCalculations { get; } = new List<ComponentCalculation>();

    public virtual CommisioningWork? CommissioningWorks { get; set; }
    public virtual ExecutionState? ExecutionState { get; set; }
    public virtual Law Law { get; set; } = null!;
    public virtual LegalEntity? LegalEntity { get; set; }
    public virtual Method? Method { get; set; }
    public virtual Minopttorg? Minopttorg { get; set; }
    public virtual Organization Organization { get; set; } = null!;
    public virtual Platform? Platform { get; set; }
    public virtual ProcurementState? ProcurementState { get; set; }
    public virtual Region? Region { get; set; }
    public virtual RepresentativeType? RepresentativeType { get; set; }
    public virtual ShipmentPlan? ShipmentPlan { get; set; }
    public virtual SignedOriginal? SignedOriginal { get; set; }
    public virtual TimeZone? TimeZone { get; set; }
    public virtual WarrantyState? WarrantyState { get; set; }
}