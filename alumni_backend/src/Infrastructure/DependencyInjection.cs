using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database Context - Using InMemory for development (PostgreSQL for production)
        var usePostgreSQLValue = configuration.GetSection("Database:UsePostgreSQL").Value;
        var usePostgreSQL = string.Equals(usePostgreSQLValue, "true", StringComparison.OrdinalIgnoreCase);
        
        Console.WriteLine($"🔍 Database config debug:");
        Console.WriteLine($"   UsePostgreSQL value: '{usePostgreSQLValue}'");
        Console.WriteLine($"   UsePostgreSQL bool: {usePostgreSQL}");
        
        services.AddDbContext<AppDbContext>(options =>
        {
            if (usePostgreSQL)
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                options.UseInMemoryDatabase("AlumniDemoDb");
            }
            
            // Enable sensitive data logging in development
            if (configuration.GetSection("Logging:IncludeScopes").Value == "true")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Register Repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAlumniProfileRepository, AlumniProfileRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        
        // Register External Services
        services.AddScoped<IImageStorageService, AwsS3ImageStorageService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<INotificationService, EmailNotificationService>();
        services.AddScoped<IPasswordService, PasswordService>();
        
        // Register Business Services
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        // Note: IOtpService is registered in Application layer

        // Register AWS S3 Client
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        return services;
    }
}