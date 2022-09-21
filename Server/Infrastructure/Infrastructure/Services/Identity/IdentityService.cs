namespace Infrastructure.Services.Identity
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using Shared;

    using Domain.Entities;

    using Infrastructure.Services.Helpers;

    using Application.Interfaces;

    using Models.User;

    internal class IdentityService
    {
        private const string InvalidErrorMessage = "Invalid credentials.";

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IJwtService jwtGenerator;

        public IdentityService(UserManager<User> userManager, IJwtService jwtGenerator, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.jwtGenerator = jwtGenerator;
            this.signInManager = signInManager;
        }

        public async Task<UserResponseModel> GenerateToken(User user)
        {
            string ipAddress = IpHelper.GetIpAddress();

            var token = await jwtGenerator.GenerateTokenAsync(user, ipAddress);

            var newRefreshToken = await jwtGenerator.GenerateRefreshToken(user);

            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await userManager.UpdateAsync(user);

            var tokenResult = new UserResponseModel(token, user.RefreshTokenExpiryTime, newRefreshToken);

            return tokenResult;
        }
    }
}