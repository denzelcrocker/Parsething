namespace DatabaseLibrary;

public partial class ParsethingContext : DbContext
{
    public ParsethingContext() { }

    public virtual DbSet<Comment> Comments { get; set; } = null!;
    public virtual DbSet<CommisioningWork> CommisioningWorks { get; set; } = null!;
    public virtual DbSet<Component> Components { get; set; } = null!;
    public virtual DbSet<ComponentCalculation> ComponentCalculations { get; set; } = null!;
    public virtual DbSet<ComponentState> ComponentStates { get; set; } = null!;
    public virtual DbSet<ComponentType> ComponentTypes { get; set; } = null!;
    public virtual DbSet<Document> Documents { get; set; } = null!;
    public virtual DbSet<Employee> Employees { get; set; } = null!;
    public virtual DbSet<ExecutionState> ExecutionStates { get; set; } = null!;
    public virtual DbSet<History> Histories { get; set; } = null!;
    public virtual DbSet<Law> Laws { get; set; } = null!;
    public virtual DbSet<LegalEntity> LegalEntities { get; set; } = null!;
    public virtual DbSet<Manufacturer> Manufacturers { get; set; } = null!;
    public virtual DbSet<Method> Methods { get; set; } = null!;
    public virtual DbSet<Minopttorg> Minopttorgs { get; set; } = null!;
    public virtual DbSet<Organization> Organizations { get; set; } = null!;
    public virtual DbSet<Platform> Platforms { get; set; } = null!;
    public virtual DbSet<Position> Positions { get; set; } = null!;
    public virtual DbSet<Preference> Preferences { get; set; } = null!;
    public virtual DbSet<Procurement> Procurements { get; set; } = null!;
    public virtual DbSet<ProcurementState> ProcurementStates { get; set; } = null!;
    public virtual DbSet<ProcurementsDocument> ProcurementsDocuments { get; set; } = null!;
    public virtual DbSet<ProcurementsEmployee> ProcurementsEmployees { get; set; } = null!;
    public virtual DbSet<ProcurementsPreference> ProcurementsPreferences { get; set; } = null!;
    public virtual DbSet<Region> Regions { get; set; } = null!;
    public virtual DbSet<RepresentativeType> RepresentativeTypes { get; set; } = null!;
    public virtual DbSet<Seller> Sellers { get; set; } = null!;
    public virtual DbSet<ShipmentPlan> ShipmentPlans { get; set; } = null!;
    public virtual DbSet<SignedOriginal> SignedOriginals { get; set; } = null!;
    public virtual DbSet<Tag> Tags { get; set; } = null!;
    public virtual DbSet<TimeZone> TimeZones { get; set; } = null!;
    public virtual DbSet<WarrantyState> WarrantyStates { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlServer(Resources.ConnectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Comment>(entity =>
        {
            _ = entity.Property(e => e.Date).HasColumnType("datetime");

            _ = entity.HasOne(d => d.Employee).WithMany(p => p.Comments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Employees");
        });

        _ = modelBuilder.Entity<CommisioningWork>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_ConmmisioningWorks");
        });

        _ = modelBuilder.Entity<Component>(entity =>
        {
            _ = entity.HasOne(d => d.ComponentType).WithMany(p => p.Components)
                .HasForeignKey(d => d.ComponentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Components_ComponentTypes");

            _ = entity.HasOne(d => d.Manufacturer).WithMany(p => p.Components)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Components_Manufacturers");
        });

        _ = modelBuilder.Entity<ComponentCalculation>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_CalculationAndPurchasingList");

            _ = entity.Property(e => e.Date).HasColumnType("datetime");
            _ = entity.Property(e => e.Price).HasColumnType("decimal(19, 2)");

            _ = entity.HasOne(d => d.Component).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComponentCalculations_Components");

            _ = entity.HasOne(d => d.ComponentState).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ComponentStateId)
                .HasConstraintName("FK_ComponentCalculations_ComponentStates");

            _ = entity.HasOne(d => d.Procurement).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComponentCalculations_Procurements");

            _ = entity.HasOne(d => d.Seller).WithMany(p => p.ComponentCalculations)
                        .HasForeignKey(d => d.SellerId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ComponentCalculations_Sellers");
        });

        _ = modelBuilder.Entity<ComponentState>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_StatusesOfProduct");
        });

        _ = modelBuilder.Entity<Employee>(entity =>
        {
            _ = entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Positions");
        });

        _ = modelBuilder.Entity<ExecutionState>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_StatusesForExecution");
        });

        _ = modelBuilder.Entity<History>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_Chronology");

            _ = entity.Property(e => e.Date).HasColumnType("datetime");

            _ = entity.HasOne(d => d.Employee).WithMany(p => p.Histories)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_Employees");
        });

        _ = modelBuilder.Entity<Minopttorg>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_Minopttorg");
        });

        _ = modelBuilder.Entity<Procurement>(entity =>
        {
            _ = entity.Property(e => e.ActDate).HasColumnType("datetime");
            _ = entity.Property(e => e.ActualDeliveryDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.Bet).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            _ = entity.Property(e => e.ConclusionDate).HasColumnType("datetime");
            _ = entity.Property(e => e.ContractAmount).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.CorrectionDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Deadline).HasColumnType("datetime");
            _ = entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            _ = entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            _ = entity.Property(e => e.Fas).HasColumnName("FAS");
            _ = entity.Property(e => e.InitialPrice).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.Inn).HasColumnName("INN");
            _ = entity.Property(e => e.MaxAcceptanceDate).HasColumnType("datetime");
            _ = entity.Property(e => e.MaxDueDate).HasColumnType("datetime");
            _ = entity.Property(e => e.MinimalPrice).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.ProtocolDate).HasColumnType("datetime");
            _ = entity.Property(e => e.RealDueDate).HasColumnType("datetime");
            _ = entity.Property(e => e.ReserveContractAmount).HasColumnType("decimal(19, 2)");
            _ = entity.Property(e => e.SigningDate).HasColumnType("datetime");
            _ = entity.Property(e => e.SigningDeadline).HasColumnType("datetime");
            _ = entity.Property(e => e.StartDate).HasColumnType("datetime");

            _ = entity.HasOne(d => d.CommissioningWorks).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.CommissioningWorksId)
                .HasConstraintName("FK_Procurements_ConmmisioningWorks");

            _ = entity.HasOne(d => d.ExecutionState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ExecutionStateId)
                .HasConstraintName("FK_Procurements_ExecutionStates");

            _ = entity.HasOne(d => d.Law).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.LawId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procurements_Laws");

            _ = entity.HasOne(d => d.LegalEntity).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.LegalEntityId)
                .HasConstraintName("FK_Procurements_LegalEntities");

            _ = entity.HasOne(d => d.Method).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK_Procurements_Methods");

            _ = entity.HasOne(d => d.Minopttorg).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.MinopttorgId)
                .HasConstraintName("FK_Procurements_Minopttorgs");

            _ = entity.HasOne(d => d.Organization).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procurements_Organizations");

            _ = entity.HasOne(d => d.Platform).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("FK_Procurements_Platforms");

            _ = entity.HasOne(d => d.ProcurementState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ProcurementStateId)
                .HasConstraintName("FK_Procurements_ProcurementStates");

            _ = entity.HasOne(d => d.Region).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Procurements_Regions");

            _ = entity.HasOne(d => d.RepresentativeType).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.RepresentativeTypeId)
                .HasConstraintName("FK_Procurements_RepresentativeTypes");

            _ = entity.HasOne(d => d.ShipmentPlan).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ShipmentPlanId)
                .HasConstraintName("FK_Procurements_ShipmentPlans");

            _ = entity.HasOne(d => d.SignedOriginal).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.SignedOriginalId)
                .HasConstraintName("FK_Procurements_SignedOriginals");

            _ = entity.HasOne(d => d.TimeZone).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.TimeZoneId)
                .HasConstraintName("FK_Procurements_TimeZones");

            _ = entity.HasOne(d => d.WarrantyState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.WarrantyStateId)
                .HasConstraintName("FK_Procurements_WarrantyStates");
        });

        _ = modelBuilder.Entity<ProcurementState>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_StatusesOfTender");
        });

        _ = modelBuilder.Entity<ProcurementsDocument>(entity =>
        {
            _ = entity.HasNoKey();

            _ = entity.HasOne(d => d.Document).WithMany()
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsDocuments_Documents");

            _ = entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsDocuments_Procurements");
        });

        _ = modelBuilder.Entity<ProcurementsEmployee>(entity =>
        {
            _ = entity.HasNoKey();

            _ = entity.HasOne(d => d.Employee).WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsEmployees_Employees");

            _ = entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsEmployees_Procurements");
        });

        _ = modelBuilder.Entity<ProcurementsPreference>(entity =>
        {
            _ = entity.HasNoKey();

            _ = entity.HasOne(d => d.Preference).WithMany()
                .HasForeignKey(d => d.PreferenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsPreferences_Preferences");

            _ = entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsPreferences_Procurements");
        });

        _ = modelBuilder.Entity<RepresentativeType>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_RepresentativeOrAdmin");
        });

        _ = modelBuilder.Entity<ShipmentPlan>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_ShipmentPlan");
        });

        _ = modelBuilder.Entity<Tag>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_TagsForParsing");
        });

        _ = modelBuilder.Entity<TimeZone>(entity =>
        {
            _ = entity.Property(e => e.Offset).HasMaxLength(50);
        });

        _ = modelBuilder.Entity<WarrantyState>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK_StatusesForWarranty");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}