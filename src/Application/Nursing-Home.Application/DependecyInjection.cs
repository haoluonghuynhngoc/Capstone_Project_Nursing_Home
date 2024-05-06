using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Nursing_Home.Application;
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator();
        services.AddCachingRedis(configuration);
    }
    private static void AddCachingRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            //options.Configuration = "localhost";
            //options.InstanceName = "CleanArchitecture-";
            string cacheConnection = configuration.GetConnectionString("CacheConnection")!;
            options.Configuration = cacheConnection;

        });
        services.AddMemoryCache();
    }
    private static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        });
    }
}
