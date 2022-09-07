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
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Authorize]
    [Route("api/[controller]/{Action=Index}")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private ILogger<TransactionController> Logger { get; set; }
        private IContextService Context { get; set; }
        private ResourceService Resource { get; set; }
        private IRepository<Transaction> TransactionRepository { get; set; }
        private IRepository<Portfolio> PortfolioRepository { get; set; }
        private IRepository<Source> SourceRepository { get; set; }
        private IRepository<Asset> AssetRepository { get; set; }
        public TransactionController(
            ILogger<TransactionController> logger,
            IContextService context,
            ResourceService resource,
            IRepository<Transaction> transactionRepository,
            IRepository<Portfolio> portfolioRepository,
            IRepository<Source> sourceRepository,
            IRepository<Asset> assetRepository)
        {
            Logger = logger;
            Context = context;
            Resource = resource;
            TransactionRepository = transactionRepository;
            PortfolioRepository = portfolioRepository;
            SourceRepository = sourceRepository;
            AssetRepository = assetRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Index(Transaction transaction)
        {
            try
            {
                Portfolio portfolio = await PortfolioRepository.FindByIdAsync(transaction.PortfolioId, 
                    new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"),
                    new Tuple<IncludeType, string>(IncludeType.Reference, "User"));
                Source source = await SourceRepository.FindByIdAsync(transaction.SourceId, new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"));
                Asset asset = await AssetRepository.FindByIdAsync(transaction.AssetId, new Tuple<IncludeType, string>(IncludeType.Reference, "AssetType"));

                if(!(portfolio.AssetTypes.Any(pt => pt.AssetTypeId == asset.AssetTypeId) && source.AssetTypes.Any(sa => sa.AssetTypeId == asset.AssetTypeId)))
                    ModelState.AddModelError("AssetId", Resource.Messages["AssetTypeNotMatchToPST"]);

                if (portfolio.UserId != Context.User.Id)
                    ModelState.AddModelError("PortfolioId", Resource.Messages["IncorrectPortfolio"]);

                if (ModelState.ErrorCount != 0)
                    return BadRequest();

                transaction.CreateAt = DateTime.Now;
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], await TransactionRepository.CreateAsync(transaction));
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to create transaction", transaction);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
    }
}
