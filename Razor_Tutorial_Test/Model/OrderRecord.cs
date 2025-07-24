using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class OrderRecord
    {
        [Key]
        public int Id { get; set; }

        public int BuyerUserID { get; set; }

        public int SellerUserID { get; set; }

        public int? OfferedItemID { get; set; }

        public int RequestedItemID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Offer price cannot be negative.")]
        public decimal? MoneyOffered { get; set; }

        public string? OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
