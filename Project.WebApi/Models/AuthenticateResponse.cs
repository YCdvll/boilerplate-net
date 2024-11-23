namespace Project.WebApi.Models;

public class AuthenticateResponse
{
    public AuthenticateResponse(string message, bool success, int id, string email = null, string name = null, string token = null, string role = null, string pict = null)
    {
        Message = message;
        Success = success;
        Id = id;
        Email = email;
        FullName = name;
        Token = token;
        Role = role;
        Picture = pict ?? "default_pict.jpg";
    }

    public string Message { get; set; }
    public bool Success { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
    public int Id { get; set; }
    public string Picture { get; set; }
}