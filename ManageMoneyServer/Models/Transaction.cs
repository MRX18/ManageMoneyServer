using ManageMoneyServer.Repositories;
using ManageMoneyServer.Services.Interfaces;
using ManageMoneyServer.Validations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ManageMoneyServer.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        [Display(Name = "Portfolio", ResourceType = typeof(Resources.Fields))]
        [ContainsInDb(typeof(IRepository<Portfolio>), ErrorMessageResourceName = "SelectedItemNotExist", ErrorMessageResourceType = typeof(Resources.Messages))]
        public int PortfolioId { get; set; }
        [JsonIgnore]
        [ForeignKey("PortfolioId")]
        public Portfolio Portfolio { get; set; }
        [Display(Name = "Asset", ResourceType = typeof(Resources.Fields))]
        [ContainsInDb(typeof(IRepository<Asset>), ErrorMessageResourceName = "SelectedItemNotExist", ErrorMessageResourceType = typeof(Resources.Messages))]
        public int AssetId { get; set; }
        [JsonIgnore]
        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }
        [Display(Name = "Source", ResourceType = typeof(Resources.Fields))]
        [ContainsInDb(typeof(IRepository<Source>), ErrorMessageResourceName = "SelectedItemNotExist", ErrorMessageResourceType = typeof(Resources.Messages))]
        public int SourceId { get; set; }
        [JsonIgnore]
        [ForeignKey("SourceId")]
        public Source Source { get; set; }
        [Display(Name = "Price", ResourceType = typeof(Resources.Fields))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public decimal Price { get; set; }
        [Display(Name = "Quantity", ResourceType = typeof(Resources.Fields))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.Messages))]
        public decimal Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public bool IsValid(IContextService context, out Dictionary<string, string> messages)
        {
            messages = new Dictionary<string, string>();

            if (!(Portfolio.AssetTypes.Any(pt => pt.AssetTypeId == Asset.AssetTypeId) && Source.AssetTypes.Any(sa => sa.AssetTypeId == Asset.AssetTypeId)))
                messages.TryAdd("AssetId", "AssetTypeNotMatchToPST");

            if (Portfolio.UserId != context.User.Id)
                messages.TryAdd("PortfolioId", "IncorrectPortfolio");

            return messages.Count == 0;
        }
    }
}
