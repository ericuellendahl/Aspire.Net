using Aspire.Net.Web.ApiEndPoints;
using Aspire.Net.Web.Components;
using Aspire.Net.Web.Security;
using Aspire.Net.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<AccessTokenService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<AuthApiClientService>();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient("ApiClient", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5000/");
    });

builder.Services.AddHttpClient<ProductApiClientService>(client =>
    {
        //client.BaseAddress = new("https+http://apiservice");
        client.BaseAddress = new("http://localhost:5000/");
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddScheme<CustomOption, JWTAuthenticationHandler>(
    "JWTAuth", options => { }
    );
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

await app.RunAsync();
