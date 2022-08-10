using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class Source
    {
        public int SourceId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<AssetType> AssetTypes { get; set; }
        [JsonIgnore]
        public List<Asset> Assets { get; set; }
        public int? LanguageId { get; set; }
        [JsonIgnore]
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }
    }
}
