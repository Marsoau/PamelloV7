using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Exceptions;

namespace PamelloV7.Server.Filters
{
    public class PamelloExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }
        public void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception is PamelloControllerException exception) {
                context.Result = exception.Result;
                context.ExceptionHandled = true;
            }
            else if (context.Exception is PamelloException x) {
                context.Result = new BadRequestObjectResult(x.Message);
                context.ExceptionHandled = true;
            }
        }
    }
}
