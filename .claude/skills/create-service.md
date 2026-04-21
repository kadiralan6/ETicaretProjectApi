# Skill: Create New Microservice

Create a complete new microservice from scratch with all layers following the established architecture.

---

## Input Required

- Service name (e.g., Notification, Shipping, Review)
- Port number (next available: 5005+)
- Entities and their properties
- Dependencies on other services (events consumed/published)

---

## Project Structure to Create

```
src/Services/{ServiceName}/
  ETicaretAPI.Services.{ServiceName}.Domain/
  ETicaretAPI.Services.{ServiceName}.Application/
  ETicaretAPI.Services.{ServiceName}.Persistence/
  ETicaretAPI.Services.{ServiceName}.Infrastructure/
  ETicaretAPI.Services.{ServiceName}.WebAPI/
```

---

## Steps

### Step 1: Create Project Files

Create 5 `.csproj` files:

**Domain.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Domain/ETicaretAPI.Common.Domain.csproj" />
  </ItemGroup>
</Project>
```

**Application.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Domain/ETicaretAPI.Services.{ServiceName}.Domain.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Application/ETicaretAPI.Common.Application.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Domain/ETicaretAPI.Common.Domain.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Infrastructure/ETicaretAPI.Common.Infrastructure.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.SharedLibrary/ETicaretAPI.Common.SharedLibrary.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.*" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.*" />
  </ItemGroup>
</Project>
```

**Persistence.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Application/ETicaretAPI.Services.{ServiceName}.Application.csproj" />
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Domain/ETicaretAPI.Services.{ServiceName}.Domain.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Persistence/ETicaretAPI.Common.Persistence.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.*" />
  </ItemGroup>
</Project>
```

**Infrastructure.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Application/ETicaretAPI.Services.{ServiceName}.Application.csproj" />
  </ItemGroup>
</Project>
```

**WebAPI.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Application/ETicaretAPI.Services.{ServiceName}.Application.csproj" />
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Persistence/ETicaretAPI.Services.{ServiceName}.Persistence.csproj" />
    <ProjectReference Include="../../{ServiceName}/ETicaretAPI.Services.{ServiceName}.Infrastructure/ETicaretAPI.Services.{ServiceName}.Infrastructure.csproj" />
    <ProjectReference Include="../../../Common/ETicaretAPI.Common.Infrastructure/ETicaretAPI.Common.Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
  </ItemGroup>
</Project>
```

### Step 2: Add to Solution

```bash
dotnet sln ETicaretAPI.sln add \
  src/Services/{ServiceName}/ETicaretAPI.Services.{ServiceName}.Domain/ETicaretAPI.Services.{ServiceName}.Domain.csproj \
  src/Services/{ServiceName}/ETicaretAPI.Services.{ServiceName}.Application/ETicaretAPI.Services.{ServiceName}.Application.csproj \
  src/Services/{ServiceName}/ETicaretAPI.Services.{ServiceName}.Persistence/ETicaretAPI.Services.{ServiceName}.Persistence.csproj \
  src/Services/{ServiceName}/ETicaretAPI.Services.{ServiceName}.Infrastructure/ETicaretAPI.Services.{ServiceName}.Infrastructure.csproj \
  src/Services/{ServiceName}/ETicaretAPI.Services.{ServiceName}.WebAPI/ETicaretAPI.Services.{ServiceName}.WebAPI.csproj
```

### Step 3: Create DbContext

File: `Persistence/Context/{ServiceName}DbContext.cs`

```csharp
using ETicaretAPI.Services.{ServiceName}.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.{ServiceName}.Persistence.Context;

public class {ServiceName}DbContext : DbContext
{
    public {ServiceName}DbContext(DbContextOptions<{ServiceName}DbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<{Entity}> {Entity}s { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof({ServiceName}DbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

### Step 4: Create DI Registrations

**PersistenceServiceRegistration.cs:**
```csharp
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.{ServiceName}.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.{ServiceName}.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<{ServiceName}DbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("{ServiceName}Db")));

        services.AddScoped<IUnitOfWork>(sp =>
        {
            var context = sp.GetRequiredService<{ServiceName}DbContext>();
            return new ETicaretAPI.Common.Persistence.UnitOfWork.UnitOfWork(context);
        });

        // Repository registrations
        // services.AddScoped<I{Entity}Repository, {Entity}Repository>();

        return services;
    }
}
```

**ApplicationServiceRegistration.cs:**
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.{ServiceName}.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

        // Service registrations
        // services.AddScoped<I{Entity}Service, {Entity}Manager>();

        return services;
    }
}
```

**{ServiceName}ServiceRegistration.cs (Infrastructure):**
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Services.{ServiceName}.Infrastructure;

public static class {ServiceName}ServiceRegistration
{
    public static IServiceCollection Add{ServiceName}Services(this IServiceCollection services, IConfiguration configuration)
    {
        // Third-party service registrations

        return services;
    }
}
```

### Step 5: Create Program.cs

File: `WebAPI/Program.cs`

```csharp
using ETicaretAPI.Common.Infrastructure;
using ETicaretAPI.Services.{ServiceName}.Application;
using ETicaretAPI.Services.{ServiceName}.Infrastructure;
using ETicaretAPI.Services.{ServiceName}.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.Add{ServiceName}Services(builder.Configuration);
builder.Services.AddCommonInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Step 6: Create appsettings.json

File: `WebAPI/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "{ServiceName}Db": "Host=localhost;Port=5432;Database=eticaret_{service_lower};Username=postgres;Password=postgres"
  },
  "RedisConfiguration": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  },
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://localhost:{PORT}"
      }
    }
  }
}
```

### Step 7: Update Ocelot Gateway

Add routes to `src/ApiGateway/ocelot.json`:

```json
{
  "UpstreamPathTemplate": "/api/{serviceLower}/{everything}",
  "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"],
  "DownstreamPathTemplate": "/api/{serviceLower}/{everything}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "localhost",
      "Port": {PORT}
    }
  ]
}
```

### Step 8: Create Entities and Full Stack

Use the `create-entity.md` skill for each entity in the new service.

---

## Checklist

- [ ] 5 projects created with correct references
- [ ] All projects added to solution
- [ ] DbContext with `ApplyConfigurationsFromAssembly`
- [ ] DI registration files for all layers
- [ ] Program.cs follows the established pattern
- [ ] appsettings.json with all infrastructure connection strings
- [ ] Ocelot gateway routes added
- [ ] Initial migration created
- [ ] Service builds: `dotnet build ETicaretAPI.sln`
- [ ] Service runs on correct port
