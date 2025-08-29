using Microsoft.AspNetCore.Identity;

namespace MatchVolley.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    // Opcional: campos extras públicos
    public Guid? TenantId { get; set; }
    public string? FullName { get; set; }
    public bool IsCompanyUser { get; set; }
}
