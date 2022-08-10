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
                JObject json = await Request.Get(Urls[1] + "asset/v2/public/asset/asset/get-all-asset");
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

        public Task<Tuple<string, decimal>> GetAssetPrice(string symbol)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, decimal>> GetAssetsPrice(params string[] symbols)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<DateTime, decimal>> GetHistoryPrices(string symbol, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
