using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Parsething.Entities;

public partial class ParsethingContext : DbContext
{
    public ParsethingContext()
    {
    }

    public ParsethingContext(DbContextOptions<ParsethingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommisioningWork> CommisioningWorks { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<ComponentCalculation> ComponentCalculations { get; set; }

    public virtual DbSet<ComponentState> ComponentStates { get; set; }

    public virtual DbSet<ComponentType> ComponentTypes { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ExecutionState> ExecutionStates { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Law> Laws { get; set; }

    public virtual DbSet<LegalEntity> LegalEntities { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Method> Methods { get; set; }

    public virtual DbSet<Minopttorg> Minopttorgs { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Platform> Platforms { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<Procurement> Procurements { get; set; }

    public virtual DbSet<ProcurementState> ProcurementStates { get; set; }

    public virtual DbSet<ProcurementsDocument> ProcurementsDocuments { get; set; }

    public virtual DbSet<ProcurementsEmployee> ProcurementsEmployees { get; set; }

    public virtual DbSet<ProcurementsPreference> ProcurementsPreferences { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RepresentativeType> RepresentativeTypes { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<ShipmentPlan> ShipmentPlans { get; set; }

    public virtual DbSet<SignedOriginal> SignedOriginals { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskExecutor> TaskExecutors { get; set; }

    public virtual DbSet<TimeZone> TimeZones { get; set; }

    public virtual DbSet<WarrantyState> WarrantyStates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Persist Security Info=False;User ID=ParsethingSolution;Password=PassSolution;Initial Catalog=Parsething;Data Source=176.112.98.217, 1433;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Comments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Employees");
        });

        modelBuilder.Entity<CommisioningWork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ConmmisioningWorks");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasOne(d => d.ComponentType).WithMany(p => p.Components)
                .HasForeignKey(d => d.ComponentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Components_ComponentTypes");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Components)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Components_Manufacturers");
        });

        modelBuilder.Entity<ComponentCalculation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CalculationAndPurchasingList");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(19, 2)");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComponentCalculations_Components");

            entity.HasOne(d => d.ComponentState).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ComponentStateId)
                .HasConstraintName("FK_ComponentCalculations_ComponentStates");

            entity.HasOne(d => d.Procurement).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComponentCalculations_Procurements");

            entity.HasOne(d => d.Seller).WithMany(p => p.ComponentCalculations)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComponentCalculations_Sellers");
        });

        modelBuilder.Entity<ComponentState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StatusesOfProduct");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Positions");
        });

        modelBuilder.Entity<ExecutionState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StatusesForExecution");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Chronology");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Histories)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_Employees");
        });

        modelBuilder.Entity<Minopttorg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Minopttorg");
        });

        modelBuilder.Entity<Procurement>(entity =>
        {
            entity.Property(e => e.ActDate).HasColumnType("datetime");
            entity.Property(e => e.ActualDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Bet).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            entity.Property(e => e.ConclusionDate).HasColumnType("datetime");
            entity.Property(e => e.ContractAmount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.CorrectionDate).HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.Fas).HasColumnName("FAS");
            entity.Property(e => e.InitialPrice).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Inn).HasColumnName("INN");
            entity.Property(e => e.MaxAcceptanceDate).HasColumnType("datetime");
            entity.Property(e => e.MaxDueDate).HasColumnType("datetime");
            entity.Property(e => e.MinimalPrice).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.ProtocolDate).HasColumnType("datetime");
            entity.Property(e => e.RealDueDate).HasColumnType("datetime");
            entity.Property(e => e.ReserveContractAmount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.SigningDate).HasColumnType("datetime");
            entity.Property(e => e.SigningDeadline).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.CommissioningWorks).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.CommissioningWorksId)
                .HasConstraintName("FK_Procurements_ConmmisioningWorks");

            entity.HasOne(d => d.ExecutionState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ExecutionStateId)
                .HasConstraintName("FK_Procurements_ExecutionStates");

            entity.HasOne(d => d.Law).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.LawId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procurements_Laws");

            entity.HasOne(d => d.LegalEntity).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.LegalEntityId)
                .HasConstraintName("FK_Procurements_LegalEntities");

            entity.HasOne(d => d.Method).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK_Procurements_Methods");

            entity.HasOne(d => d.Minopttorg).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.MinopttorgId)
                .HasConstraintName("FK_Procurements_Minopttorgs");

            entity.HasOne(d => d.Organization).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procurements_Organizations");

            entity.HasOne(d => d.Platform).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("FK_Procurements_Platforms");

            entity.HasOne(d => d.ProcurementState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ProcurementStateId)
                .HasConstraintName("FK_Procurements_ProcurementStates");

            entity.HasOne(d => d.Region).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Procurements_Regions");

            entity.HasOne(d => d.RepresentativeType).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.RepresentativeTypeId)
                .HasConstraintName("FK_Procurements_RepresentativeTypes");

            entity.HasOne(d => d.ShipmentPlan).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.ShipmentPlanId)
                .HasConstraintName("FK_Procurements_ShipmentPlans");

            entity.HasOne(d => d.SignedOriginal).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.SignedOriginalId)
                .HasConstraintName("FK_Procurements_SignedOriginals");

            entity.HasOne(d => d.TimeZone).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.TimeZoneId)
                .HasConstraintName("FK_Procurements_TimeZones");

            entity.HasOne(d => d.WarrantyState).WithMany(p => p.Procurements)
                .HasForeignKey(d => d.WarrantyStateId)
                .HasConstraintName("FK_Procurements_WarrantyStates");
        });

        modelBuilder.Entity<ProcurementState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StatusesOfTender");
        });

        modelBuilder.Entity<ProcurementsDocument>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Document).WithMany()
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsDocuments_Documents");

            entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsDocuments_Procurements");
        });

        modelBuilder.Entity<ProcurementsEmployee>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Employee).WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsEmployees_Employees");

            entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsEmployees_Procurements");
        });

        modelBuilder.Entity<ProcurementsPreference>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Preference).WithMany()
                .HasForeignKey(d => d.PreferenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsPreferences_Preferences");

            entity.HasOne(d => d.Procurement).WithMany()
                .HasForeignKey(d => d.ProcurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProcurementsPreferences_Procurements");
        });

        modelBuilder.Entity<RepresentativeType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RepresentativeOrAdmin");
        });

        modelBuilder.Entity<ShipmentPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ShipmentPlan");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TagsForParsing");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasOne(d => d.Developer).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.DeveloperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Developers");
        });

        modelBuilder.Entity<TaskExecutor>(entity =>
        {
            entity.HasOne(d => d.Developer).WithMany(p => p.TaskExecutors)
                .HasForeignKey(d => d.DeveloperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskExecutors_Developers");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskExecutors)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskExecutors_Statuses");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskExecutors)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskExecutors_Tasks");
        });

        modelBuilder.Entity<TimeZone>(entity =>
        {
            entity.Property(e => e.Offset).HasMaxLength(50);
        });

        modelBuilder.Entity<WarrantyState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StatusesForWarranty");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
