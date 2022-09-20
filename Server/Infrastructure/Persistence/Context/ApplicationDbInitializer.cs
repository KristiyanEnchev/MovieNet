namespace Persistence.Context
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Domain.Entities;

    using Npgsql;

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (!await DatabaseExistsAsync())
                {
                    _logger.LogInformation("Database does not exist. Creating it now...");
                    await CreateDatabaseAsync();
                }

                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch (NpgsqlException ex) when (ex.Message.Contains("does not exist"))
            {
                _logger.LogInformation("Database does not exist.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the database exists.");
                throw;
            }
        }

        private async Task CreateDatabaseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            var administratorRole = new UserRole("Administrator")
            {
                Description = "Admin Role"
            };

            var userRole = new UserRole("User")
            {
                Description = "User Role"
            };

            if (!_roleManager.Roles.Any(r => r.Name == administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
            }

            if (!_roleManager.Roles.Any(r => r.Name == userRole.Name))
            {
                await _roleManager.CreateAsync(userRole);
            }

            var adminEmail = "admin@admin.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedBy = "Initial Seed",
                    EmailConfirmed = true
                };

                var createAdminResult = await _userManager.CreateAsync(adminUser, "123456");
                if (createAdminResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(adminUser, new[] { administratorRole.Name });
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
                }
            }

            var userEmail = "user@example.com";
            var standardUser = await _userManager.FindByEmailAsync(userEmail);
            if (standardUser == null)
            {
                standardUser = new User
                {
                    Id = "58efac42-e31d-4039-bfe1-76672c615dd5",
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "User",
                    LastName = "One",
                    IsActive = true,
                    CreatedBy = "Initial Seed",
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(standardUser, "123456");
                if (createUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(standardUser, new[] { userRole.Name });
                }
                else
                {
                    _logger.LogError("Failed to create standard user: {Errors}", string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}