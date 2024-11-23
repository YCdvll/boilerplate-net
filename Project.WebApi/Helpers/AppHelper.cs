using System.Security.Claims;

namespace Project.WebApi.Helpers;

public static class AppHelper
{
    public static int GetUserId(ClaimsIdentity? user)
    {
        var userIdClaim = ((ClaimsIdentity)user).FindFirst("id").Value;
        var id = Convert.ToInt32(userIdClaim);
        return id;
    }
}