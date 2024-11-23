namespace Project.Common.Services.Models;

public class SecretKeys
{
    public string? SqlConnectionString { get; set; }
    public string BlobConnexionString { get; set; }
    public string HashApiKey { get; set; }
}