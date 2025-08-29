// Domain/Entities/Tenant.cs
namespace MatchVolley.Domain.Entities;

public sealed class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}