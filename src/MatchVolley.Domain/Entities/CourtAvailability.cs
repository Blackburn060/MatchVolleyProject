// Domain/Entities/CourtAvailability.cs
namespace MatchVolley.Domain.Entities;

public sealed class CourtAvailability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CourtId { get; set; }
    public Court Court { get; set; } = default!;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal Price { get; set; }
    public bool IsBlocked { get; set; }
}
