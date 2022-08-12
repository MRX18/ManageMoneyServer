using ManageMoneyServer.Api.Exchanges.Models;
using ManageMoneyServer.Models;
using ManageMoneyServer.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMoneyServer.Api.Exchanges
{
    public class BinanceSource : ISource
    {
        public string SourceName => "Binance";
        public string Slug => "binance";
        public string SymbolTo => "USDT";
        public List<AssetTypes> Types => new List<AssetTypes> { AssetTypes.Cryptocurrency };
        public List<string> Urls => new List<string> { "https://api.binance.com/", "https://www.binance.com/bapi/" };
        private RequestService Request { get; set; }

        public BinanceSource(RequestService request)
        {
            Request = request;
        }

        public async Task<IEnumerable<Asset>> GetAssets()
        {
            try
            {
                JToken json = await Request.Get(Urls[1] + "asset/v2/public/asset/asset/get-all-asset");
                string symbols = json["data"].ToString();

                if (!string.IsNullOrEmpty(symbols))
                {
                    List<BinanceAssetModel> binanceAssets = JsonConvert.DeserializeObject<List<BinanceAssetModel>>(symbols);
                    return binanceAssets.Where(a => !a.Delisted && !a.Etf && a.Trading).Select(a => new Asset
                    {
                        Name = a.AssetName,
                        Sumbol = a.AssetCode,
                        Slug = a.AssetCode,
                        AssetTypeValue = AssetTypes.Cryptocurrency
                    });
                }
            } catch(Exception ex) 
            { 
                // TODO: Add logger
            }

            return null;
        }

        /// <param name="symbol">Examle: BTC</param>
        public async Task<Tuple<string, decimal>> GetAssetPrice(string symbol)
        {
            symbol = symbol.ToUpper();
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException($"Variable \"{nameof(symbol)}\" is null or empty");

            Tuple<string, decimal> result = null;
            try
            {
                JObject json = (await Request.Get(Urls[0] + "api/v3/ticker/price", new Dictionary<string, string> { ["symbol"] = symbol + SymbolTo })) as JObject;

                if (json.ContainsKey("symbol") && json.ContainsKey("price"))
                {
                    result = new Tuple<string, decimal>(json.Value<string>("symbol"), json.Value<decimal>("price"));
                }
            } catch(Exception ex)
            {
                // TODO: Add logger
            }
            return result;
        }

        public async Task<Dictionary<string, decimal>> GetAssetsPrice(params string[] symbols)
        {
            if (symbols == null || symbols.Count() == 0 && symbols.Any(s => string.IsNullOrEmpty(s)))
                throw new ArgumentNullException($"Array \"{nameof(symbols)}\" is null or empty");

            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            try
            {
                JArray json = (await Request.Get(Urls[0] + $"api/v3/ticker/price", new Dictionary<string, string> { ["symbols"] = JsonConvert.SerializeObject(symbols.Select(s => s.ToUpper() + SymbolTo)) })) as JArray;
                foreach(JObject item in json)
                {
                    if (item.ContainsKey("symbol") && item.ContainsKey("price"))
                    {
                        result.Add(item.Value<string>("symbol"), item.Value<decimal>("price"));
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Add logger
            }
            return result;
        }

        public Task<Dictionary<DateTime, decimal>> GetHistoryPrices(string symbol, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
