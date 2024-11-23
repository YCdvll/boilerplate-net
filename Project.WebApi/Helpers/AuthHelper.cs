using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Project.Common.DataAccess.Models.User;
using Project.Common.Services.Models;

namespace Project.WebApi.Helpers;

public class AuthHelper
{
    // helper methods

    public static string GenerateJwtToken(AppSettings appSettings, User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new Claim(ClaimTypes.Name, user.Email), new Claim("Role", user.RoleId.ToString()) }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string HashPassword(string password)
    {
        using (var sha256Hash = SHA256.Create())
        {
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));

            return builder.ToString();
        }
    }

    public static Tuple<bool, string> PasswordPolicy(string password)
    {
        if (string.IsNullOrEmpty(password))
            return new Tuple<bool, string>(false, "Merci de renseigner un mot de passe valide");

        var hasNumber = new Regex(@"[0-9]+");
        var hasMiniMaxChars = new Regex(@".{6,12}");

        if (!hasMiniMaxChars.IsMatch(password))
            return new Tuple<bool, string>(false, "Votre mot de passe doit avoir entre 6 et 12 caratères");
        if (!hasNumber.IsMatch(password))
            return new Tuple<bool, string>(false, "Votre mot de passe doit contenir au moins 1 chiffre");

        return new Tuple<bool, string>(true, "");
    }
}