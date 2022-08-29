using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageMoneyServer.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int PortfolioId { get; set; }
        [ForeignKey("PortfolioId")]
        public Portfolio Portfolio { get; set; }
        public int AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
