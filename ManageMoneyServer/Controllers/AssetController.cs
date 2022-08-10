using ManageMoneyServer.Api.Exchanges;
using ManageMoneyServer.Models.ViewModels;
using ManageMoneyServer.Results;
using ManageMoneyServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private SourceService Source { get; set; }
        private ResourceService Resource { get; set; }
        public AssetController(SourceService source, ResourceService resource)
        {
            Source = source;
            Resource = resource;
        }
        [HttpPut]
        public async Task<IActionResult> Cryptocurrencies(string source = null)
        {
            try
            {
                object result = null;
                if (Source.Contains(source, AssetTypes.Cryptocurrency))
                {
                    result = await Source.SynchronizeAssets(source);
                }
                else
                {
                    result = await Source.SynchronizeAssets(AssetTypes.Cryptocurrency);
                }
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], result);
            } catch(Exception ex)
            {
                // TODO: add logger
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpPut]
        public IActionResult Stocks(string source = null)
        {
            return Ok("Stocks");
        }
        [HttpPut]
        public IActionResult Fiats(string source = null)
        {
            return Ok("Fiats");
        }
    }
}