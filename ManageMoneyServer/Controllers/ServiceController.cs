using ManageMoneyServer.Results;
using ManageMoneyServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ManageMoneyServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ResourceService _resources;
        public ServiceController(ResourceService resources)
        {
            _resources = resources;
        }
        public IActionResult Resourses()
        {
            try
            {
                return new JsonResponse(_resources.Json());
            } catch(Exception ex)
            {
                return new JsonResponse(Models.ViewModels.NotificationType.Error, _resources.Messages["FailedResource"], null);
            }
        }
    }
}
