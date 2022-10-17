using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class UserBlock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(SourceUserId))]
        public User? SourceUser { get; set; }
        public int SourceUserId { get; set; }

        [ForeignKey(nameof(BlockedUserId))]
        public User? BlockedUser { get; set; }
        public int BlockedUserId { get; set; }
    }
}
