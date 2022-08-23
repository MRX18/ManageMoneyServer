using ManageMoneyServer.Api.Exchanges;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string Name { get; set; }
        public string Sumbol { get; set; }
        public string Slug { get; set; }
        public string Icon { get; set; }
        public int AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }
        public List<Source> Sources { get; set; }
        public int? LanguageId { get; set; }
        [JsonIgnore]
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }

        #region No mapped
        [NotMapped]
        public AssetTypes Type { get; set; }
        #endregion
    }
}
