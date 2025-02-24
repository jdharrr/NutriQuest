using AuthenticationServices;
using CacheServices;
using DatabaseServices;
using GeolocationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NutriQuestServices;
using NutriQuestServices.FoodService;
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

    public static IServiceCollection ConfigureNutriQuestServices(this IServiceCollection services)
    {
        services.AddScoped<FoodService>();
        services.AddScoped<StoreService>();
        services.AddScoped<GeolocationService>();

        return services;
    }

    public static IServiceCollection ConfigureGoogleApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleApiSettings>(configuration.GetSection("Apis"));
        services.AddHttpClient<GoogleApi>();
        services.AddScoped<GoogleApi>();
        
        return services;
    }

    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            var options = ConfigurationOptions.Parse(settings.RedisConnection);
            

            return ConnectionMultiplexer.Connect(options);
        });
        services.AddScoped<CacheService>();

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthenticationService>();

        return services;
    }
}
