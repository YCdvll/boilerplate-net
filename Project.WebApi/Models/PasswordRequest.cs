using System.ComponentModel.DataAnnotations;

namespace Project.WebApi.Models;

public class PasswordRequest
{
    [Required] public string Password { get; set; }

    [Required] public string Token { get; set; }
}