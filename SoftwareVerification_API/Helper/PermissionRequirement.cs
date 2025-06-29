using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SoftwareVerification_API.Helper
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<XcelAppUser> _userManager;

        public PermissionAuthorizationHandler(UserManager<XcelAppUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (user == null)
                return;

            var claims = await _userManager.GetClaimsAsync(user);

            if (claims.Any(c => c.Type == "Permission" && c.Value == requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
