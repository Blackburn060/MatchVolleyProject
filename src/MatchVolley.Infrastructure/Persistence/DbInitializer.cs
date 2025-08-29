using MatchVolley.Domain.Entities;
using MatchVolley.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MatchVolley.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task EnsureMigratedAndSeededAsync(this MatchVolleyDbContext db,
        IServiceProvider sp, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        // Tenant demo
        var demoTenant = await db.Tenants.FirstOrDefaultAsync(ct)
                         ?? (await db.Tenants.AddAsync(new Tenant { Name = "Demo Tenant" }, ct)).Entity;

        await db.SaveChangesAsync(ct);

        // Usuário admin demo
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        var adminEmail = "admin@demo.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                TenantId = demoTenant.Id,
                FullName = "Demo Admin",
                IsCompanyUser = true
            };
            await userManager.CreateAsync(admin, "Admin@123!");
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("tenant_id", demoTenant.Id.ToString()));
        }

        // Venue/Court/Availability demo
        if (!await db.Venues.AnyAsync(ct))
        {
            var v = new Venue { TenantId = demoTenant.Id, Name = "Arena Central", City = "São Paulo", State = "SP" };
            var c1 = new Court { TenantId = demoTenant.Id, Venue = v, Name = "Quadra 1", Surface = "Areia" };
            var c2 = new Court { TenantId = demoTenant.Id, Venue = v, Name = "Quadra 2", Surface = "Areia" };

            var now = DateTime.UtcNow.Date.AddHours(12); // hoje 12:00 UTC
            var slots = Enumerable.Range(0, 4).Select(i => new CourtAvailability
            {
                TenantId = demoTenant.Id,
                Court = c1,
                StartUtc = now.AddHours(i),
                EndUtc = now.AddHours(i + 1),
                Price = 80
            }).ToList();

            await db.AddAsync(v, ct);
            await db.AddRangeAsync(slots, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
