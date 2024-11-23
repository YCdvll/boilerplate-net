using Project.Common.Services.Models;
using Project.WebApi.Services;

namespace Project.WebApi;

public static class DependenciesRegistrar
{
    public static void Register(AppSettings appsettings, IServiceCollection services, SecretKeys secretKeys)
    {
        RegisterDependencies(appsettings, services, secretKeys);
        RegisterServices(services);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IWebSocketService, WebSocketService>();
    }

    private static void RegisterDependencies(AppSettings appsettings, IServiceCollection services, SecretKeys secretKeys)
    {
        try
        {
            Common.Services.DependenciesRegistrar.Register(appsettings, services, secretKeys);
            Common.DataAccess.DependenciesRegistrar.Register(services, secretKeys);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ApplicationException("RegisterDependencies extService failed");
        }
    }
}