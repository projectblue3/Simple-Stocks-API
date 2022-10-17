using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class UserFollow
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(SourceUserId))]
        public User? SourceUser { get; set; }
        public int SourceUserId { get; set; }

        [ForeignKey(nameof(FollowedUserId))]
        public User? FollowedUser { get; set; }
        public int FollowedUserId { get; set; }
    }
}
