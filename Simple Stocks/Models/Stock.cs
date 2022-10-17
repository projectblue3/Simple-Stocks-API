using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        public string Ticker { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Price { get; set; }

        public virtual ICollection<UserStock>? UserStocks { get; set; }
    }
}
