using ManageMoneyServer.Validations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources.Fields))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [StringLength(32, MinimumLength = 1, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string Name { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Fields))]
        [StringLength(32, ErrorMessageResourceName = "StringLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public string Description { get; set; }
        // TODO: one asset type will be used for the first time
        [Display(Name = "AssetTypes", ResourceType = typeof(Resources.Fields))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [CollectionLength(1, IsRange = false, ErrorMessageResourceName = "CollectionEqualsLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public List<AssetType> AssetTypes { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
