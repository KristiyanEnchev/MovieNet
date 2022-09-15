namespace Persistence.Context
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    public class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ApplicationDbInitializer(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.MigrateAsync();
            await SeedUsersAsync();
        }

        private async Task SeedUsersAsync()
        {
            if (!await _userManager.Users.AnyAsync())
            {
                var user = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user, "Admin123!");
            }
        }
    }
}
