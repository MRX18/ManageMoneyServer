namespace ManageMoneyServer.Api.Exchanges.Models
{
    public class NasdaqAssetModel
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Lastsale { get; set; }
        public string Netchange { get; set; }
        public string Pctchange { get; set; }
        public string MarketCap { get; set; }
    }
}
