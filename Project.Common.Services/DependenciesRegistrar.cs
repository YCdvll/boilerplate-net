using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Models;
using Project.Common.Services.Services;

namespace Project.Common.Services;

public static class DependenciesRegistrar
{
    public static void Register(AppSettings appSettings, IServiceCollection services, SecretKeys secretKeys)
    {
        //System Services
        services.AddScoped<IMemoryCache, MemoryCache>();

        //Azure Services
        services.AddScoped<IBlobStorageService>(_ => new BlobStorageService(secretKeys.BlobConnexionString));
        //Internal Services
        services.AddScoped<ITokenService>(_ => new TokenService(secretKeys.HashApiKey));

        //External Services
        services.AddScoped<IMailService>(provider => new MailService(appSettings, provider.GetRequiredService<IBlobStorageService>()));
    }
}