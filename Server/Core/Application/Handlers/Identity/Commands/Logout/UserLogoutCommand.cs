namespace Application.Handlers.Identity.Commands.Logout
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    using Models.User;

    using Application.Interfaces;

    using Shared;

    public class UserLogoutCommand : UserLogoutModel, IRequest<Result<string>>
    {
        public UserLogoutCommand(string email)
            : base(email)
        {
        }

        public class UserLogoutCommandHandler : IRequestHandler<UserLogoutCommand, Result<string>>
        {
            private readonly IIdentityService identity;

            public UserLogoutCommandHandler(IIdentityService identity)
            {
                this.identity = identity;
            }

            public async Task<Result<string>> Handle(UserLogoutCommand request, CancellationToken cancellationToken)
            {
                var result = await identity.LogoutAsync(request.Email);

                return result;
            }
        }
    }
}