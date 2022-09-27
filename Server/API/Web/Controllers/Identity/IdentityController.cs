namespace Web.Controllers.Identity
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Shared;

    using Application.Handlers.Identity.Commands.Register;

    using Web.Extensions;

    public class IdentityController : ApiController
    {
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        [SwaggerOperation("Register a user.", "")]
        public async Task<ActionResult<string>> Register(UserRegisterCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }
    }
}