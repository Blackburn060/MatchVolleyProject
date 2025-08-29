using MatchVolley.Application.Abstractions;
using MatchVolley.Infrastructure.Identity;
using MatchVolley.Infrastructure.Persistence;
using MatchVolley.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MatchVolley.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, HttpTenantProvider>();

        services.AddDbContext<MatchVolleyDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        // expõe IAppDbContext como MatchVolleyDbContext
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<MatchVolleyDbContext>());

        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<MatchVolleyDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
