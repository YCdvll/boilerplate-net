using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.WebApi.Models;

namespace Project.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class Account : ControllerBase
{
    private readonly IConfiguration _configuration;

    public Account(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("Test")]
    [AllowAnonymous]
    public async Task<ApiResponse> TestAsync(CancellationToken cancellationToken)
    {
        var response = new ApiResponse();
        var sqlDbSettings = _configuration.GetSection("SqlDbSettings");
        var connectionString = sqlDbSettings.GetValue<string>("ConnectionString");
        response.Success = true;
        response.Data = "connectionString";

        return response;
    }
}