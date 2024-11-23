using Microsoft.EntityFrameworkCore;
using Project.Common.DataAccess.DataBaseFactory;
using Project.Common.DataAccess.Models.User;
using Project.Common.Services.ExternalServices;
using Project.Common.Services.Helpers;
using Project.WebApi.Models;

namespace Project.WebApi.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> GetUserAsync(string email, CancellationToken cancellationToken);
    Task<string> SaveUserAsync(User user, CancellationToken cancellationToken);
    Task<ApiResponse> UpdateUserInfosAsync(User user, CancellationToken cancellationToken);
    Task<ApiResponse> GetUserRolesAsync(int userId, CancellationToken cancellationToken);
    Task<ApiResponse> SetNewPasswordAsync(User user, CancellationToken cancellationToken);
    Task<int> GetUsersCountAsync(CancellationToken cancellationToken);
}

internal class UserService : IUserService
{
    private readonly IMailService _mailService;
    private readonly SqlDbContext _sqlDbContext;

    public UserService(SqlDbContext sqlDbContext, IMailService mailService)
    {
        _sqlDbContext = sqlDbContext;
        _mailService = mailService;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var query = from user in _sqlDbContext.User
            join address in _sqlDbContext.PostalAddress
                on user.Id equals address.UserId into groupingAddress
            from address in groupingAddress.DefaultIfEmpty()
            select new User
            {
                Id = user.Id,
                LastConnexion = user.LastConnexion,
                IsActivated = user.IsActivated,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                RoleId = user.RoleId,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PostalAddress = address
            };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _sqlDbContext.User.SingleAsync(x => x.Id == id, cancellationToken);

        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var query = from user in _sqlDbContext.User
            join address in _sqlDbContext.PostalAddress
                on user.Id equals address.UserId into grouping
            from address in grouping.DefaultIfEmpty()
            where user.Email == email
            select new User
            {
                Id = user.Id,
                LastConnexion = user.LastConnexion,
                IsActivated = user.IsActivated,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                RoleId = user.RoleId,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PostalAddress = address
            };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<User> GetUserAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _sqlDbContext.User.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return user;
    }

    public async Task<string> SaveUserAsync(User user, CancellationToken cancellationToken)
    {
        await _sqlDbContext.User.AddAsync(user, cancellationToken);
        await _sqlDbContext.SaveAsync(cancellationToken);

        return user.Id.ToString();
    }
    

    public async Task<ApiResponse> UpdateUserInfosAsync(User user, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();

        if (InputHelper.StringFormat(user.FirstName, 2, 20))
        {
            result.Message = "Merci de renseigner un prénom valide";
            result.Success = false;
            return result;
        }

        if (InputHelper.StringFormat(user.LastName, 2, 20))
        {
            result.Message = "Merci de renseigner un nom valide";
            result.Success = false;
            return result;
        }

        _sqlDbContext.User.Update(user);
        await _sqlDbContext.SaveAsync(cancellationToken);

        result.Message = "Vos informations ont bien été mise à jour";
        result.Success = true;

        return result;
    }

    public async Task<ApiResponse> GetUserRolesAsync(int roleId, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();
        result.Success = true;
        result.Data = await _sqlDbContext.Role.Where(x => x.Id == roleId).Select(x => x.Name).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<ApiResponse> SetNewPasswordAsync(User user, CancellationToken cancellationToken)
    {
        var result = new ApiResponse();

        result.Success = true;
        _sqlDbContext.User.Update(user);
        await _sqlDbContext.SaveAsync(cancellationToken);

        return result;
    }

    public async Task<int> GetUsersCountAsync(CancellationToken cancellationToken)
    {
        return await _sqlDbContext.User.CountAsync(cancellationToken);
    }
}