using Azure.Security.KeyVault.Secrets;
using Project.Common.Services.Models;

namespace Project.WebApi.Helpers;

public class AzureSettings
{
    public async Task<SecretKeys> GetAzureSecretAsync(SecretClient azureSecretClient)
    {
        var result = new SecretKeys();
        var sqlSecretQuery = await azureSecretClient.GetSecretAsync("sqlConnectionStringProd");
        var hashApiKeyQuery = await azureSecretClient.GetSecretAsync("secretKey");
        var sendGridTokenQuery = await azureSecretClient.GetSecretAsync("sendGridToken");
        var blobConnexionStringQuery = await azureSecretClient.GetSecretAsync("azureStorageConnexionString");

        result.SqlConnectionString = sqlSecretQuery.Value.Value;
        result.HashApiKey = hashApiKeyQuery.Value.Value;
        result.SendGridToken = sendGridTokenQuery.Value.Value;
        result.BlobConnexionString = blobConnexionStringQuery.Value.Value;

        return result;
    }
}