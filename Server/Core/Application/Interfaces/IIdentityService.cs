namespace Application.Interfaces
{
    using System.Threading.Tasks;

    using Models.User;

    using Shared;

    public interface IIdentityService
    {
        Task<Result<string>> Register(UserRegisterRequestModel userRequest);
        Task<Result<UserResponseModel>> Login(UserRequestModel userRequest);
        Task<Result<UserResponseModel>> RefreshTokenAsync(UserRefreshModel request);
        Task<Result<string>> LogoutAsync(string userEmail);
    }
}