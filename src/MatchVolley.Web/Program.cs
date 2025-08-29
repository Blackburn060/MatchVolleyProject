using MatchVolley.Web.Components;
using MatchVolley.Application;
using MatchVolley.Infrastructure;
using MatchVolley.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Razor Components (Blazor .NET 8) em modo Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Identity UI padrão (precisa Razor Pages)
builder.Services.AddRazorPages();

// App/Application
builder.Services.AddMediatR(typeof(AssemblyMarker));

// Infra (DbContext, Identity, TenantProvider)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Endpoints
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<MatchVolleyDbContext>();
    await db.EnsureMigratedAndSeededAsync(sp);
}

app.Run();
