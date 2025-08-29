namespace MatchVolley.Infrastructure.Tenancy;

public interface ITenantProvider
{
    Guid CurrentTenantId { get; }
}
