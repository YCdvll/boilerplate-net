using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Project.Common.Services.Models;

namespace Project.Common.Services.ExternalServices;

public interface IMailService
{
    Task SendMailAsync(string toEmail, string subject, string content, string? sender, CancellationToken cancellationToken);
    Task<string> BuildTemplateHtmlAsync(dynamic dynamicTemplateData, MailTemplateEnum templateMail, CancellationToken cancellationToken);
}

public class MailService : IMailService
{
    private const string NoreplyEmail = "contact@domain.io";
    private readonly AppSettings _appSettings;
    private readonly IBlobStorageService _blobStorageService;

    public MailService(AppSettings appSettings, IBlobStorageService blobStorageService)
    {
        _appSettings = appSettings;
        _blobStorageService = blobStorageService;
    }

    public async Task SendMailAsync(string toEmail, string subject, string content, string? sender, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient();
        using var message = new MimeMessage();

        if (_appSettings.IsDebug)
            toEmail = "contact@domain.io";
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Domain", sender ?? NoreplyEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = content };
            var smtp = new SmtpClient();
            await smtp.ConnectAsync(_appSettings.EmailSmtp, 587, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_appSettings.EmailUser, _appSettings.EmailSecret, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception e)
        {
            throw new ApplicationException("Erreur lors de l'envoi de l'email.");
        }
    }

    public async Task<string> BuildTemplateHtmlAsync(dynamic dynamicTemplateData, MailTemplateEnum templateMail, CancellationToken cancellationToken)
    {
        var templateMailName = GetTemplateFileName(templateMail);
        var fileTemplate = await _blobStorageService.GetFileAsync($"mail/{templateMailName}.html", ContainerName.Public, cancellationToken);
        var stream = new StreamReader(new MemoryStream(fileTemplate));
        var emailTemplate = await stream.ReadToEndAsync(cancellationToken);

        var contentResult = FillTemplateData(dynamicTemplateData, emailTemplate);
        return contentResult;
    }

    private string FillTemplateData(dynamic dynamicTemplateData, string htmlContent)
    {
        var properties = dynamicTemplateData.GetType().GetProperties();
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(dynamicTemplateData);

            if (propertyName == "Link")
                propertyValue = $"{_appSettings.FrontDomain}{propertyValue}";

            htmlContent = htmlContent.Replace($"{{{{{propertyName}}}}}", propertyValue.ToString());
        }

        return htmlContent;
    }

    private string GetTemplateFileName(MailTemplateEnum templateMail)
    {
        return templateMail switch
        {
            MailTemplateEnum.Basic => "template-mail",
            MailTemplateEnum.Welcome => "template-welcome",
            MailTemplateEnum.TripWarning => "template-trip-warning",
            MailTemplateEnum.PasswordReset => "template-reset-password",
            MailTemplateEnum.Feedback => "template-feedback",
            MailTemplateEnum.Link => "template-mail-link",
            _ => "template-mail"
        };
    }
}