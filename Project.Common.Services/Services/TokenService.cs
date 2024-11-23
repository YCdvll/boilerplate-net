using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Project.Common.Services.Services;

public interface ITokenService
{
    string GenerateToken(string userEmail, DateTime expirationDate);
    string GetEmailFromToken(string token);
}

public class TokenService : ITokenService
{
    private readonly string _secretKey;

    public TokenService(string secretKey)
    {
        _secretKey = secretKey;
    }

    public string GenerateToken(string userEmail, DateTime expirationDate)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("email", userEmail) }),
            Expires = expirationDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GetEmailFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_secretKey);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email");

        return emailClaim?.Value;
    }
}