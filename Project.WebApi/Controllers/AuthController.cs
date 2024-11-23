using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Common.DataAccess.Models;
using Project.Common.DataAccess.Models.User;
using Project.WebApi.Models;
using Project.WebApi.Services;

namespace Project.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("Login")]
    [AllowAnonymous]
    public async Task<AuthenticateResponse> LoginAsync(AuthenticateRequest userCred, CancellationToken cancellationToken)
    {
        var response = await _authService.AuthenticateAsync(userCred, cancellationToken);

        if (!response.Success)
            return response;
        var claims = new List<Claim>
        {
            new("id", response.Id.ToString()),
            new(ClaimTypes.Name, response.Email),
            new(ClaimTypes.Role, response.Role)
        };

        var customIdentity = new ClaimsIdentity(claims, "jwt-token");
        var claimsPrincipal = new ClaimsPrincipal(customIdentity);

        return response;
    }

    [HttpPost]
    [Route("CreateUserAccount")]
    [AllowAnonymous]
    public async Task<AccountResponse> CreateUserAccountAsync(User user, CancellationToken cancellationToken)
    {
        // return new AccountResponse("Merci pour l'intérêt que vous portez au site, mais les inscriptions ne sont pas encore ouvertes !!", false);

        var result = await _authService.CreateUserAccountAsync(user, cancellationToken);

        return result;
    }

    [HttpPost]
    [Route("ResetPassword")]
    [AllowAnonymous]
    public async Task<ApiResponse> ResetPasswordAsync(string user, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(user, cancellationToken);

        return result;
    }

    [HttpPost]
    [Route("SetNewPassword")]
    [AllowAnonymous]
    public async Task<ApiResponse> SetNewPasswordAsync(PasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.SetPasswordAsync(request, cancellationToken);

        return result;
    }
}