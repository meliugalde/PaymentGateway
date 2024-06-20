namespace PaymentGateway.Api
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using PaymentGateway.Domain.Models.Responses;

    using System.Linq;

    public class ValidateInputAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "See the errors property for details.",
                    Instance = context.HttpContext.Request.Path
                };

                context.Result = new BadRequestObjectResult(new PaymentResponse { Status = Domain.Enum.PaymentStatus.Rejected, StatusString = Domain.Enum.PaymentStatus.Rejected.ToString() });
            }
        }
    }

}
