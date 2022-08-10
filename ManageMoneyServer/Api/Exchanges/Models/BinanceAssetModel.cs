namespace ManageMoneyServer.Api.Exchanges.Models
{
    public class BinanceAssetModel
    {
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        public bool IsLegalMoney { get; set; }
        public bool Etf { get; set; }
        public bool Delisted { get; set; }
        public bool Trading { get; set; }
    }
}