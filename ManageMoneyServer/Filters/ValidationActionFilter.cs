using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ManageMoneyServer.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context) 
        { 
            if(!context.ModelState.IsValid)
            {
                if(context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                context.Result = (context.Controller as ControllerBase).ValidationProblem(new ValidationProblemDetails(context.ModelState) 
                { 
                    Status = context.HttpContext.Response.StatusCode
                });
            }
        }
    }
}
