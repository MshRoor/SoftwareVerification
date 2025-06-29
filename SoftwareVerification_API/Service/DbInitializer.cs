using AppModels.Common;
using DataAccess.Data;
using SoftwareVerification_API.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SoftwareVerification_API.Service
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_db.Roles.Any(x => x.Name == SD.Role_Admin)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Name = "First Run User",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _db.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
