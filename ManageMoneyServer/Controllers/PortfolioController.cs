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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Authorize]
    [Route("api/[controller]/{action=Index}")]
    [ApiController]
    public partial class PortfolioController : ControllerBase
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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], 
                    await PortfolioRepository.GetListAsync(p => p.UserId == Context.User.Id, true, p => p.AssetTypes));
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to get portfolio");
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
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

                portfolio.AssetTypes = assetTypes;
                portfolio.UserId = Context.User.Id;
                Portfolio createdPortfolio = await PortfolioRepository.CreateAsync(portfolio);

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], createdPortfolio);
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to create portfolio", portfolio);
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Index(int portfolioId)
        {
            try
            {
                if (!await PortfolioRepository.HasAsync(p => p.PortfolioId == portfolioId && p.UserId == Context.User.Id))
                {
                    ModelState.AddModelError("PortfolioId", string.Format(Resource.Messages["SelectedItemNotExist"], Resource.Fields["Portfolio"]));
                    return BadRequest();
                }

                await PortfolioRepository.RemoveAsync(portfolioId);
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"]);
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to remove portfolio", portfolioId);
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Index(PortfolioBase portfolio)
        {
            try
            {
                Portfolio currentPortfolio = await PortfolioRepository.FindAsync(p => p.PortfolioId == portfolio.PortfolioId && p.UserId == Context.User.Id, false, p => p.AssetTypes);

                if (currentPortfolio == null)
                {
                    ModelState.AddModelError("PortfolioId", string.Format(Resource.Messages["SelectedItemNotExist"], Resource.Fields["Portfolio"]));
                    return BadRequest();
                }

                currentPortfolio.Name = portfolio.Name;
                currentPortfolio.Description = portfolio.Description;

                currentPortfolio = await PortfolioRepository.UpdateAsync(currentPortfolio);

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], currentPortfolio);
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to update portfolio", portfolio);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
    }
}