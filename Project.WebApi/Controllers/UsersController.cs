using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Common.DataAccess.Models.User;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Services;
using Project.WebApi.Models;
using Project.WebApi.Services;

namespace Project.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public UsersController(IBlobStorageService blobStorageService, IUserService userService, ITokenService tokenService)
    {
        _blobStorageService = blobStorageService;
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpGet]
    [Route("Users")]
    [Authorize(Policy = "Admin")]
    public async Task<IEnumerable<User>> UsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);
        return users;
    }

    [HttpGet]
    [Route("UserInfos/{id}")]
    public async Task<User> GetUserInfosAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        return user;
    }

    [HttpGet]
    [Route("UserInfos")]
    public async Task<ApiResponse> GetUserInfosAsync(CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = false;
        var userEmail = User.Identity!.Name;
        if (userEmail == null)
            throw new Exception("User email is null");
        var response = await _userService.GetUserByEmailAsync(userEmail, cancellationToken);
        if (response != null)
        {
            result.Success = true;
            result.Data = response;
        }

        return result;
    }

    [HttpPost]
    [Route("UpdateUserInfos")]
    public async Task<ApiResponse> UpdateUserInfosAsync(User user, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserInfosAsync(user, cancellationToken);
        return result;
    }

    [HttpGet]
    [Route("UserRoles/{id}")]
    public async Task<ApiResponse> UserRolesAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserRolesAsync(id, cancellationToken);

        return result;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("ActivateAccount")]
    public async Task<ApiResponse> ActivateAccountAsync(string t, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = false;
        result.Message = "Une erreur est survenue";

        var isValidToken = _tokenService.GetEmailFromToken(t);
        var user = await _userService.GetUserByEmailAsync(isValidToken, cancellationToken);

        if (user != null)
        {
            user.IsActivated = true;
            await _userService.UpdateUserInfosAsync(user, cancellationToken);
            result.Success = true;
            result.Message = "Votre compte a été activé avec succès";
            result.Data = user.Email;
            return result;
        }

        return result;
    }

    [HttpGet]
    [Route("IsLoggedIn")]
    public ApiResponse IsLoggedIn(CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = User.Identity!.IsAuthenticated;
        return result;
    }

    [HttpGet]
    [Route("IsAdmin")]
    public ApiResponse IsAdmin(CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = User.Claims.Any(x => x.Type == "Role" && x.Value == "1");
        return result;
    }

    [HttpGet]
    [Route("GetUsersCount")]
    public async Task<ApiResponse> GetUsersCountAsync(CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = true;
        result.Data = await _userService.GetUsersCountAsync(cancellationToken);
        return result;
    }
}