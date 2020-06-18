using System.ComponentModel.DataAnnotations;
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

        public Controller(ILogger<Controller> logger, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager) : base(userManager)
        {            
            this.logger = logger;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Login, this will create a authentication cookie!
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login/{username}/{password}")]
        public async Task<ActionResult<UserModel>> Login([FromRoute]string username, [FromRoute]string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, true, false);
            if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                return Ok(new UserModel { Username = User.Identity.Name ?? "N/A", IsAdmin = User.IsInRole(Roles.Admin) });
            }
            else
            {
                return this.Unauthorized();
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
