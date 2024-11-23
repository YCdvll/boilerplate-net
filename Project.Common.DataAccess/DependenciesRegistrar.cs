using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Project.Common.DataAccess.DataBaseFactory;
using Project.Common.Services.Models;

namespace Project.Common.DataAccess;

public static class DependenciesRegistrar
{
    // public static void Register(IServiceCollection services, SecretKeys secretKeys)
    // {
    //     services.AddDbContext<SqlDbContext>(options => options.UseMySql(secretKeys.SqlConnectionString, ServerVersion.AutoDetect(secretKeys.SqlConnectionString)));
    // }

    public static void Register(IServiceCollection services, SecretKeys secretKeys)
    {
        services.AddDbContext<SqlDbContext>(options => ConfigureDbContext(options, secretKeys));
    }

    private static void ConfigureDbContext(DbContextOptionsBuilder options, SecretKeys secretKeys)
    {
        try
        {
            options.UseMySql(secretKeys.SqlConnectionString, ServerVersion.AutoDetect(secretKeys.SqlConnectionString));
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            throw new InvalidOperationException("La connexion à la base de données a échoué. Veuillez vérifier les paramètres de connexion ou l'état du serveur.");
        }
    }
}