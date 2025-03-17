using System.Security.Claims;

namespace Chess;

public interface IIdentityProvider
{
    string UserId { get; }
}

public class IdentityProvider(IHttpContextAccessor contextAccessor) : IIdentityProvider
{
    private HttpContext Context => contextAccessor.HttpContext ?? throw new Exception("Context is null");

    public string UserId => Context.User.UserId();
}

public static class UserExt
{
    public static string UserId(this ClaimsPrincipal user) =>
        user.Claim(ClaimTypes.NameIdentifier) ?? throw new Exception("Unauthorized");

    public static string? Claim(this ClaimsPrincipal user, string type)
    {
        var claim = user.FindFirst(type);
        return claim?.Value;
    }
}