using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace MiFloraGateway
{
    public class IdentityBaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public IdentityBaseController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Used to create a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isAdmin">If true the user will be able to edit entities</param>
        /// <returns></returns>
        protected async Task<IActionResult> CreateUserAsync(string username, string password, bool isAdmin)
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
        /// <param name="identityResult"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected static BadRequestObjectResult ThrowError(IdentityResult identityResult, string errorMessage) =>
            new BadRequestObjectResult(identityResult.ToErrorResult(errorMessage, _ => null));

        /// <summary>
        /// Used to convert a IdentityResult to a error!
        /// </summary>
        /// <param name="identityResult"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected static BadRequestObjectResult ThrowError(IdentityResult identityResult, string errorMessage, Func<string, string?> fieldResolver) =>
            new BadRequestObjectResult(identityResult.ToErrorResult(errorMessage, fieldResolver));
        

        /// <summary>
        /// Used to convert a IdentityResult to a error!
        /// </summary>
        /// <param name="identityResult"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected static BadRequestObjectResult ThrowError(ErrorResult errorResult) =>
            new BadRequestObjectResult(errorResult);
        
    }

    public static class IdentityResultExtensionMethods
    {

        private static Dictionary<string, string> translator = new Dictionary<string, string>
        {
            { "InvalidUserName", "Username" },
            { "DuplicateUserName", "Username" },
            { "PasswordMismatch", "Password" },
            { "PasswordTooShort", "Password" },
            { "PasswordRequiresUniqueChars", "Password" },
            { "PasswordRequiresNonAlphanumeric", "Password" },
            { "PasswordRequiresDigit", "Password" },
            { "PasswordRequiresLower", "Password" },
            { "PasswordRequiresUpper", "Password" }
        };


        public static IEnumerable<ErrorResultField> ToErrorResultFields(this IEnumerable<IdentityError> errors) =>
            errors.ToErrorResultFields(x => null);

        public static IEnumerable<ErrorResultField> ToErrorResultFields(this IEnumerable<IdentityError> errors, string field) =>
            errors.ToErrorResultFields(x => field);

        public static IEnumerable<ErrorResultField> ToErrorResultFields(this IEnumerable<IdentityError> errors, Func<string, string?> fieldResolver) =>
            errors.Select(x => new ErrorResultField(
                field: fieldResolver(x.Code) ?? translator.GetValueOrDefault(x.Code) ?? throw new InvalidOperationException("Could not translate the value"),
                code: x.Code,
                description: x.Description
            ));

        public static ErrorResult ToErrorResult(this IdentityResult result, string message, Func<string, string?> fieldResolver) =>
            new ErrorResult {
                ErrorMessage = message,
                Errors = result.Errors.ToErrorResultFields(fieldResolver)
            };
    }

    public class ErrorResultField
    {
        public ErrorResultField()
        {

        }

        public ErrorResultField(string field, string description, string code = null) :
            this(new[] { field }, description, code)
        {

        }

        public ErrorResultField(string[] fields, string description, string code = null)
        {
            this.Fields = fields;
            this.Description = description;
            this.Code = code;
        }

        [Required]
        public string[] Fields { get; set; } = null!;

        public string? Code { get; set; }

        [Required]
        public string Description { get; set; } = null!;
    }

    public class SuccessResult
    {
        [Required]
        public string SuccessMessage { get; set; } = null!;
    }

    public class ErrorResult
    {
        [Required]
        public string ErrorMessage { get; set; } = null!;

        [Required]
        public IEnumerable<ErrorResultField> Errors { get; set; } = null!;
    }
}
