# .NET 8 Clean Architecture Starter Project


## 1. Architecture & Structure

The project follows **Clean Architecture** with 4 layers:

- **Api** (Presentation Layer)
- **Application** (Business Logic, Use Cases, DTOs, Validation)
- **Domain** (Entities, ValueObjects, Business Rules)
- **Infrastructure** (EF Core, PostgreSQL, Repositories, External Services)

### Dependency Injection (DI) Best Practice

- **Each layer should provide a static DependencyInjection class with an extension method (e.g. `AddApplication`, `AddInfrastructure`) to register its own services.**
- In `Program.cs`, call these extension methods to register all services, keeping the startup code clean and maintainable.

**Example:**

In `Application/DependencyInjection.cs`:
csharp
public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    // Register MediatR, AutoMapper, FluentValidation, etc.
    return services;
  }
}

In `Infrastructure/DependencyInjection.cs`:
csharp
public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
  {
    // Register DbContext, repositories, etc.
    return services;
  }
}

In `Program.cs`:
csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


tests/

### Recommended Solution Folder Structure

src/
 ├── Api                # Main API project (entry point)
 │   ├── Controllers/
 │   ├── Middleware/
 │   ├── Extensions/
 │   ├── appsettings.json   # API config (Kestrel, ConnectionStrings, etc.)
 │   └── ...
 ├── Application        # Application logic (reference only)
 │   ├── DTOs/
 │   ├── Validators/
 │   ├── Extensions/
 │   ├── Interfaces/
 │   ├── Services/
 │   └── ...
 ├── Domain             # Domain model (reference only)
 │   ├── Entities/
 │   ├── ValueObjects/
 │   ├── Enums/
 │   ├── Events/
 │   ├── Exceptions/
 │   └── ...
 ├── Infrastructure     # Infrastructure (reference only)
 │   ├── Extensions/
 │   ├── Interfaces/
 │   ├── Services/
 │   ├── Repositories/
 │   ├── External/
 │   └── ...

 ├── UnitTests/
 ├── IntegrationTests/


> **Note:**
> - The Api folder is the entry point for running and debugging the application. Use `dotnet run --project src/Api/Api.csproj`.
> - Place appsettings.json (for Kestrel, ConnectionStrings, etc.) inside the src/Api folder, not the root.
> - For environment-specific configuration, create `appsettings.Development.json`, `appsettings.Staging.json`, and appsettings.Production.json in the same src/Api folder. .NET will automatically load the file that matches the current environment (set by `ASPNETCORE_ENVIRONMENT`).
> - Example:
>   - appsettings.json (default, shared config)
>   - appsettings.Development.json (overrides for development)
>   - appsettings.Production.json (overrides for production)
> - Example content for `appsettings.Development.json`:
>   
json
>   {
>     "ConnectionStrings": {
>       "DefaultConnection": "Host=localhost;Database=mydb_dev;Username=postgres;Password=devpass"
>     }
>   }
>   

> - To run with a specific environment: ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/Api/Api.csproj
> - All other folders (Application, Domain, Infrastructure) are reference libraries only, not entry points.
> **Note:**
> - แนะนำให้แยกโฟลเดอร์ Interfaces, Services, Entities, DTOs, Validators, ValueObjects, Enums, Events, Exceptions, Repositories, External, Middleware, Extensions ตามแต่ละ layer เพื่อความเป็นระเบียบและขยายง่าย
> - สามารถเพิ่มโฟลเดอร์ย่อยอื่น ๆ ตามความเหมาะสมของแต่ละโปรเจกต์



## 2.1 Kestrel Port Configuration

**Configure Kestrel to use a specific port (e.g., 5000):**

**appsettings.json**
json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://0.0.0.0:5000"
    }
  }
}

**Program.cs**
csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});

> This will make your API start on port 5000 as configured in appsettings.json.

**PostgreSQL + EF Core 8** (Code-First + Migrations).

**Connection string:**

json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=123456"
}


**Sample BaseEntity:**

csharp
public abstract class BaseEntity
{
  public int Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
}

**Sample User Entity (support External Auth):**

csharp
public class User : BaseEntity
{
  public string Name { get; set; }
  public string Email { get; set; }
  public string? Provider { get; set; } // "Google", "Facebook", "Local"
  public string? ProviderId { get; set; } // รหัสผู้ใช้จาก provider
  public string? PictureUrl { get; set; }
}

**Sample DbContext:**

csharp
public class AppDbContext : DbContext
{
  public DbSet<User> Users { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}

Run migrations:

bash
dotnet ef migrations add InitialCreate -p Infrastructure -s Api
dotnet ef database update -p Infrastructure -s Api

---

## 3. API Layer
### Controller Rules
- **Thin Controllers**: Only receive/return data — logic in Application Services
- **No Database Access**: Use Application Services only
- Use DTOs/ViewModels for requests/responses (never return Domain entities directly)
- Routing: [Route("api/[controller]")] with REST conventions
- Always validate input with ModelState.IsValid or FluentValidation
- **ProducesResponseType Attributes**: Document all possible HTTP status codes with response types
  
- REST Controller Example:

csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = new { Id = id, Name = "Test User" };
        return Ok(new { success = true, data = user, error = (string)null });
    }
}

- Swagger at /swagger
- API Versioning
- Standard Response: { success, data, error }

---

## 4. Security

- JWT Authentication
- Role-based Authorization

csharp
[Authorize(Roles = "Admin")]
[HttpGet("admin")]
public IActionResult GetAdminData()
{
    return Ok(new { success = true, data = "Secret", error = (string)null });
}

- CORS Policy

csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

---

## 5. Application Layer

- CQRS with MediatR
- DTOs with AutoMapper
- FluentValidation

csharp
public record CreateUserCommand(string Name) : IRequest<int>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // save user
        return 1;
    }
}

---

## 6. Observability

- **Logging**: Serilog
- **Error Handling**: Global Middleware
- **Health Check**: /health

---

## 7. External Authentication

Support **Google** and **Facebook** login.

### Example (Google)

csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

### Example (Facebook)

csharp
builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    });

---

## 8. Configuration & Environment Separation

- Main config: **`appsettings.json`**
- Environment specific: **`appsettings.Development.json`**, **`appsettings.Staging.json`**, **`appsettings.Production.json`**

**Example: appsettings.json**

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mydb;Username=postgres;Password=123456"
  },
  "Authentication": {
    "Jwt": {
      "Key": "supersecretkey",
      "Issuer": "myapi",
      "Audience": "myapi_users"
    },
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "Facebook": {
      "AppId": "your-facebook-app-id",
      "AppSecret": "your-facebook-app-secret"
    }
  }
}

**Bind to Strongly Typed Config:**

csharp
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Authentication:Jwt"));

---

## 9. Deployment

- Dockerfile
- docker-compose (API + PostgreSQL)
- Run migrations on startup

---

## 10. Testing

- Unit Tests with xUnit
- Integration Tests with EF Core + PostgreSQL + Testcontainers
