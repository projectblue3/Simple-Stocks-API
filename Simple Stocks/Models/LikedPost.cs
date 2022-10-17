using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class LikedPost
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post? Post { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}