using Aspire.Net.ApiService.Application.Services;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Domain.Settings;
using Aspire.Net.ApiService.Infrastrutura.Brokers;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Aspire.Net.ApiService.Infrastrutura.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddTransient<PagamentoProducerMQ>();
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));


var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = new ConfigurationOptions
    {
        EndPoints = { { redisSettings.Host, redisSettings.Port } },
        User = redisSettings.User,
        Password = redisSettings.Password,
        AbortOnConnectFail = false
    };

    return ConnectionMultiplexer.Connect(options);
});
builder.Services.AddSingleton<ICacheRepository, RedisCacheRepository>();
builder.Services.AddSingleton<CacheService>();

string? connectionString = builder.Configuration.GetConnectionString("PostgreConnection");

builder.Services.AddHealthChecks()
    .AddRedis($"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password},abortConnect=false", name: "redis")
    .AddNpgSql(connectionString, name: "postgresql", failureStatus: HealthStatus.Unhealthy, tags: ["db", "sql", "postgres"]);

builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(10000); 
    options.AddHealthCheckEndpoint("API", "/health");
}).AddInMemoryStorage();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                    options.UseNpgsql(connectionString, npgsqlOptions =>
                                                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"))
                .UseSnakeCaseNamingConvention());

// Configuração do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configuração do Swagger com JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aspire.Net API",
        Version = "v1",
        Description = "API com autenticação JWT"
    });

    // Configuração para JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == 401)
    {
        await response.WriteAsync("Unauthorized: Token inválido ou expirado");
    }
    else if (response.StatusCode == 403)
    {
        await response.WriteAsync("Forbidden: Acesso negado");
    }
});

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aspire.Net API v1");
    });

    // Configurar endpoint de health check
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // importante!
    });
    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/hc-ui";
        options.ApiPath = "/hc-json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
