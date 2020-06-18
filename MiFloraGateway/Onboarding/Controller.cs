using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cronos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Authentication;
using MiFloraGateway.Database;

namespace MiFloraGateway.Onboarding
{
    [Route("Onboarding")]
    [ApiController]
    public class Controller : IdentityBaseController
    {
        private readonly ILogger<Controller> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DatabaseContext databaseContext;
        private readonly ISettingsManager settingsManager;
        private readonly IDataTransmitter dataTransmitter;

        public Controller(ILogger<Controller> logger, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, 
            DatabaseContext databaseContext, ISettingsManager settingsManager, IDataTransmitter dataTransmitter) : base(userManager)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.databaseContext = databaseContext;
            this.settingsManager = settingsManager;
            this.dataTransmitter = dataTransmitter;
        }

        /// <summary>
        /// Check if any users have been created yet.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("isSetup")]
        public async Task<ActionResult<bool>> IsSetup()
        {
            return Ok(await userManager.Users.AnyAsync());
        }

        private void FixTypes(Dictionary<Settings, object> settings)
        {

            foreach (var (setting, value) in settings.ToArray())
            {
                var attribute = Enum<Settings>.GetAttribute<SettingAttribute>(setting);
                if (value != null && value.GetType() == typeof(long) && attribute.Type == typeof(int))
                {
                    settings[setting] = (int)(long)value;
                }
            }
        }

        /// <summary>
        /// Create the initial admin user, this is only possible if no users have been created yet
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setup")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(ErrorResult))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(SuccessResult))]
        public async Task<ActionResult<SuccessResult>> Setup([FromBody] SetupModel model)
        {
            if (await userManager.Users.AnyAsync())
            {
                return BadRequest("Can't create a initial user when one already exists!");
            }

            var errors = new List<ErrorResultField>();
            //Validate password requirements!
            foreach (var v in userManager.PasswordValidators)
            {
                var passwordResult = await v.ValidateAsync(userManager, null, model.Password);
                if (!passwordResult.Succeeded)
                {
                    errors.AddRange(passwordResult.Errors.ToErrorResultFields());
                }
            }

            foreach (var v in userManager.UserValidators)
            {
                var usernameResult = await v.ValidateAsync(userManager, new IdentityUser(model.Username));
                if (!usernameResult.Succeeded)
                {
                    errors.AddRange(usernameResult.Errors.ToErrorResultFields());
                }
            }

            FixTypes(model.Settings);
            foreach (var (setting, value) in model.Settings)
            {
                var attribute = Enum<Settings>.GetAttribute<SettingAttribute>(setting);

                if (attribute.IsRequired && value == null)
                {
                    errors.Add(new ErrorResultField(                    
                        description: "null is not allowed",
                        field: "Settings." + setting
                    ));
                    continue;
                }
                else if (value != null && value.GetType() != attribute.Type)
                {
                    errors.Add(new ErrorResultField(
                        description: "Invalid type",
                        field: "Settings." + setting
                    ));
                }
                if (attribute is StringSettingAttribute stringSettingAttribute)
                {
                    var stringValue = (string?)value;
                    if (attribute.IsRequired && string.IsNullOrEmpty(stringValue))
                    {
                        errors.Add(new ErrorResultField(
                            description: "null is not allowed",
                            field: "Settings." + setting
                        ));
                        continue;
                    }
                    switch (stringSettingAttribute.StringType)
                    {
                        case StringSettingType.Cron:
                            ValidateCronExpression(errors, setting, stringValue);
                            break;
                        case StringSettingType.IPAddressOrHostname:
                            if (!Regex.IsMatch(stringValue, ValidationPatterns.IPAddressOrHostnameRegex))
                            {
                                errors.Add(new ErrorResultField(
                                    description: "Invalid input",
                                    field: "Settings." + setting
                                ));
                            }
                            break;
                    }
                }
            }

            if (errors.Any())
            {
                return ThrowError(new ErrorResult
                {
                    ErrorMessage = "Validation error occurred",
                    Errors = errors
                });
            }

            var result = await dataTransmitter.VerifySettingsAsync((string)model.Settings[Settings.MQTTClientId],
                                                (string)model.Settings[Settings.MQTTServerAddress],
                                                (int)model.Settings[Settings.MQTTPort],
                                                (string)model.Settings[Settings.MQTTUsername],
                                                (string)model.Settings[Settings.MQTTPassword],
                                                (bool)model.Settings[Settings.MQTTUseTLS]);
            if (!result.Success)
            {
                return ThrowError(new ErrorResult
                {
                    ErrorMessage = "Validation error occurred",
                    Errors = new[] { new ErrorResultField(new[] {
                        "Settings.MQTTClientId",
                        "Settings.MQTTServerAddress",
                        "Settings.MQTTUsername",
                        "Settings.MQTTPassword",
                        "Settings.MQTTUseTLS" }, result.Message!)
                    }
                });
            }

            await using (var transaction = await databaseContext.Database.BeginTransactionAsync())
            {
                var createRoleResult = await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                if (!createRoleResult.Succeeded)
                {
                    return ThrowError(createRoleResult, "Failed to create admin role!");
                }
                var user = await CreateUserAsync(model.Username, model.Password, true);


                foreach (var (setting, value) in model.Settings)
                {
                    //This wont work the type is wrong (its Object)
                    await settingsManager.SetUnsafeAsync(setting, value);
                }

                await transaction.CommitAsync();                
            }

            return Ok(new SuccessResult
            {
                SuccessMessage = "Onboarding completed"
            });
        }

        private static void ValidateCronExpression(List<ErrorResultField> errors, Settings setting, string cronExpression)
        {
            try
            {
                var parts = cronExpression.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var format = CronFormat.Standard;

                if (parts.Length == 6)
                {
                    format |= CronFormat.IncludeSeconds;
                }
                else if (parts.Length != 5)
                {
                    throw new CronFormatException("Only supports 5 or 6 (with seconds) part-based expressions");
                }

                CronExpression.Parse(cronExpression, format);
            }
            catch (CronFormatException ex)
            {
                errors.Add(new ErrorResultField(
                    description: ex.Message,
                    field: "Settings." + setting
                ));
            }
        }
    }
}