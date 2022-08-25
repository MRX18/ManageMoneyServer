using Newtonsoft.Json;
using System.Collections.Generic;

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
    }
}
