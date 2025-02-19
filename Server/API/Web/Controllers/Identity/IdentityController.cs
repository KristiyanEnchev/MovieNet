﻿namespace Web.Controllers.Identity
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Shared;

    using Application.Handlers.Identity.Commands.Refresh;
    using Application.Handlers.Identity.Commands.Login;
    using Application.Handlers.Identity.Commands.Register;
    using Application.Handlers.Identity.Commands.Logout;

    using Web.Extensions;

    using Models.User;

    public class IdentityController : ApiController
    {
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        [SwaggerOperation("Register a user.", "")]
        public async Task<ActionResult<string>> Register(UserRegisterCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        [SwaggerOperation("Request an access token using credentials.", "")]
        public async Task<ActionResult<Result<UserResponseModel>>> Login(UserLoginCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }
        
        [AllowAnonymous]
        [HttpPost(nameof(Refresh))]
        [SwaggerOperation("Request an access token using a refresh token.", "")]
        public async Task<ActionResult<Result<UserResponseModel>>> Refresh(UserRefreshCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPost(nameof(Logout))]
        [SwaggerOperation("Logs out a user", "")]
        public async Task<ActionResult<Result<string>>> Logout(UserLogoutCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }
    }
}