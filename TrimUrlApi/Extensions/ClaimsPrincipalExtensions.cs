using System.Security.Claims;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;

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

        public static UserRole? GetAuthUserRole(this ClaimsPrincipal user)
        {
            var roleNum = user.FindFirst("roleNum")?.Value;
            if (roleNum == null)
            {
                return null;
            }

            var roleNumInt = Int32.Parse(roleNum);
            return (UserRole)roleNumInt;
        }

        public static bool HasAdminPrivileges(this ClaimsPrincipal user)
        {
            var role = user.GetAuthUserRole();
            return role is UserRole.Admin or UserRole.Owner;
        }

        public static bool HasOwnerPrivileges(this ClaimsPrincipal user)
        {
            var role = user.GetAuthUserRole();
            return role is UserRole.Owner;
        }
    }
}
