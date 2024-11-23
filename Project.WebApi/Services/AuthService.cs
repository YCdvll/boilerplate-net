using Microsoft.Extensions.Options;
using Project.Common.DataAccess.Models.User;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Helpers;
using Project.Common.Services.Models;
using Project.Common.Services.Services;
using Project.WebApi.Helpers;
using Project.WebApi.Models;

namespace Project.WebApi.Services;

public interface IAuthService
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, CancellationToken cancellationToken);
    Task<AccountResponse> CreateUserAccountAsync(User user, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse> ResetPasswordAsync(string user, CancellationToken cancellationToken);
    Task<ApiResponse> SetPasswordAsync(PasswordRequest user, CancellationToken cancellationToken);
}

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IMailService _mailService;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthService(IOptions<AppSettings> appSettings, IUserService userService, IMailService mailService, ITokenService tokenService)
    {
        _appSettings = appSettings.Value;
        _userService = userService;
        _mailService = mailService;
        _tokenService = tokenService;
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByEmailAsync(model.Email, cancellationToken);

        // return null if user not found
        if (user == null)
            return new AuthenticateResponse("Identifiant ou mot de passe incorrect", false, 0);

        if (string.IsNullOrEmpty(user.Email) || user.Password != AuthHelper.HashPassword(model.Password))
            return new AuthenticateResponse("Identifiant ou mot de passe incorrect", false, user.Id, user.Email);

        if (user.IsActivated == false)
            return new AuthenticateResponse("Votre compte n'est pas encore activé", false, user.Id, user.Email);

        // authentication successful so generate jwt token
        var token = AuthHelper.GenerateJwtToken(_appSettings, user);

        user.LastConnexion = DateTime.Now;
        await _userService.UpdateUserInfosAsync(user, cancellationToken);

        return new AuthenticateResponse("Bienvenue", true, user.Id, user.Email, $"{user.FirstName} {user.LastName}", token, user.RoleId.ToString(), user.ProfilePictureUrl!);
    }

    public async Task<AccountResponse> CreateUserAccountAsync(User user, CancellationToken cancellationToken)
    {
        if (user == null)
            return new AccountResponse("Formulaire vide", false);

        if (string.IsNullOrEmpty(user.Email))
            return new AccountResponse("Merci de renseigner un email", false);

        if (string.IsNullOrEmpty(user.FirstName))
            return new AccountResponse("Merci de renseigner un Prénom", false);

        if (string.IsNullOrEmpty(user.LastName))
            return new AccountResponse("Merci de renseigner un Nom", false);

        if (string.IsNullOrEmpty(user.Password))
            return new AccountResponse("Merci de renseigner un mot de passe valide", false);

        var checkPassword = AuthHelper.PasswordPolicy(user.Password);
        if (!checkPassword.Item1)
            return new AccountResponse(checkPassword.Item2, checkPassword.Item1);

        var isExist = await _userService.GetUserAsync(user.Email, cancellationToken);
        if (isExist != null)
            return new AccountResponse("Une erreur est survenue", false);

        user.Password = AuthHelper.HashPassword(user.Password);
        user.RoleId = 2; //User

        user.UserPreferences = new UserPreferences
        {
            Updated = DateTime.Now,
            NewsLetter = true,
            EmailNotification = true
        };

        await _userService.SaveUserAsync(user, cancellationToken);


        var token = _tokenService.GenerateToken(user.Email, DateTime.Now.AddDays(2));
        var activationLink = $"/login?t={token}";

        var mailData = new
        {
            Name = InputHelper.GetShortName(user!.FirstName, user.LastName),
            Link = activationLink
        };

        // var htmlTemplate = await _mailService.BuildTemplateHtmlAsync(mailData, MailTemplateEnum.Welcome, cancellationToken);
        // await _mailService.SendMailAsync(user.Email, "Bienvenue sur domain", htmlTemplate, null, cancellationToken);

        return new AccountResponse($"Bienvenue {user.FirstName}, un mail vous a été envoyé pour valider votre compte.", true);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _userService.GetUsersAsync(cancellationToken);
    }

    public async Task<ApiResponse> ResetPasswordAsync(string user, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = false;
        result.Message = $"Un email a été envoyé à l'adresse suivante: {user} pour réinitialiser votre mot de passe.";

        var userExist = await _userService.GetUserAsync(user, cancellationToken);

        if (userExist != null)
        {
            var token = _tokenService.GenerateToken(userExist.Email, DateTime.Now.AddDays(1));
            var resetLink = $"/resetPassword?t={token}";

            var mailData = new
            {
                Name = InputHelper.GetShortName(userExist!.FirstName, userExist.LastName),
                Link = resetLink
            };

            var htmlTemplate = await _mailService.BuildTemplateHtmlAsync(mailData, MailTemplateEnum.PasswordReset, cancellationToken);
            await _mailService.SendMailAsync(userExist.Email, "domain : Réinitialisation de mot de passe", htmlTemplate, null, cancellationToken);

            result.Success = true;
            return result;
        }

        return result;
    }

    public async Task<ApiResponse> SetPasswordAsync(PasswordRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = false;

        var user = _tokenService.GetEmailFromToken(request.Token);
        var userExist = await _userService.GetUserAsync(user, cancellationToken);

        if (userExist != null)
        {
            userExist.Password = AuthHelper.HashPassword(request.Password);
            await _userService.SetNewPasswordAsync(userExist, cancellationToken);

            var mailData = new
            {
                Name = InputHelper.GetShortName(userExist!.FirstName, userExist.LastName),
                Title = "Changement de mot de passe",
                Content = "Votre mot de passe a été modifié avec succès. Vous pouvez vous connecter avec votre nouveau mot de passe.",
                Link = "/login",
                LinkText = "Se connecter"
            };

            var htmlTemplate = await _mailService.BuildTemplateHtmlAsync(mailData, MailTemplateEnum.Link, cancellationToken);
            await _mailService.SendMailAsync(userExist.Email, "domain : Changement mot de passe réussi", htmlTemplate, null, cancellationToken);

            result.Success = true;
            return result;
        }

        return result;
    }
}