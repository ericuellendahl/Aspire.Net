using Aspire.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

//builder.Configuration.Sources.Clear();
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
//    .AddEnvironmentVariables()
//    .AddCommandLine(args);

//var reverseProxySection = builder.Configuration.GetSection("ReverseProxy");

// Carrega as configurações do YARP
builder.Services.AddReverseProxy()
                //.LoadFromConfig(reverseProxySection)   
                .LoadFromMemory(BaseConfiguration.GetRoutes(), BaseConfiguration.GetClusters());

var app = builder.Build();

// Configuração para desenvolvimento - remove redirecionamento HTTPS
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

// Ativa o proxy reverso
app.MapReverseProxy();

await app.RunAsync();