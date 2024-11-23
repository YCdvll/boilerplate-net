namespace Project.Common.DataAccess.Models.User;

public class User : SqlRepository
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime LastConnexion { get; set; } = DateTime.UtcNow;
    public string Password { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int RoleId { get; set; }
    public bool IsActivated { get; set; }
    public PostalAddress? PostalAddress { get; set; }
    public UserPreferences? UserPreferences { get; set; }
}