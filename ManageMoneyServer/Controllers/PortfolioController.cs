using ManageMoneyServer.Models;
using ManageMoneyServer.Models.ViewModels;
using ManageMoneyServer.Repositories;
using ManageMoneyServer.Results;
using ManageMoneyServer.Services;
using ManageMoneyServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private ILogger<PortfolioController> Logger { get; set; }
        private ResourceService Resource { get; set; }
        private IContextService Context { get; set; }
        private IRepository<Portfolio> PortfolioRepository { get; set; }
        private IRepository<AssetType> AssetTypeRepository { get; set; }
        public PortfolioController(
            ILogger<PortfolioController> logger,
            ResourceService resource,
            IContextService context,
            IRepository<Portfolio> portfolioRepository,
            IRepository<AssetType> assetTypeRepository)
        {
            Logger = logger;
            Resource = resource;
            Context = context;
            PortfolioRepository = portfolioRepository;
            AssetTypeRepository = assetTypeRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Index(Portfolio portfolio)
        {
            try
            {
                List<AssetType> assetTypes = await AssetTypeRepository.GetListAsync(a => portfolio.AssetTypes.Select(at => at.AssetTypeId).Contains(a.AssetTypeId));

                if (assetTypes.Count < portfolio.AssetTypes.Count)
                {
                    ModelState.AddModelError(nameof(portfolio.AssetTypes), Resource.Messages["AssetTypeNotFound"]);
                    return BadRequest();
                }

                await AssetTypeRepository.Attach(assetTypes.ToArray());
                portfolio.AssetTypes = assetTypes;
                portfolio.UserId = Context.User.Id;
                Portfolio createdPortfolio = await PortfolioRepository.CreateAsync(portfolio);

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], createdPortfolio);
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to create portfolio", portfolio);
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationFailed"]);
            }
        }
    }
}