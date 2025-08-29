using MatchVolley.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchVolley.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Venue> Venues { get; }
    DbSet<Court> Courts { get; }
    DbSet<CourtAvailability> CourtAvailabilities { get; }
    DbSet<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
