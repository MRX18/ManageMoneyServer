using ManageMoneyServer.Results;
using ManageMoneyServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ManageMoneyServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private ILogger<ServiceController> Logger { get; set; }
        private ResourceService Resources { get; set; }
        public ServiceController(ILogger<ServiceController> logger, ResourceService resources)
        {
            Logger = logger;
            Resources = resources;
        }
        public IActionResult Resourses()
        {
            try
            {
                return new JsonResponse(Resources.Json());
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to generate resources");
                return new JsonResponse(Models.ViewModels.NotificationType.Error, Resources.Messages["FailedResource"], null);
            }
        }
    }
}
