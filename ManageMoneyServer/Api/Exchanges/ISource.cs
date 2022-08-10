using ManageMoneyServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageMoneyServer.Api.Exchanges
{
    public enum AssetTypes : int
    {
        Stocks = 1,
        Cryptocurrency = 2,
        Fiat = 3
    }
    public interface ISource
    {
        public string SourceName { get; }
        public string Slug { get; }
        public List<AssetTypes> Types { get; }
        public List<string> Urls { get; }
        public Task<IEnumerable<Asset>> GetAssets();
        public Task<Tuple<string, decimal>> GetAssetPrice(string symbol);
        public Task<Dictionary<string, decimal>> GetAssetsPrice(params string[] symbols);
        public Task<Dictionary<DateTime, decimal>> GetHistoryPrices(string symbol, DateTime start, DateTime end);
    }
}
