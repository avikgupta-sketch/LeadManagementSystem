using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LMS.Data.Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration config)
    {
        // Roles
        string[] roles = { "Admin", "Manager", "Agent" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        // Admin Config
        var adminSection = config.GetSection("AdminSeed");

        var email = adminSection["Email"] ?? "admin@lms.com";
        var password = adminSection["Password"] ?? "Admin@123";
        var fullName = adminSection["FullName"] ?? "System Admin";

        var existingAdmin = await userManager.FindByEmailAsync(email);

        if (existingAdmin == null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}