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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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
        private async Task<bool> CheckTransaction(Transaction transaction)
        {
            transaction.Portfolio = await PortfolioRepository.FindByIdAsync(transaction.PortfolioId, false,
                    new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"),
                    new Tuple<IncludeType, string>(IncludeType.Reference, "User"));
            transaction.Source = await SourceRepository.FindByIdAsync(transaction.SourceId, false, new Tuple<IncludeType, string>(IncludeType.Collection, "AssetTypes"));
            transaction.Asset = await AssetRepository.FindByIdAsync(transaction.AssetId, false, new Tuple<IncludeType, string>(IncludeType.Reference, "AssetType"));

            bool result = transaction.IsValid(Context, out Dictionary<string, string> messages);
            
            foreach (KeyValuePair<string, string> message in messages)
            {
                ModelState.AddModelError(message.Key, Resource.Messages[message.Value]);
            }

            return result;
        }
        [HttpPost("")]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            try
            {
                if (!await CheckTransaction(transaction))
                {
                    return BadRequest();
                }

                transaction.CreateAt = DateTime.Now;
                transaction.UpdateAt = DateTime.Now;
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], await TransactionRepository.CreateDetachedAsync(transaction));
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to create transaction", transaction);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
        [HttpPut("")]
        public async Task<IActionResult> Update(TransactionBase transaction)
        {
            try
            {
                Transaction currentTransaction = await TransactionRepository.FindAsync(t => t.TransactionId == transaction.TransactionId &&
                    t.Portfolio.UserId == Context.User.Id, false, t => t.Portfolio);

                if (currentTransaction == null)
                {
                    ModelState.AddModelError("TransactionId", string.Format(Resource.Messages["SelectedItemNotExist"], Resource.Fields["Transaction"]));
                    return BadRequest();
                }

                currentTransaction.Price = transaction.Price;
                currentTransaction.Quantity = transaction.Quantity;
                currentTransaction.IsPurchase = transaction.IsPurchase;
                currentTransaction.UpdateAt = DateTime.Now;

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], await TransactionRepository.UpdateDetachedAsync(currentTransaction));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update transaction", transaction);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
        [HttpDelete("")]
        public async Task<IActionResult> Delete(int transactionId)
        {
            try
            {
                if (!await TransactionRepository.HasAsync(t => t.TransactionId == transactionId && t.Portfolio.UserId == Context.User.Id, t => t.Portfolio))
                {
                    ModelState.AddModelError("TransactionId", string.Format(Resource.Messages["SelectedItemNotExist"], Resource.Fields["Transaction"]));
                    return BadRequest();
                }

                await TransactionRepository.RemoveAsync(transactionId);
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"]);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to remove transaction", transactionId);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
        [HttpGet("")]
        public async Task<IActionResult> Get(int? portfolioId = null, int? assetId = null)
        {
            try
            {
                Expression<Func<Transaction, bool>> predicate = t => t.Portfolio.UserId == Context.User.Id;

                if (portfolioId.HasValue)
                {
                    // TODO: add extension methods to add expressions
                    Expression<Func<Transaction, bool>> subPredicate = t => t.PortfolioId == portfolioId.Value;
                    InvocationExpression invokeExpr = Expression.Invoke(subPredicate, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<Transaction, bool>>(Expression.AndAlso(predicate.Body, invokeExpr), predicate.Parameters);
                }

                if (assetId.HasValue)
                {
                    Expression<Func<Transaction, bool>> subPredicate = t => t.AssetId == assetId.Value;
                    InvocationExpression invokeExpr = Expression.Invoke(subPredicate, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<Transaction, bool>>(Expression.AndAlso(predicate.Body, invokeExpr), predicate.Parameters);
                }

                List<Transaction> transactions = await TransactionRepository.GetListAsync(predicate, true, t => t.Portfolio);

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], transactions);
            } catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to get transactions", portfolioId, assetId);
            }

            return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
        }
    }
}
