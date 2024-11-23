using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Models;
using Project.WebApi.Models;

namespace Project.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMailService _mailService;

    public AdminController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost]
    [Route("SendMail")]
    [Authorize(Policy = "Admin")]
    public async Task<ApiResponse> SendMailAsync(ContactForm mail, CancellationToken cancellationToken)
    {
        var result = new ApiResponse
        {
            Success = false,
            Message = "Not implemented"
        };

        var mailData = new
        {
            Name = mail.Info,
            Title = mail.Subject,
            Content = mail.Message,
            LinkText = "Voir le site",
            Link = "/"
        };

        var mailTemplate = await _mailService.BuildTemplateHtmlAsync(mailData, MailTemplateEnum.Link, cancellationToken);
        await _mailService.SendMailAsync(mail.Email, mail.Subject, mailTemplate, "contact@domain.io", cancellationToken);

        if (mailData != null)
        {
            result.Success = true;
            result.Message = "Mail sent";
        }

        return result;
    }
}