using System.Threading.Tasks;
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
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<Controller> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public Controller(ILogger<Controller> logger, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Login, this will create a authentication cookie!
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            var result = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, true, false);
            if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                return Ok();
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
            return await CreateUser(model.Username, model.Password, model.IsAdmin);
        }

        /// <summary>
        /// Check if any users have been created yet.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HasUser")]
        public async Task<ActionResult<bool>> HasUser()
        {
            return Ok(await userManager.Users.AnyAsync());
        }

        /// <summary>
        /// Create the initial admin user, this is only possible if no users have been created yet
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("registerInitialUser")]
        public async Task<IActionResult> CreateInitialUser([FromBody] InitialRegisterModel model)
        {
            if (await userManager.Users.AnyAsync())
            {
                return BadRequest("Can't create a initial user when one already exists!");
            }

            var createRoleResult = await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            if (!createRoleResult.Succeeded)
            {
                return ThrowError(createRoleResult, "Failed to create admin role!");
            }
            return await CreateUser(model.Username, model.Password, true);
        }

        /// <summary>
        /// Used to create a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isAdmin">If true the user will be able to edit entities</param>
        /// <returns></returns>
        private async Task<IActionResult> CreateUser(string username, string password, bool isAdmin)
        {
            var user = new IdentityUser(username);
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return ThrowError(result, "User Registration Failed");
            }
            if (isAdmin)
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, Roles.Admin);
                if (!addRoleResult.Succeeded)
                {
                    //try to delete the user!
                    await userManager.DeleteAsync(user);
                    return ThrowError(addRoleResult, "User created but failed to add admin role");
                }
            }
            return StatusCode(StatusCodes.Status201Created, new { user.Id, Message = "User Registration Successful" });
        }

        /// <summary>
        /// Used to convert a IdentityResult to a error!
        /// </summary>
        /// <param name="createRoleResult"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static IActionResult ThrowError(IdentityResult createRoleResult, string errorMessage)
        {
            var dictionary = new ModelStateDictionary();
            foreach (IdentityError error in createRoleResult.Errors)
            {
                dictionary.AddModelError(error.Code, error.Description);
            }
            return new BadRequestObjectResult(new
            {
                Message = errorMessage,
                Errors = dictionary
            });
        }
    }
}
