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
                transaction.Portfolio = await PortfolioRepository.FindByIdAsync(transaction.PortfolioId, false,
                    new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"),
                    new Tuple<IncludeType, string>(IncludeType.Reference, "User"));
                transaction.Source = await SourceRepository.FindByIdAsync(transaction.SourceId, false, new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"));
                transaction.Asset = await AssetRepository.FindByIdAsync(transaction.AssetId, false, new Tuple<IncludeType, string>(IncludeType.Reference, "AssetType"));

                if (!transaction.IsValid(Context, out Dictionary<string, string> messages))
                {
                    foreach(KeyValuePair<string, string> message in messages)
                    {
                        ModelState.AddModelError(message.Key, Resource.Messages[message.Value]);
                    }

                    return BadRequest();
                }

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
