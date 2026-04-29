using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Context;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<LeadRemark> LeadRemarks => Set<LeadRemark>();

    private bool IsSqlite =>
        Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 🔹 Soft Delete Filters
        builder.Entity<Lead>()
            .HasQueryFilter(l => !l.IsDeleted);

        builder.Entity<ApplicationUser>()
            .HasQueryFilter(u => !u.IsDeleted);

        // 🔹 User self reference (Manager → Agents)
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Manager)
            .WithMany(m => m.Agents)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔹 Lead → Assigned Agent
        builder.Entity<Lead>()
            .HasOne(l => l.AssignedAgent)
            .WithMany(u => u.AssignedLeads)
            .HasForeignKey(l => l.AssignedAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔹 LeadRemark → Lead
        builder.Entity<LeadRemark>()
            .HasOne(r => r.Lead)
            .WithMany(l => l.Remarks)
            .HasForeignKey(r => r.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔹 LeadRemark → ChangedBy
        builder.Entity<LeadRemark>()
            .HasOne(r => r.ChangedBy)
            .WithMany()
            .HasForeignKey(r => r.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);

        // ─────────────────────────────────────────────────────────────
        // SQLite shim for the SQL-Server-only [Timestamp] RowVersion
        // ─────────────────────────────────────────────────────────────
        // SQL Server auto-generates `rowversion`. SQLite does not, so
        // we tell EF to never auto-generate it on SQLite, and we set the
        // value ourselves in SaveChanges below. SQL Server users see no
        // change at all (this branch is skipped at runtime).
        if (IsSqlite)
        {
            builder.Entity<Lead>()
                .Property(l => l.RowVersion)
                .ValueGeneratedNever()
                .IsConcurrencyToken(false);
        }
    }

    public override int SaveChanges()
    {
        StampSqliteRowVersion();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampSqliteRowVersion();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void StampSqliteRowVersion()
    {
        if (!IsSqlite) return;

        foreach (var entry in ChangeTracker.Entries<Lead>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.RowVersion = Guid.NewGuid().ToByteArray();
            }
        }
    }
}
