using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nursing_Home.Application.Common.Security;
using Nursing_Home.Application.Repositories;
using Nursing_Home.Application.Services;
using Nursing_Home.Domain.Entities.Identities;
using Nursing_Home.Infrastructure.Persistence.Data;
using Nursing_Home.Infrastructure.Persistence.Interceptors;
using Nursing_Home.Infrastructure.Persistence.SendData;
using Nursing_Home.Infrastructure.Repositories;
using Nursing_Home.Infrastructure.Services;

namespace Nursing_Home.Infrastructure;
public static class DependecyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddDefaultIdentity();
        services.AddRepository();
        services.AddServices();
        services.AddInitialiseDatabase();
        services.AddConfigureSettingServices(configuration);
    }
    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        string defaultConnection = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
           options.UseMySql(defaultConnection, ServerVersion.AutoDetect(defaultConnection),
               builder =>
               {
                   builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                   builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
               })
                  .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
                  .EnableSensitiveDataLogging()
                  .UseLazyLoadingProxies()
                  //.UseProjectables()
                  .EnableDetailedErrors()
                  );
    }
    private static void AddDefaultIdentity(this IServiceCollection services)
    {

        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 0;
            //options.User.RequireUniqueEmail = true;

            // Lockout configuration
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout time of 5 minutes
            options.Lockout.MaxFailedAccessAttempts = 5; // Lockout after 5 failed attempts
            options.Lockout.AllowedForNewUsers = true;

        }).AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();
    }
    private static void AddRepository(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
    private static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<IJwtService, JwtService>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<ICacheService, CacheService>()
            ;
    }
    private static void AddInitialiseDatabase(this IServiceCollection services)
    {
        services
            .AddScoped<BaseDataInitial>();
    }
    private static void AddConfigureSettingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
    }

    public static async Task UseInitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<BaseDataInitial>();

        if (app.Environment.IsDevelopment())
        {

            await initialiser.MigrateAsync();
            await initialiser.SeedAsync();
        }

        if (app.Environment.IsProduction())
        {

            await initialiser.MigrateAsync();
            await initialiser.SeedAsync();
        }

    }
}
