using MatchVolley.Application.Abstractions;
using MatchVolley.Domain.Entities;
using MatchVolley.Infrastructure.Identity;
using MatchVolley.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MatchVolley.Infrastructure.Persistence;

public sealed class MatchVolleyDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
{
    private readonly ITenantProvider _tenantProvider;

    public MatchVolleyDbContext(DbContextOptions<MatchVolleyDbContext> options, ITenantProvider tenantProvider)
        : base(options) => _tenantProvider = tenantProvider;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<CourtAvailability> CourtAvailabilities => Set<CourtAvailability>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Índices e constraints
        b.Entity<Court>().HasIndex(x => new { x.TenantId, x.VenueId, x.IsActive });
        b.Entity<CourtAvailability>().HasIndex(x => new { x.TenantId, x.CourtId, x.StartUtc });
        b.Entity<Booking>().HasIndex(x => new { x.TenantId, x.CourtId, x.StartUtc });

        // Filtros multi-tenant
        b.Entity<Venue>().HasQueryFilter(x => x.TenantId == _tenantProvider.CurrentTenantId);
        b.Entity<Court>().HasQueryFilter(x => x.TenantId == _tenantProvider.CurrentTenantId);
        b.Entity<CourtAvailability>().HasQueryFilter(x => x.TenantId == _tenantProvider.CurrentTenantId);
        b.Entity<Booking>().HasQueryFilter(x => x.TenantId == _tenantProvider.CurrentTenantId);

        // Relacionamentos básicos
        b.Entity<Court>()
            .HasOne(c => c.Venue)
            .WithMany(v => v.Courts)
            .HasForeignKey(c => c.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Preenche TenantId automaticamente em entidades multi-tenant
        var tid = _tenantProvider.CurrentTenantId;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            switch (entry.Entity)
            {
                case Venue v when v.TenantId == Guid.Empty:
                    v.TenantId = tid; break;
                case Court c when c.TenantId == Guid.Empty:
                    c.TenantId = tid; break;
                case CourtAvailability a when a.TenantId == Guid.Empty:
                    a.TenantId = tid; break;
                case Booking bk when bk.TenantId == Guid.Empty:
                    bk.TenantId = tid; break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
