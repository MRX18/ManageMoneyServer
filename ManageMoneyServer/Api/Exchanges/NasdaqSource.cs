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
    public class NasdaqSource : ISource
    {
        public string SourceName => "Nasdaq";

        public string Slug => "nasdaq";

        public string SymbolTo => "USD";

        public List<AssetTypes> Types => new List<AssetTypes> { AssetTypes.Stocks };

        public List<string> Urls => new List<string> { "https://api.nasdaq.com/" };
        public RequestService Request { get; set; }
        public NasdaqSource(RequestService request)
        {
            Request = request;
        }

        public async Task<Tuple<string, decimal>> GetAssetPrice(string symbol)
        {
            symbol = symbol.ToUpper();
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException($"Variable \"{nameof(symbol)}\" is null or empty");

            Tuple<string, decimal> result = null;
            try
            {
                JObject json = (await Request.Get(Urls[0] + $"api/quote/{symbol}/info", new Dictionary<string, object> { ["assetclass"] = "stocks" })) as JObject;
                
                if(json.ContainsKey("data") && json.Value<object>("data") != null)
                {
                    result = new Tuple<string, decimal>(json["data"].Value<string>("symbol"), Convert.ToDecimal(json["data"]["secondaryData"].Value<string>("lastSalePrice").TrimStart('$')));
                }
            } catch(Exception ex)
            {
                // TODO: Add logger
            }

            return result;
        }

        public async Task<IEnumerable<Asset>> GetAssets()
        {
            try
            {
                bool loop = true;
                const int take = 1000;
                List<NasdaqAssetModel> nasdaqAssets = new List<NasdaqAssetModel>();

                while(loop) {
                    JToken json = await Request.Get(Urls[0] + $"api/screener/stocks", new Dictionary<string, object> { ["limit"] = take, ["offset"] = nasdaqAssets.Count });
                    string symbols = json["data"]["table"]["rows"].ToString();
                    List<NasdaqAssetModel> asstes = JsonConvert.DeserializeObject<List<NasdaqAssetModel>>(symbols);
                    nasdaqAssets.AddRange(asstes);

                    if (asstes.Count < take)
                        loop = false;
                }

                return nasdaqAssets.Select(a => new Asset {
                    Name = a.Name,
                    Sumbol = a.Symbol,
                    Slug = a.Symbol,
                    AssetTypeValue = AssetTypes.Stocks
                });
            } catch(Exception ex)
            {
                // TODO: Add logger
            }

            return null;
        }

        public async Task<Dictionary<string, decimal>> GetAssetsPrice(params string[] symbols)
        {
            if (symbols == null || symbols.Count() == 0 && symbols.Any(s => string.IsNullOrEmpty(s)))
                throw new ArgumentNullException($"Array \"{nameof(symbols)}\" is null or empty");

            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            try
            {
                JObject json = (await Request.Get(Urls[0] + $"api/quote/watchlist?symbol=" + string.Join("&symbol=", symbols.Select(s => s + "%7cstocks")))) as JObject;

                if (json.ContainsKey("data") && json.Value<object>("data") != null)
                {
                    IEnumerable<JToken> assets = json["data"].ToArray();
                    foreach(JObject asset in assets)
                    {
                        result.Add(asset.Value<string>("symbol"), Convert.ToDecimal(asset.Value<string>("lastSalePrice").TrimStart('$')));
                    }
                }
            } catch(Exception ex)
            {
                // TODO: Add logger
            }

            return result;
        }

        public async Task<Dictionary<DateTime, decimal>> GetHistoryPrices(string symbol, DateTime? start = null, DateTime? end = null)
        {
            symbol = symbol.ToUpper();
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException($"Variable \"{nameof(symbol)}\" is null or empty");

            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();
            try
            {
                Dictionary<string, object> @params = new Dictionary<string, object>
                {
                    ["assetclass"] = "stocks"
                };

                if (start.HasValue)
                {
                    @params.Add("fromdate", start.Value.ToString("yyyy-MM-dd"));
                }

                if (start.HasValue)
                {
                    @params.Add("todate", end.Value.ToString("yyyy-MM-dd"));
                }

                JObject json = (await Request.Get(Urls[0] + $"api/quote/AAPL/chart", @params)) as JObject;

                if(json.ContainsKey("data") && json.Value<object>("data") != null)
                {
                    IEnumerable<JToken> assets = json["data"]["chart"].ToArray();
                    foreach(JObject asset in assets)
                    {
                        result.Add(asset["z"].Value<DateTime>("dateTime"), asset["z"].Value<decimal>("value"));
                    }
                }
            } catch(Exception ex)
            {
                // TODO: Add logger
            }

            return result;
        }
    }
}
