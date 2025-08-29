// Domain/Entities/Booking.cs
namespace MatchVolley.Domain.Entities;

public sealed class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CourtId { get; set; }
    public Court Court { get; set; } = default!;

    // evitamos dependência com Identity no domínio:
    public string UserId { get; set; } = default!;

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
