namespace Project.WebApi.Models;

public class AccountResponse
{
    public AccountResponse(string message, bool succes)
    {
        Message = message;
        Success = succes;
    }

    public string Message { get; set; }
    public bool Success { get; set; }
}