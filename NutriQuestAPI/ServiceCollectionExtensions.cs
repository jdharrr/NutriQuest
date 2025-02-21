using DatabaseServices;
using GeolocationServices;
using NutriQuestServices;
using StackExchange.Redis;

namespace NutriQuestAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("DatabaseSettings"));
        services.AddSingleton<MongoService>();
        services.AddScoped(typeof(DatabaseService<>));

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<FoodService>();
        services.AddScoped<StoreService>();
        services.AddScoped<GeolocationService>();

        return services;
    }

    public static IServiceCollection ConfigureGoogleApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleApiSettings>(configuration.GetSection("ApiKeys"));
        services.AddHttpClient<GoogleApi>();
        services.AddScoped<GoogleApi>();
        
        return services;
    }

    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]!)
        );

        return services;
    }
}
