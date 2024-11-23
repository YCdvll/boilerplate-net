using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Models;
using Project.WebApi.Models;
using Project.WebApi.Services;

namespace Project.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly IBlobStorageService _blobService;
    private readonly IUserService _userService;

    public ToolsController(IBlobStorageService blobService, IUserService userService)
    {
        _blobService = blobService;
        _userService = userService;
    }

    [HttpPost]
    [Route("UploadProfilePicture")]
    public async Task<ApiResponse> UploadProfilePictureAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file != null && file.Length > 0)
        {
            var fileName = file.FileName.ToLower();
            if (fileName.EndsWith(".jpg") == false && fileName.EndsWith(".jpeg") == false && fileName.EndsWith(".png") == false)
                return new ApiResponse { Message = "Le fichier doit être au format .jpg, .jpeg ou .png" };

            using (var stream = file.OpenReadStream())
            {
                var user = await _userService.GetUserByEmailAsync(User.Identity.Name, cancellationToken);

                if (user != null)
                {
                    user.ProfilePictureUrl = $"{user.Id}_pict.jpg";
                    await _blobService.UploadFileAsync($"users-img/{user.Id}_pict.jpg", ContainerName.Public, stream, cancellationToken);

                    await _userService.UpdateUserInfosAsync(user, cancellationToken);

                    return new ApiResponse { Message = "Le fichier a été téléchargé avec succès.", Success = true, Data = user.ProfilePictureUrl };
                }
            }
        }

        return new ApiResponse { Message = "Aucun fichier n'a été fourni." };
    }

    [HttpGet]
    [Route("GetFile/{id}")]
    public async Task<ActionResult> GetFileAsync(string id, CancellationToken cancellationToken)
    {
        var file = await _blobService.GetFileAsync(id, ContainerName.Gpx, cancellationToken);
        if (file != null && file.Length > 0) return File(file, "application/octet-stream", id);

        return NotFound("Le fichier demandé n'a pas été trouvé.");
    }
}