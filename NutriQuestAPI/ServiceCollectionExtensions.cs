using DatabaseServices;
using NutriQuestServices;

namespace NutriQuestAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoService = new MongoService(configuration.GetSection("DatabaseSettings").Get<MongoSettings>()!);
        services.AddSingleton(mongoService);
        services.AddScoped(typeof(DatabaseService<>));

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<FoodService>();

        return services;
    }
}
