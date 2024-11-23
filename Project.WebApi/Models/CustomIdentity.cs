using System.Security.Principal;

namespace Project.WebApi.Models;

public class CustomIdentity : IIdentity
{
    public int Id { get; set; }
    public string Role { get; set; }
    public string AuthenticationType { get; set; }
    public bool IsAuthenticated { get; set; }
    public string Name { get; set; }
}