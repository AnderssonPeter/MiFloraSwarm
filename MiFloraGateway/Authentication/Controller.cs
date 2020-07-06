using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiFloraGateway.Authentication
{

    [Route("Authentication")]
    public class Controller : IdentityBaseController
    {
        private readonly ILogger<Controller> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public Controller(ILogger<Controller> logger, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager) : base(userManager)
        {            
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// Login, this will create a authentication cookie!
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login/{username}/{password}")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(UserModel))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.Unauthorized, type: typeof(ErrorResult))]
        public async Task<ActionResult<UserModel>> Login([FromRoute]string username, [FromRoute]string password)
        {
            var user = await userManager.FindByNameAsync(username);
            var result = await signInManager.PasswordSignInAsync(user, password, true, false);
            if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                var isAdmin = await userManager.IsInRoleAsync(user, Roles.Admin);
                return Ok(new UserModel { Username = user.UserName ?? "N/A", IsAdmin = isAdmin });
            }
            else
            {
                return this.Unauthorized(new ErrorResult
                {
                    ErrorMessage = "Invalid username and/or password",
                    Errors = new List<ErrorResultField>()
                }); ;
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            return await CreateUserAsync(model.Username, model.Password, model.IsAdmin);
        }

        /// <summary>
        /// Check if any users have been created yet.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCurrentUser")]
        public ActionResult<UserModel> GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new UserModel { Username = User.Identity.Name ?? "N/A", IsAdmin = User.IsInRole(Roles.Admin) });
            }
            return NotFound();
        }
    }
}
