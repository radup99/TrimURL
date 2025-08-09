using System.Security.Claims;
using TrimUrlApi.Entities;

namespace TrimUrlApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetAuthUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return sub != null ? Int32.Parse(sub) : null;
        }

        public static string? GetAuthUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("username")?.Value;
        }
    }
}
