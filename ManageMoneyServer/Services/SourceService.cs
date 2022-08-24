using ManageMoneyServer.Api.Exchanges;
using ManageMoneyServer.Models;
using ManageMoneyServer.Models.ViewModels;
using ManageMoneyServer.Repositories;
using ManageMoneyServer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Services
{
    public class SourceService : ISourceService
    {
        private Dictionary<string, ISource> Sources { get; set; } = new Dictionary<string, ISource>();
        public ISource Binance { get; set; }
        public ISource Nasdaq { get; set; }
        public ISource this[string index]
        {
            get 
            {
                if(Sources.TryGetValue(index, out ISource source))
                {
                    return source;
                }

                throw new Exception($"Index\"{index}\" not found");
            }
        }
        private ILogger<SourceService> Logger { get; set; }
        private IContextService Context { get; set; }
        private IRepository<Asset> AssetRepository { get; set; }
        private IRepository<Source> SourceRepository { get; set; }
        public SourceService(
            ILogger<SourceService> logger,
            IContextService context,
            RequestService request, 
            IRepository<Asset> assetRepository, 
            IRepository<Source> sourceRepository)
        {
            Logger = logger;
            Context = context;
            AssetRepository = assetRepository;
            SourceRepository = sourceRepository;
            Init(request);
        }

        private void Init(RequestService request)
        {
            Binance = new BinanceSource(request);
            Sources.Add(Binance.Slug, Binance);

            Nasdaq = new NasdaqSource(request);
            Sources.Add(Nasdaq.Slug, Nasdaq);
        }

        public bool Contains(string slug, AssetTypes type)
        {
            if(!string.IsNullOrEmpty(slug))
            {
                return Sources.ContainsKey(slug.ToLower()) && Sources[slug].Types.Contains(type);
            }

            return false;
        }

        public bool Contains(string slug)
        {
            if (!string.IsNullOrEmpty(slug))
            {
                return Sources.ContainsKey(slug.ToLower());
            }

            return false;
        }

        public IEnumerator<ISource> GetEnumerator()
        {
            return Sources.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public async Task<Dictionary<string, SynchronizedViewModel<Asset>>> SynchronizeAssets()
        {
            Dictionary<string, SynchronizedViewModel<Asset>> result = new Dictionary<string, SynchronizedViewModel<Asset>>();
            
            foreach (ISource source in Sources.Values)
            {
                result.Add(source.SourceName, await SynchronizeAssets(source));
            }

            return result;
        }
        public async Task<Dictionary<string, SynchronizedViewModel<Asset>>> SynchronizeAssets(AssetTypes type)
        {
            Dictionary<string, SynchronizedViewModel<Asset>> result = new Dictionary<string, SynchronizedViewModel<Asset>>();

            foreach (ISource source in Sources.Values)
            {
                if(source.Types.Contains(type))
                {
                    result.Add(source.SourceName, await SynchronizeAssets(source));
                }
            }

            return result;
        }

        public async Task<SynchronizedViewModel<Asset>> SynchronizeAssets(string source)
        {
            source = source.ToLower();

            if(!string.IsNullOrEmpty(source) && Sources.ContainsKey(source))
            {
                return await SynchronizeAssets(Sources[source]);
            }

            throw new ArgumentNullException("The source name is null or not in the source list");
        }
        private async Task<SynchronizedViewModel<Asset>> SynchronizeAssets(ISource source)
        {
            SynchronizedViewModel<Asset> result = new SynchronizedViewModel<Asset>();
            
            try
            {
                List<Asset> sourceAssets = (await source.GetAssets()).ToList();

                if (sourceAssets.Count > 0)
                {
                    List<Asset> assets2Update = new List<Asset>();

                    Source sourceInfo = await SourceRepository.FindAsync(s => s.Slug == source.Slug);
                    await SourceRepository.Attach(sourceInfo);

                    List<Asset> assets = await AssetRepository.GetListAsync(a => source.Types.Contains((AssetTypes)a.AssetType.Value), a => a.AssetType, a => a.Sources);

                    for (int i = 0; i < sourceAssets.Count; i++)
                    {
                        Asset sourceAsset = sourceAssets[i];
                        sourceAsset.Sources = new List<Source> { sourceInfo };
                        sourceAsset.LanguageId = Context.DefaultLanguage.LanguageId;
                        sourceAsset.AssetTypeId = Context.AssetTypes.First(a => a.Type == sourceAsset.Type).AssetTypeId;

                        Asset asset = assets.FirstOrDefault(a => a.Sumbol.Equals(sourceAsset.Sumbol, StringComparison.OrdinalIgnoreCase));

                        if (asset != null)
                        {
                            if (!asset.Sources.Any(s => s.Slug == source.Slug))
                            {
                                asset.Sources.Add(sourceInfo);
                                assets2Update.Add(asset);
                            }
                            sourceAssets.Remove(sourceAsset);
                            assets.Remove(asset);
                            i--;
                        }
                    }

                    // TODO: need to check the update functionality when the second source is available
                    if (assets2Update.Count > 0)
                    {
                        Asset[] assets2UpdateArray = assets2Update.ToArray();
                        await AssetRepository.Attach(assets2UpdateArray);
                        await AssetRepository.UpdateAsync(assets2UpdateArray);
                    }
                    await AssetRepository.CreateAsync(sourceAssets.ToArray());
                    await AssetRepository.RemoveAsync(assets.ToArray());

                    result.Added = sourceAssets;
                    result.Removed = assets;
                }
            } catch(Exception ex)
            {
                Logger.LogError(ex, $"Asset sync problem for {source.SourceName}", source);
            }

            return result;
        }
    }
}
