using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMoneyServer.Models
{
    public class Language
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        [JsonIgnore]
        public List<AssetTypeInfo> AssetTypeInfos { get; set; }
    }
}
