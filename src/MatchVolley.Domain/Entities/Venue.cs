// Domain/Entities/Venue.cs
namespace MatchVolley.Domain.Entities;

public sealed class Venue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Court> Courts { get; set; } = new List<Court>();
}
