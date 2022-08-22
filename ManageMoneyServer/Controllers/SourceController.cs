using ManageMoneyServer.Api.Exchanges;
using ManageMoneyServer.Models;
using ManageMoneyServer.Models.ViewModels;
using ManageMoneyServer.Repositories;
using ManageMoneyServer.Results;
using ManageMoneyServer.Services;
using ManageMoneyServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceController : ControllerBase
    {
        private IContextService Context { get; set; }
        private ISourceService Source { get; set; }
        private ResourceService Resource { get; set; }
        private IRepository<Asset> AssetRepository { get; set; }
        private IRepository<Source> SourceRepository { get; set; }
        private IRepository<AssetType> AssetTypeRepository { get; set; }
        public SourceController(IContextService context,
            ISourceService source, 
            ResourceService resource, 
            IRepository<Asset> assetRepository,
            IRepository<Source> sourceRepository,
            IRepository<AssetType> assetTypeRepository)
        {
            Context = context;
            Source = source;
            Resource = resource;
            AssetRepository = assetRepository;
            SourceRepository = sourceRepository;
            AssetTypeRepository = assetTypeRepository;
        }
        [HttpPut]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Source> sources = await SourceRepository.GetListAsync();
                List<Source> sourcesToAdd = new List<Source>();

                foreach (ISource source in Source)
                {
                    Source current = sources.FirstOrDefault(s => s.Slug == source.Slug);
                    if (current == null)
                    {
                        List<AssetType> types = await AssetTypeRepository.GetListAsync(at => source.Types.Contains((AssetTypes)at.Value));
                        await AssetTypeRepository.Attach(types.ToArray());

                        sourcesToAdd.Add(new Source
                        {
                            Name = source.SourceName,
                            Slug = source.Slug,
                            AssetTypes = types,
                            LanguageId = 1 // TODO: add deffault language id to context
                        });
                        continue;
                    }
                    sources.Remove(current);
                }

                await SourceRepository.CreateAsync(sourcesToAdd.ToArray());
                await SourceRepository.RemoveAsync(sources.ToArray());

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], new SynchronizedViewModel<Source> 
                { 
                    Added = sourcesToAdd,
                    Removed = sources
                });
            } catch (Exception ex)
            {
                // TODO: add logger
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(AssetTypes? type = null)
        {
            try
            {
                if (type.HasValue)
                    return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"],
                        await SourceRepository.GetListAsync(s => s.AssetTypes.Any(t => t.Value == (int)type.Value)));
                else
                    return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], await SourceRepository.GetListAsync());
            }
            catch (Exception ex)
            {
                // TODO: add logger
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Assets(string source = null)
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
        [HttpGet]
        public async Task<IActionResult> Assets([Required][StringLength(16, MinimumLength = 2)]string source, 
            int? skip = null, int? take = null)
        {
            try
            {
                List<Asset> assets = new List<Asset>();
                if (Source.Contains(source))
                {
                    IQueryable<Asset> query = await AssetRepository.GetAsync(a => a.Sources.Any(s => s.Slug == source), a => a.Sources);

                    if (skip.HasValue)
                    {
                        query = query.Skip(skip.Value);
                    }

                    if (take.HasValue)
                    {
                        query = query.Take(take.Value);
                    }

                    assets = query.ToList();
                }

                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], assets);
            } catch(Exception ex)
            {
                return new JsonResponse(NotificationType.Error, Resource.Messages["OperationFailed"]);
            }
        }
        [HttpGet]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "source", "symbol" })]
        public async Task<IActionResult> Price([Required][StringLength(16, MinimumLength = 2)]string source,
            [Required][StringLength(5, MinimumLength = 1)]string symbol)
        {
            if (Source.Contains(source))
            {
                Tuple<string, decimal> price = await Source[source].GetAssetPrice(symbol);
                return new JsonResponse(NotificationType.Success, Resource.Messages["OperationSuccessful"], price?.Item2);
            }
            return new JsonResponse(NotificationType.Error, string.Format(Resource.Messages["FailedGetPrice"], symbol, source));
        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok("Test");
        }
    }
}