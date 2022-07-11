using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ManageMoneyServer.Filters
{
    public class ValidationActionFilter : Attribute, IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                context.Result = (context.Controller as ControllerBase).ValidationProblem(new ValidationProblemDetails(context.ModelState)
                {
                    Status = context.HttpContext.Response.StatusCode
                });
            }

            await next();
        }
    }
}
