using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class AssetType
    {
        public int AssetTypeId { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        [JsonIgnore]
        public List<Source> Sources { get; set; }
        public int? LanguageId { get; set; }
        [JsonIgnore]
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }
    }
}