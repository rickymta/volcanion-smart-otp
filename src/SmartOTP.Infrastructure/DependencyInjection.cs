using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Infrastructure.Persistence;
using SmartOTP.Infrastructure.Persistence.Repositories;
using SmartOTP.Infrastructure.Services;

namespace SmartOTP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "SmartOTP_";
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
