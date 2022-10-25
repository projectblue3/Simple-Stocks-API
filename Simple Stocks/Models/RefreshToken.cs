using Simple_Stocks.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class RefreshToken
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        public DateTimeOffset ExpiresAt { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual User? User { get; set; }
    }
}
