// src/MatchVolley.Infrastructure/Tenancy/HttpTenantProvider.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace MatchVolley.Infrastructure.Tenancy;

public sealed class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor;
    private readonly Guid? _defaultTenantId; // lido de env/appsettings

    public HttpTenantProvider(IHttpContextAccessor accessor, IConfiguration config)
    {
        _accessor = accessor;

        // 1) tenta variável de ambiente: DEFAULT_TENANT_ID
        if (Guid.TryParse(Environment.GetEnvironmentVariable("DEFAULT_TENANT_ID"), out var envId))
            _defaultTenantId = envId;

        // 2) se não tiver env, tenta appsettings: Tenancy:DefaultTenantId
        if (_defaultTenantId is null &&
            Guid.TryParse(config["Tenancy:DefaultTenantId"], out var cfgId))
            _defaultTenantId = cfgId;
    }

    public Guid CurrentTenantId => TryGet(out var id) ? id : Guid.Empty;

    public bool TryGet(out Guid tenantId)
    {
        tenantId = Guid.Empty;
        var ctx = _accessor.HttpContext;

        // Claim (usuário autenticado)
        var fromClaim = ctx?.User?.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(fromClaim, out tenantId)) return true;

        // Header (requests públicas/admin)
        var fromHeader = ctx?.Request.Headers["X-Tenant"].FirstOrDefault();
        if (Guid.TryParse(fromHeader, out tenantId)) return true;

        // Fallback: Default (env/appsettings)
        if (_defaultTenantId is Guid def && def != Guid.Empty)
        {
            tenantId = def;
            return true;
        }

        return false; // sem tenant resolvido
    }
}