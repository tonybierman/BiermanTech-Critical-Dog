using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Data
{
    public static class IdentityDbInitializer
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider, string adminEmail, string adminPassword)
        {
            // Get required services
            var identityContext = serviceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Apply migrations
                await identityContext.Database.MigrateAsync();

                // Create Admin role
                string adminRole = "Admin";
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                // Create Admin user
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }
                    else
                    {
                        throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the admin user.");
                throw;
            }
        }

        public static async Task SeedRegularUser(IServiceProvider serviceProvider, string userEmail, string userPassword)
        {
            // Get required services
            var identityContext = serviceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Apply migrations
                await identityContext.Database.MigrateAsync();

                // Create Regular user
                var regularUser = await userManager.FindByEmailAsync(userEmail);
                if (regularUser == null)
                {
                    regularUser = new IdentityUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(regularUser, userPassword);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create regular user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the regular user.");
                throw;
            }
        }
    }
}