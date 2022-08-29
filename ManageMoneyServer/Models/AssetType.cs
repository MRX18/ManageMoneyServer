using ManageMoneyServer.Api.Exchanges;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class AssetType
    {
        public int AssetTypeId { get; set; }
        public int Value { get; set; }
        public List<AssetTypeInfo> Infos { get; set; }
        [JsonIgnore]
        public List<Source> Sources { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        #region
        [NotMapped]
        public AssetTypes Type => (AssetTypes)Value;
        #endregion
    }
}