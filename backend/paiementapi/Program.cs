// <copyright file="Program.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using paiementapi.Data;
using paiementapi.Helpers;
using paiementapi.Interfaces;
using paiementapi.Options;
using paiementapi.Repository;
using paiementapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PaiementAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

Env.Load(); // load environment variable in .env

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new Exception("CONNECTION_STRING is missing in .env");

// JWT Options Configuration
builder.Services.Configure<JwtOptions>(options =>
{
    options.Key = Environment.GetEnvironmentVariable("JWT_KEY")
        ?? throw new Exception("JWT_KEY is missing in .env");
    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
        ?? throw new Exception("JWT_ISSUER is missing in .env");
    options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        ?? throw new Exception("JWT_AUDIENCE is missing in .env");
    options.ExpirationHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "1");
});

// EF Core with PostgreSQL
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

// DI for application services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllers();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

async Task SeedRolesAsync(IServiceProvider services)
{    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.MapControllers();
await SeedRolesAsync(app.Services);
app.Run();
