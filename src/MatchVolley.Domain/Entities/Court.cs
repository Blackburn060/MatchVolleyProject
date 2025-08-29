// Domain/Entities/Court.cs
namespace MatchVolley.Domain.Entities;

public sealed class Court
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Surface { get; set; } = "Default";
    public bool IsActive { get; set; } = true;
}
