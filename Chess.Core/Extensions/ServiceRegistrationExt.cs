using System.Reflection;
using Chess.Core.Manage;
using Chess.Core.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
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

        services.Configure<MongoOptions>(configuration.GetSection("MongoDbSettings") ??
                                         throw new NullReferenceException("MongoDbSettings is null"));
        return services;
    }

    public static void AddManagers(this IServiceCollection services)
    {
        var assembly = typeof(ServiceRegistrationExt).Assembly;

        var managers = assembly.DefinedTypes.Where(t => typeof(IManager).IsAssignableFrom(t) && !t.IsAbstract);
        foreach (var manager in managers)
        {
            services.AddScoped(manager);
        }

        services.AddSingleton<SessionManager>();
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

    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging =>
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


            configuration.ReadFrom.Configuration(context.Configuration,
                readerOptions: new ConfigurationReaderOptions(typeof(ConsoleLoggerConfigurationExtensions).Assembly));
        };

    public static void AddGoogleAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var googleIssuer = "https://accounts.google.com";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = googleIssuer;
                options.Audience = configuration["ValidAudience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = googleIssuer,
                    ValidateAudience = true,
                    ValidAudience = configuration["ValidAudience"],
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                        {
                            Console.WriteLine("Authed request via hub");
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }
}