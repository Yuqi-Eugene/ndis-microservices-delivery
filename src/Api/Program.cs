using System.Text;
using Api.Auth;
using Api.Data;
using Api.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using MediatR;
using Api.Middlewares;

// Program.cs is the application's composition root.
// In ASP.NET Core, this is where you register services, configure middleware,
// and define how the process starts.
var builder = WebApplication.CreateBuilder(args);

// Adds MVC controller support so attribute-routed API controllers can handle HTTP requests.
builder.Services.AddControllers();

// Registers EF Core with PostgreSQL.
// AppDbContext will be resolved from DI anywhere it is needed.
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Identity manages users, passwords, and roles.
// This project stores Identity data in the same PostgreSQL database via AppDbContext.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Read JWT settings once during startup so authentication can validate tokens later.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Fail fast if required auth settings are missing.
// Startup validation is better than discovering bad configuration only after the first login request.
if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT settings missing. Set Jwt:Key, Jwt:Issuer, Jwt:Audience in appsettings.Development.json");
}

builder.Services
    .AddAuthentication(options =>
    {
        // Every [Authorize] endpoint will use JWT bearer tokens unless overridden.
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // These rules define what a "valid" token means for this API.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Application services used by handlers/controllers.
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Registers MediatR so controllers can send commands/queries instead of calling handlers directly.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Discovers and registers FluentValidation validators from this project assembly.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Pipeline behaviors wrap every MediatR request.
// ValidationBehavior runs before the handler so bad requests fail early and consistently.
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Api.Application.Common.Behaviors.ValidationBehavior<,>));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Swagger is enabled for local development to make the API easy to inspect and test.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NDIS Delivery API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed roles and a default admin account.
// This keeps local development friction low and gives the smoke test a known admin login.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var roles = new[] { "Admin", "Provider" };
    foreach (var role in roles)
    {
        // Role creation is idempotent: only create missing roles.
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    if (app.Environment.IsDevelopment())
    {
        var adminEmail = app.Configuration["Seed:AdminEmail"] ?? "admin@ndis.local";
        var adminPassword = app.Configuration["Seed:AdminPassword"] ?? "Admin123$";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            // The seeded admin exists only for development convenience.
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(admin, adminPassword);
            if (create.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    // Swagger UI is intentionally limited to Development.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception translation should run early so downstream failures become consistent ProblemDetails responses.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// Order matters:
// 1. Authentication reads the bearer token and builds HttpContext.User.
// 2. Authorization evaluates [Authorize] attributes using that user principal.
app.UseAuthentication();
app.UseAuthorization();

// Connect attribute-routed controllers to the request pipeline.
app.MapControllers();

app.Run();
