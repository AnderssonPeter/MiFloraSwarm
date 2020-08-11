using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiFloraGateway
{
    /// <summary>
    /// Used to return a error when there are validation errors based on the model validation rules.
    /// </summary>
    public class ValidatorActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Do nothing
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Intercept the request before the controller has executed, and reject the request if there are validation errors!
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
