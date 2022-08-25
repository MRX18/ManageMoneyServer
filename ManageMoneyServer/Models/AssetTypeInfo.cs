using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManageMoneyServer.Models
{
    public class AssetTypeInfo
    {
        public int AssetTypeInfoId { get; set; }
        public string Name { get; set; }
        public int LanguageId { get; set; }
        public int AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }
        [JsonIgnore]
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }
    }
}
