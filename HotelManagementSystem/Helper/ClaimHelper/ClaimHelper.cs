using System.Globalization;
using System.Security.Claims;

namespace HotelManagementSystem.Helper.ClaimHelper
{
    public class ClaimHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        public static int GetRoleId(ClaimsPrincipal user)
        {
            //if (user?.Identity?.IsAuthenticated != true)
            //throw new UnauthorizedAccessException("User is not authenticated.");

            var claim = user.FindFirst("RoleId");
            var value = claim?.Value;
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("RoleId claim is missing or empty.");

            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var roleId))
                throw new InvalidOperationException($"RoleId claim has invalid integer value: '{value}'.");

            return roleId;
        }
    }
}
