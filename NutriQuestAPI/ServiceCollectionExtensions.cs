using AuthenticationServices;
using CacheServices;
using DatabaseServices;
using GeolocationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using NutriQuestServices;
using NutriQuestServices.FoodServices;
using StackExchange.Redis;
using Azure.Identity;
using EmailServices;
using NutriQuestServices.UserServices;

namespace NutriQuestAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureKeyVault(this IServiceCollection services, IConfiguration configuration)
    {
        var vaultUrl = configuration["KeyVault:Url"];
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            // Needed for Rider as it does not register Azure login into DefaultAzureCredential
            ExcludeInteractiveBrowserCredential = false  
        });
        var keyVaultConfig = new ConfigurationBuilder().AddConfiguration(configuration)
                                                       .AddAzureKeyVault(new Uri(vaultUrl!), credential)
                                                       .Build();

        services.AddSingleton<IConfiguration>(keyVaultConfig);

        return services;
    }

    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfigureOptions<MongoSettings>, MongoSettingsConfiguration>();
        services.AddSingleton<MongoService>();
        services.AddScoped(typeof(DatabaseService<>));

        return services;
    }

    public static IServiceCollection ConfigureNutriQuestServices(this IServiceCollection services)
    {
        services.AddScoped<FoodService>();
        services.AddScoped<StoreService>();
        services.AddScoped<GeolocationService>();
        services.AddScoped<UserService>();

        return services;
    }

    public static IServiceCollection ConfigureGoogleApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfigureOptions<GoogleApiSettings>, GoogleApiSettingsConfiguration>();
        services.AddHttpClient<GoogleApi>();
        services.AddScoped<GoogleApi>();
        
        return services;
    }

    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfigureOptions<RedisSettings>, RedisSettingsConfiguration>();
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
        services.AddSingleton<IConfigureOptions<JwtSettings>, JwtSettingsConfiguration>();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthenticationService>();

        return services;
    }

    public static IServiceCollection ConfigureEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfigureOptions<EmailSettings>, EmailSettingsConfiguration>();
        services.AddScoped<EmailService>();

        return services;
    }
}
