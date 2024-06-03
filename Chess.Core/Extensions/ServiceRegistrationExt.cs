using System.Reflection;
using Chess.Core.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;

namespace Chess.Core.Extensions;

public static class ServiceRegistrationExt
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        });

        services.AddScoped(sp =>
        {
            var databaseName = configuration.GetConnectionString("Database");
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(databaseName);
        });
        return services;
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        var assembly = typeof(ServiceRegistrationExt).Assembly;

        var managers = assembly.DefinedTypes.Where(t => t.IsSubclassOfGeneric(typeof(BaseManager<,,>)) && !t.IsAbstract);
        foreach (var manager in managers)
        {
            services.AddScoped(manager);
        }

        return services;
    }

    private static bool IsSubclassOfGeneric(this Type type, Type genericBaseType)
    {
        if (type == null || genericBaseType == null || !genericBaseType.IsGenericType)
        {
            return false;
        }

        if (genericBaseType.IsInterface)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericBaseType);
        }

        Type currentType = type;
        while (currentType != null && currentType != typeof(object))
        {
            var currentTypeInfo = currentType.GetTypeInfo();
            if (currentTypeInfo.IsGenericType && currentTypeInfo.GetGenericTypeDefinition() == genericBaseType)
            {
                return true;
            }

            currentType = currentTypeInfo.BaseType;
        }

        return false;
    }
     public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, configuration) =>
        {

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Destructure.ToMaximumDepth(9)
                .Filter.ByExcluding(e =>
                {
                    if (e.Properties.TryGetValue("RequestPath", out var path)
                        && path.ToString().Contains("/hc"))
                    {
                        return true;
                    }
                    return false;
                })
                .WriteTo.Console()
                .WriteTo.Debug();
            

            configuration.ReadFrom.Configuration(context.Configuration, readerOptions: new ConfigurationReaderOptions(typeof(ConsoleLoggerConfigurationExtensions).Assembly));
        };

     public static void AddGoogleAuth(this IServiceCollection services, IConfiguration configuration)
         => services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = configuration["ValidIssuer"],
                     ValidateAudience = true,
                     ValidAudience = configuration["ValidAudience"],
                     ClockSkew=TimeSpan.Zero
                 };
             });
}