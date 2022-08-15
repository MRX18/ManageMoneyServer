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
                JObject json = (await Request.Get(Urls[0] + "api/v3/ticker/price", new Dictionary<string, object> { ["symbol"] = symbol + SymbolTo })) as JObject;

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
                JArray json = (await Request.Get(Urls[0] + $"api/v3/ticker/price", new Dictionary<string, object> { ["symbols"] = JsonConvert.SerializeObject(symbols.Select(s => s.ToUpper() + SymbolTo)) })) as JArray;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Example: BTC</param>
        /// <param name="start">Example: DateTime.Now.AddDays(-30)</param>
        /// <param name="end">Example: DateTime.Now</param>
        /// <returns></returns>
        public async Task<Dictionary<DateTime, decimal>> GetHistoryPrices(string symbol, DateTime? start = null, DateTime? end = null)
        {
            // Example response
            //[
            //  [
            //    1499040000000,      // Open time
            //    "0.01634790",       // Open
            //    "0.80000000",       // High
            //    "0.01575800",       // Low
            //    "0.01577100",       // Close
            //    "148976.11427815",  // Volume
            //    1499644799999,      // Close time
            //    "2434.19055334",    // Quote asset volume
            //    308,                // Number of trades
            //    "1756.87402397",    // Taker buy base asset volume
            //    "28.46694368",      // Taker buy quote asset volume
            //    "17928899.62484339" // Ignore.
            //  ]
            //]

            symbol = symbol.ToUpper();
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException($"Variable \"{nameof(symbol)}\" is null or empty");

            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();
            try
            {
                Dictionary<string, object> @params = new Dictionary<string, object> 
                { 
                    ["symbol"] = symbol + SymbolTo,
                    ["interval"] = "1d"
                };

                if(start.HasValue)
                {
                    @params.Add("startTime", DateTimeToUnixTime(start.Value));
                }

                if (start.HasValue)
                {
                    @params.Add("endTime", DateTimeToUnixTime(end.Value));
                }

                JArray json = (await Request.Get(Urls[0] + $"api/v3/klines", @params)) as JArray;
                foreach (JArray item in json)
                {
                    result.Add(UnixTimeToDateTime(Convert.ToInt64(item.ToArray()[0])), Convert.ToDecimal(item.ToArray()[1]));

                    //open data
                    //DateTime openDate = UnixTimeToDateTime(Convert.ToInt64(item.ToArray()[0]));
                    //decimal openPrice = Convert.ToDecimal(item.ToArray()[1]);

                    //close data
                    //DateTime closeDate = UnixTimeToDateTime(Convert.ToInt64(item.ToArray()[6]));
                    //decimal closePrice = Convert.ToDecimal(item.ToArray()[4]);
                }
            }
            catch (Exception ex)
            {
                // TODO: Add logger
            }
            return result;
        }
        private long DateTimeToUnixTime(DateTime date)
        {
            return Convert.ToInt64((date - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }
        private DateTime UnixTimeToDateTime(long unixTime)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(unixTime);
        }
    }
}
